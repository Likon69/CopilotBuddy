using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bots.DungeonBuddy.Attributes;
using Bots.DungeonBuddy.Enums;
using Bots.DungeonBuddy.Profiles;
using Styx;
using Styx.Helpers;
using Styx.Loaders;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

namespace Bots.DungeonBuddy
{
    /// <summary>
    /// Charge et gère les scripts de donjons.
    /// Scripts dynamiques depuis Dungeon Scripts\ + types compilés dans l'assembly.
    /// </summary>
    public static class DungeonManager
    {
        private static readonly Dictionary<uint, Type> _dungeonTypes = new();
        private static readonly Dictionary<uint, Dictionary<int, MethodInfo>> _encounterHandlers = new();
        private static readonly Dictionary<uint, Dictionary<int, MethodInfo>> _objectHandlers = new();
        
        private static Dungeon? _currentDungeon;
        private static Composite? _currentDungeonBehavior;

        /// <summary>
        /// Donjon actif
        /// </summary>
        public static Dungeon? CurrentDungeon
        {
            get => _currentDungeon;
            set
            {
                _currentDungeon = value;
                _currentDungeonBehavior = null;
            }
        }

        public static Composite CurrentDungeonBehavior => _currentDungeonBehavior ??= BuildDungeonBehavior();

        /// <summary>
        /// Port de HB 4.3.4 ns26.Class147.smethod_0(Dungeon).
        /// Construit le comportement de script de donjon en enveloppant les handlers
        /// EncounterHandler et ObjectHandler avec leur contexte approprié.
        /// </summary>
        private static Composite BuildDungeonBehavior()
        {
            if (_currentDungeon == null)
                return new PrioritySelector();

            var methodInfos = _currentDungeon.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.GetParameters().Length == 0 &&
                            (m.ReturnType == typeof(Composite) || m.ReturnType.IsSubclassOf(typeof(Composite))))
                .Where(m => m.GetCustomAttributes(typeof(EncounterHandlerAttribute), false).Any() ||
                            m.GetCustomAttributes(typeof(ObjectHandlerAttribute), false).Any());

            var behaviors = new List<Composite>();

            foreach (var method in methodInfos)
            {
                try
                {
                    var composite = method.Invoke(_currentDungeon, null) as Composite;
                    if (composite == null)
                    {
                        Logging.WriteDiagnostic("[DungeonBuddy] Error building dungeon behavior for {0}.{1}: returned null composite", _currentDungeon.Name, method.Name);
                        continue;
                    }

                    var encounterAttrs = method.GetCustomAttributes<EncounterHandlerAttribute>(false).ToList();
                    var objectAttrs = method.GetCustomAttributes<ObjectHandlerAttribute>(false).ToList();

                    if (encounterAttrs.Count > 0)
                    {
                        if (encounterAttrs.Any(attr => attr.BossEntry == 0))
                        {
                            behaviors.Add(new PrioritySelector(
                                new ContextChangeHandler(context => ObjectManager.GetObjectsOfType<WoWUnit>(false, false)),
                                composite));
                        }
                        else
                        {
                            behaviors.Add(new PrioritySelector(
                                new ContextChangeHandler(context => (object)FindBestEncounterUnit(encounterAttrs)!),
                                new Decorator(
                                    ctx => ctx != null || ShouldRunCurrentBossHandler(encounterAttrs),
                                    composite)));
                        }
                    }
                    else if (objectAttrs.Count > 0)
                    {
                        if (objectAttrs[0].ObjectEntry == 0)
                        {
                            var objectRangeSqr = objectAttrs[0].ObjectRangeSqr;
                            behaviors.Add(new PrioritySelector(
                                new ContextChangeHandler(context => ObjectManager.GetObjectsOfType<WoWGameObject>(false, false)
                                    .Where(o => o.IsValid && o.DistanceSqr <= objectRangeSqr)),
                                composite));
                        }
                        else
                        {
                            behaviors.Add(new PrioritySelector(
                                new ContextChangeHandler(context => (object)FindNearestObject(objectAttrs)!),
                                new Decorator(ctx => ctx != null, composite)));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteDiagnostic("[DungeonBuddy] Error building dungeon behavior for {0}.{1}: {2}",
                        _currentDungeon.Name, method.Name, ex.Message);
                }
            }

            return new PrioritySelector(behaviors.ToArray());
        }

        private static WoWUnit? FindBestEncounterUnit(List<EncounterHandlerAttribute> encounterAttrs)
        {
            return ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                .Where(unit => unit.IsValid && unit.IsAlive && encounterAttrs.Any(attr => IsEncounterMatch(unit, attr)))
                .OrderBy(unit => unit.DistanceSqr)
                .FirstOrDefault();
        }

        private static bool IsEncounterMatch(WoWUnit unit, EncounterHandlerAttribute attr)
        {
            if ((long)attr.BossEntry != (long)unit.Entry)
                return false;

            if (attr.Mode == CallBehaviorMode.Combat && !unit.Combat)
                return false;

            if (attr.Mode == CallBehaviorMode.CurrentBoss)
            {
                var currentBoss = BossManager.CurrentBoss;
                if (currentBoss == null || currentBoss.Entry != (uint)attr.BossEntry)
                    return false;
            }

            return unit.DistanceSqr <= attr.BossRangeSqr;
        }

        private static bool ShouldRunCurrentBossHandler(List<EncounterHandlerAttribute> encounterAttrs)
        {
            var currentBoss = BossManager.CurrentBoss;
            if (currentBoss == null)
                return false;

            return encounterAttrs.Any(attr => attr.Mode == CallBehaviorMode.CurrentBoss && currentBoss.Entry == (uint)attr.BossEntry);
        }

        private static WoWGameObject? FindNearestObject(List<ObjectHandlerAttribute> objectAttrs)
        {
            return ObjectManager.GetObjectsOfType<WoWGameObject>(false, false)
                .Where(obj => obj.IsValid && objectAttrs.Any(attr => obj.Entry == attr.ObjectEntry && obj.DistanceSqr <= attr.ObjectRangeSqr))
                .OrderBy(obj => obj.DistanceSqr)
                .FirstOrDefault();
        }

        /// <summary>
        /// Charge les scripts de donjon depuis Dungeon Scripts\ (compilation dynamique, pattern HB 4.3.4),
        /// plus les scripts compilés dans l'assembly.
        /// </summary>
        public static void LoadDungeonScripts()
        {
            _dungeonTypes.Clear();
            _encounterHandlers.Clear();
            _objectHandlers.Clear();

            // 1. Dynamic loading from Dungeon Scripts\ folder (HB 4.3.4 pattern)
            // HB: string text = Path.Combine(Application.StartupPath, "Dungeon Scripts");
            string scriptDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dungeon Scripts");
            if (Directory.Exists(scriptDir))
            {
                Logging.Write("[DungeonBuddy] Compiling dungeon scripts...");
                int errors = 0;

                var scriptPaths = new List<string>();
                scriptPaths.AddRange(Directory.GetFiles(scriptDir, "*.cs", SearchOption.TopDirectoryOnly));
                scriptPaths.AddRange(Directory.GetDirectories(scriptDir, "*", SearchOption.TopDirectoryOnly));

                foreach (string path in scriptPaths)
                {
                    try
                    {
                        var loader = new DynamicLoader<Dungeon>(path, true);
                        if (loader.CompilerResults != null &&
                            loader.CompilerResults.Errors.Cast<CompilerError>().Any(e => !e.IsWarning))
                        {
                            errors++;
                            Logging.Write("[DungeonBuddy] Could not compile dungeon script from {0}", path);
                            foreach (CompilerError err in loader.CompilerResults.Errors)
                            {
                                if (!err.IsWarning)
                                {
                                    Logging.Write($"File: {err.FileName} Line: {err.Line} Error: {err.ErrorText}");
                                }
                            }
                        }
                        else
                        {
                            foreach (var dungeon in loader)
                                RegisterInstance(dungeon);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.WriteDiagnostic("[DungeonBuddy] Error loading {0}: {1}", path, ex.Message);
                        errors++;
                    }
                }

                if (errors > 0)
                    Logging.Write("[DungeonBuddy] Compiling failed for {0} dungeon scripts. Check log for details.", errors);
            }

            // 2. Also scan the compiled assembly for any built-in scripts
            LoadDungeonTypes();

            if (_dungeonTypes.Count == 0)
                Logging.Write("[DungeonBuddy] No dungeon scripts found.");
            else
                Logging.Write("[DungeonBuddy] Loaded {0} dungeon scripts.", _dungeonTypes.Count);
        }

        /// <summary>
        /// Registers a dungeon instance: stores its Type and indexes its handlers.
        /// The instance is disposed after registration.
        /// </summary>
        private static void RegisterInstance(Dungeon dungeon)
        {
            var dungeonId = dungeon.DungeonId;
            if (!_dungeonTypes.ContainsKey(dungeonId))
            {
                _dungeonTypes[dungeonId] = dungeon.GetType();
                IndexHandlers(dungeon.GetType(), dungeonId);
            }
            dungeon.Dispose();
        }

        /// <summary>
        /// Scanne l'assembly compilée pour les types héritant de Dungeon.
        /// </summary>
        private static void LoadDungeonTypes()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var dungeonTypes = assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(Dungeon)) && !t.IsAbstract);

                foreach (var type in dungeonTypes)
                {
                    try
                    {
if (Activator.CreateInstance(type) is not Dungeon instance)
                    continue;

                        var dungeonId = instance.DungeonId;
                        instance.Dispose();

                        if (!_dungeonTypes.ContainsKey(dungeonId))
                        {
                            _dungeonTypes[dungeonId] = type;
                            IndexHandlers(type, dungeonId);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.WriteDiagnostic($"[DungeonBuddy] Error loading {type.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteDiagnostic($"[DungeonBuddy] Error scanning dungeon types: {ex.Message}");
            }
        }

        private static void IndexHandlers(Type dungeonType, uint dungeonId)
        {
            _encounterHandlers[dungeonId] = new Dictionary<int, MethodInfo>();
            _objectHandlers[dungeonId] = new Dictionary<int, MethodInfo>();

            foreach (var method in dungeonType.GetMethods())
            {
                // Index encounter handlers
                var encounterAttrs = method.GetCustomAttributes<EncounterHandlerAttribute>();
                foreach (var attr in encounterAttrs)
                {
                    _encounterHandlers[dungeonId][attr.BossEntry] = method;
                    BossManager.RegisterBoss((uint)attr.BossEntry, attr.BossName);
                }

                // Index object handlers
                var objectAttrs = method.GetCustomAttributes<ObjectHandlerAttribute>();
                foreach (var attr in objectAttrs)
                {
                    _objectHandlers[dungeonId][attr.ObjectEntry] = method;
                }
            }
        }

        /// <summary>
        /// Active le script de donjon approprié.
        /// HB 4.3.4: utilise LfgManager.CurrentLfgDungeonId quand on est en groupe LFG,
        /// sinon GetLfgDungeonIdFromMapId pour faire la correspondance MapId → LFG DungeonId.
        /// </summary>
        public static void SetDungeon(uint mapId)
        {
            uint dungeonId;
            if (LfgManager.CurrentLfgDungeonId > 0)
            {
                dungeonId = LfgManager.CurrentLfgDungeonId;
            }
            else
            {
                try
                {
                    dungeonId = ProfileManager.GetLfgDungeonIdFromMapId(mapId);
                }
                catch (InstanceNotFoundException ex)
                {
                    // Race condition: OnMapChanged fires before LfgManager.CurrentLfgDungeonId
                    // is updated (e.g. SM mapId 189 shared across all 4 wings, or solo entry).
                    // Log a warning and bail — the next pulse will retry with the correct dungeonId.
                    Logging.Write("[DungeonBuddy] Warning: {0}", ex.Message);
                    return;
                }
            }

            SetDungeonById(dungeonId);
        }

        /// <summary>
        /// Active un script de donjon à partir de son ID.
        /// Ce flux est le même que HB 4.3.4 method_43.
        /// </summary>
        public static void SetDungeonById(uint dungeonId)
        {
            if (_currentDungeon?.DungeonId == dungeonId)
            {
                return;
            }

            _currentDungeon?.Detach();
            _currentDungeon?.Dispose();
            _currentDungeon = null;
            _currentDungeonBehavior = null;

            if (_dungeonTypes.TryGetValue(dungeonId, out var dungeonType))
            {
                if (Activator.CreateInstance(dungeonType) is not Dungeon dungeon)
                {
                    Logging.Write("[DungeonBuddy] Failed to instantiate dungeon script type {0}", dungeonType.Name);
                    return;
                }

                _currentDungeon = dungeon;
                _currentDungeon.Attach();
                if (ProfileManager.CurrentProfile == null || ProfileManager.CurrentProfile.DungeonId != dungeonId)
                {
                    ProfileManager.LoadProfileForDungeon(dungeonId);
                }
                BossManager.Initialize(_currentDungeon);
                Logging.Write("Entered dungeon: {0}", _currentDungeon.Name);
            }
            else
            {
                Logging.Write($"[DungeonBuddy] No script found for dungeon {dungeonId}");
            }
        }

        /// <summary>
        /// Obtient le behavior pour un boss spécifique
        /// </summary>
        public static Composite? GetEncounterBehavior(int bossEntryId)
        {
            if (_currentDungeon == null)
                return null;

            var dungeonId = _currentDungeon.DungeonId;
            
            if (_encounterHandlers.TryGetValue(dungeonId, out var handlers) &&
                handlers.TryGetValue(bossEntryId, out var method))
            {
                return method.Invoke(_currentDungeon, null) as Composite;
            }

            return null;
        }

        /// <summary>
        /// Obtient le behavior pour un objet spécifique
        /// </summary>
        public static Composite? GetObjectBehavior(int objectEntryId)
        {
            if (_currentDungeon == null)
                return null;

            var dungeonId = _currentDungeon.DungeonId;
            
            if (_objectHandlers.TryGetValue(dungeonId, out var handlers) &&
                handlers.TryGetValue(objectEntryId, out var method))
            {
                return method.Invoke(_currentDungeon, null) as Composite;
            }

            return null;
        }

        public static void Clear()
        {
            _currentDungeon?.Detach();
            _currentDungeon?.Dispose();
            LfgManager.DungeonCompletedReason = CompleteReason.None;
            _currentDungeon = null;
            _currentDungeonBehavior = null;
            ProfileManager.UnloadProfile();
        }

        /// <summary>
        /// Retourne la position d'entrée d'un donjon depuis son script, SANS activer ce script.
        /// Utilisé par SoloFarm pour naviguer vers l'entrée avant d'entrer dans l'instance.
        /// Évite d'appeler SetDungeonById (qui déclenche Attach/BossManager/Profile) hors instance.
        /// </summary>
        public static WoWPoint GetEntranceForDungeon(uint dungeonId)
        {
            if (!_dungeonTypes.TryGetValue(dungeonId, out var dungeonType))
                return WoWPoint.Zero;

            try
            {
                if (Activator.CreateInstance(dungeonType) is not Dungeon dungeon)
                    return WoWPoint.Zero;

                var entrance = dungeon.Entrance;
                dungeon.Dispose();
                return entrance;
            }
            catch
            {
                return WoWPoint.Zero;
            }
        }

        /// <summary>
        /// Reload dungeon scripts at runtime (HB 4.3.4: DungeonManager.ReloadDungeons()).
        /// </summary>
        public static void ReloadDungeons()
        {
            Logging.Write("[DungeonBuddy] Reloading dungeon scripts...");
            _currentDungeon?.Detach();
            _currentDungeon = null;
            _currentDungeonBehavior = null;
            LoadDungeonScripts();
        }
    }
}