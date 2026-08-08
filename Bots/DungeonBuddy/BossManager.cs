using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic.Pathing;
using Styx.Helpers;
using Bots.DungeonBuddy.Attributes;
using Bots.DungeonBuddy.Profiles;
using TreeSharp;

namespace Bots.DungeonBuddy
{
    /// <summary>
    /// Gère les boss du donjon actuel.
    /// Track les boss tués et le boss actuel à target.
    /// </summary>
    public static class BossManager
    {
        private static readonly HashSet<uint> _killedBossIds = new();
        private static readonly List<Boss> _bosses = new();
        public static readonly Stopwatch BossTimer = new Stopwatch();

        /// <summary>
        /// Fired after a boss is marked as dead. Payload is the killed boss.
        /// </summary>
        public static event Action<Boss> OnBossKill;

        public class Boss
        {
            public uint Entry { get; set; }
            public uint EntryId { get { return Entry; } set { Entry = value; } }
            public string Name { get; set; }
            public bool IsOptional { get; set; }
            public bool Optional { get { return IsOptional; } set { IsOptional = value; } }
            public bool IsDead { get; set; }
            public bool IsAlive => !IsDead;
            public int CastingSpellId { get; set; }
            public CircularQueue<WoWPoint> PathBreadCrumbs { get; set; } = new CircularQueue<WoWPoint>();

            public WoWUnit ToWoWUnit()
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == this.Entry);
            }

            /// <summary>
            /// Best-effort world location: uses live ObjectManager unit if in range,
            /// otherwise WoWPoint.Zero. Callers should guard for Zero.
            /// </summary>
            public WoWPoint Location => ToWoWUnit()?.Location ?? WoWPoint.Zero;

            /// <summary>
            /// Marks this boss as dead. Called by dungeon scripts (HB Boss.MarkAsDead() parity).
            /// </summary>
            public void MarkAsDead() { IsDead = true; }

            /// <summary>
            /// Resets this boss to alive. Called by BossManager.Reset().
            /// </summary>
            public void Reset() { IsDead = false; }
        }

        /// <summary>
        /// HB 4.3.4 parity: returns the first Boss object that is not yet dead,
        /// in registration order — the profile's KillOrder, then any script-only bosses.
        /// Does NOT require the boss to be in ObjectManager range — using ObjectManager
        /// here would return null when bosses are out of draw distance at the dungeon
        /// entrance, causing IsComplete = true and premature 'dungeon complete'.
        /// Matches HB smethod_9: IsAlive check + Optional/KillOptionalBosses guard.
        /// (Faction guard omitted — inner Boss has no Faction field; all registered bosses default to Both.)
        /// </summary>
        public static Boss? CurrentBoss =>
            _bosses
                .Where(b => !b.IsDead && (!b.IsOptional || DungeonBuddySettings.Instance.KillOptionalBosses))
                .FirstOrDefault();

        /// <summary>
        /// Liste de tous les boss du donjon
        /// </summary>
        public static IReadOnlyList<Boss> Bosses => _bosses;
        public static IReadOnlyList<Boss> BossEncounters => _bosses;

        /// <summary>
        /// Initialise les boss pour le donjon actuel.
        /// Remet à zéro les boss tués et re-peuple la liste depuis le profil XML, puis les attributs [EncounterHandler].
        /// </summary>
        public static void Initialize(Dungeon dungeon)
        {
            _killedBossIds.Clear();
            _bosses.Clear();

            if (dungeon == null)
                return;

            var profile = ProfileManager.CurrentProfile;
            if (profile != null)
            {
                foreach (var profileBoss in profile.BossEncounters.OrderBy(b => b.KillOrder))
                    RegisterBoss(profileBoss.Entry, profileBoss.Name, profileBoss.Optional);
            }

            foreach (var method in dungeon.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach (EncounterHandlerAttribute attr in method.GetCustomAttributes(typeof(EncounterHandlerAttribute), false))
                {
                    if (attr.BossEntry != 0)
                        RegisterBoss((uint)attr.BossEntry, attr.BossName);
                }
            }
        }

        /// <summary>
        /// Marque un boss comme tué et fire l'événement OnBossKill.
        /// Also syncs Profile.Boss.MarkAsDead() so PathBreadCrumbs advances to the next boss.
        /// </summary>
        public static void MarkBossDead(uint entryId)
        {
            _killedBossIds.Add(entryId);

            var boss = _bosses.FirstOrDefault(b => b.EntryId == entryId);
            if (boss != null && !boss.IsDead)
            {
                boss.IsDead = true;
                BossTimer.Reset();
                OnBossKill?.Invoke(boss);
            }

            // Sync to Profile.Boss so GetSoloFarmMoveToPoint advances to the next boss.
            ProfileManager.CurrentProfile?.BossEncounters
                .FirstOrDefault(b => b.Entry == entryId)
                ?.MarkAsDead();
        }

        /// <summary>
        /// HB 4.3.4 parity: poll one dead boss candidate per tick and mark it dead.
        /// Used as a dedicated root branch (DungeonBot.method_12 array[2]).
        /// </summary>
        public static Composite CreateCheckForDeadBossBehavior()
        {
            return new PrioritySelector(
                new ContextChangeHandler(ctx => _bosses
                    .Where(b => !b.IsDead)
                    .Select(b => b.ToWoWUnit())
                    .FirstOrDefault(u => u != null && u.IsDead)),
                new Decorator(
                    ctx => ctx is WoWUnit,
                    new global::TreeSharp.Action(ctx =>
                    {
                        var bossUnit = ctx as WoWUnit;
                        if (bossUnit != null)
                        {
                            MarkBossDead(bossUnit.Entry);
                        }

                        return RunStatus.Success;
                    })
                )
            );
        }

        /// <summary>
        /// Register un boss (appelé par DungeonManager lors du chargement des scripts)
        /// </summary>
        public static void RegisterBoss(uint entryId, string name, bool isOptional = false)
        {
            if (!_bosses.Any(b => b.EntryId == entryId))
            {
                _bosses.Add(new Boss
                {
                    EntryId = entryId,
                    Name = name,
                    IsOptional = isOptional,
                    IsDead = false
                });
            }
        }

        /// <summary>
        /// Marque un boss comme tué (par nom, utilisé par l'UI)
        /// </summary>
        public static void MarkBossDead(string name)
        {
            var boss = _bosses.FirstOrDefault(b => b.Name == name);
            if (boss != null)
            {
                boss.IsDead = true;
                _killedBossIds.Add(boss.EntryId);
            }
        }

        /// <summary>
        /// Remet un boss à vivant (par nom, utilisé par l'UI)
        /// </summary>
        public static void ResetBoss(string name)
        {
            var boss = _bosses.FirstOrDefault(b => b.Name == name);
            if (boss != null)
            {
                boss.IsDead = false;
                _killedBossIds.Remove(boss.EntryId);
            }
        }

        /// <summary>
        /// Vérifie si tous les boss obligatoires sont morts
        /// </summary>
        public static bool AreAllRequiredBossesDead()
        {
            return _bosses.Where(b => !b.IsOptional).All(b => b.IsDead);
        }

        /// <summary>
        /// Reset pour nouveau donjon
        /// </summary>
        public static void Reset()
        {
            _killedBossIds.Clear();
            _bosses.Clear();
        }
    }
}