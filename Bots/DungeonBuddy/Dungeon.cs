using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Bots.DungeonBuddy.Avoidance;
using Bots.DungeonBuddy.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals.DBC;
using Styx.WoWInternals.WoWObjects;

namespace Bots.DungeonBuddy
{
    /// <summary>
    /// Classe de base pour tous les scripts de donjon.
    /// Chaque donjon hérite de cette classe.
    /// </summary>
    public abstract class Dungeon : IDisposable
    {
        private readonly List<AvoidInfo> _avoidInfos = new();
        private bool _disposed;

        /// <summary>
        /// Nom du donjon (affiché depuis LFG_Dungeons.dbc)
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (LfgDungeon != null && LfgDungeon.IsValid)
                    return LfgDungeon.Name;
                return string.Empty;
            }
        }

        /// <summary>
        /// ID du donjon dans LFG_Dungeons.dbc
        /// </summary>
        public abstract uint DungeonId { get; }

        /// <summary>
        /// Position de l'entrée du donjon (pour corpse run)
        /// </summary>
        private LfgDungeons? _lfgDungeon;

        private LfgDungeons LfgDungeon
        {
            get
            {
                return _lfgDungeon ??= new LfgDungeons(this.DungeonId);
            }
        }

        public virtual WoWPoint Entrance
        {
            get
            {
                var map = this.LfgDungeon.Map;
                if (map == null)
                {
                    return WoWPoint.Zero;
                }

                Vector2 ghostEntranceLocation = map.GhostEntranceLocation;
                if (Navigator.FindHeight(ghostEntranceLocation.X, ghostEntranceLocation.Y, out float z))
                {
                    return new WoWPoint(ghostEntranceLocation.X, ghostEntranceLocation.Y, z);
                }
                return WoWPoint.Zero;
            }
        }

        /// <summary>
        /// Position de la sortie du donjon
        /// </summary>
        public virtual WoWPoint ExitLocation => WoWPoint.Zero;

        /// <summary>
        /// True si le corpse run peut se faire en volant
        /// </summary>
        public virtual bool IsFlyingCorpseRun => false;

        public virtual Styx.Helpers.CircularQueue<WoWPoint> CorpseRunBreadCrumb => new Styx.Helpers.CircularQueue<WoWPoint>();

        /// <summary>
        /// True si le donjon est terminé (tous les boss tués)
        /// </summary>
        public virtual bool IsComplete => BossManager.CurrentBoss == null;

        // ═══════════════════════════════════════════════════════════
        // TARGETING FILTERS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Filtre de ciblage: Ajouter des cibles
        /// </summary>
        public virtual void IncludeTargetsFilter(List<WoWObject> incoming, HashSet<WoWObject> outgoing)
        {
        }

        /// <summary>
        /// Filtre de ciblage: Retirer des cibles
        /// </summary>
        public virtual void RemoveTargetsFilter(List<WoWObject> units)
        {
        }

        /// <summary>
        /// Filtre de ciblage: Modifier les priorités
        /// </summary>
        public virtual void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
        }

        // ═══════════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Appelé quand le joueur entre dans le donjon
        /// </summary>
        internal void Attach()
        {
            Targeting.Instance.IncludeTargetsFilter += IncludeTargetsFilter;
            Targeting.Instance.WeighTargetsFilter += WeighTargetsFilter;
            Targeting.Instance.RemoveTargetsFilter += RemoveTargetsFilter;

            ScriptHelpers.EventInProcess = false;

            // Ajouter les avoids
            Bots.DungeonBuddy.Avoidance.AvoidanceManager.AddRange(_avoidInfos.Where(a => !Bots.DungeonBuddy.Avoidance.AvoidanceManager.AvoidInfos.Contains(a)));
            
            OnEnter();
        }

        /// <summary>
        /// Appelé quand le joueur quitte le donjon
        /// </summary>
        internal void Detach()
        {
            Targeting.Instance.IncludeTargetsFilter -= IncludeTargetsFilter;
            Targeting.Instance.WeighTargetsFilter -= WeighTargetsFilter;
            Targeting.Instance.RemoveTargetsFilter -= RemoveTargetsFilter;
            
            // Retirer les avoids
            Bots.DungeonBuddy.Avoidance.AvoidanceManager.RemoveAll(a => _avoidInfos.Contains(a));

            Bots.DungeonBuddy.Helpers.ScriptHelpers.RestoreMovement();

            OnExit();
        }

        /// <summary>
        /// Override pour logique d'entrée custom
        /// </summary>
        public virtual void OnEnter()
        {
        }

        /// <summary>
        /// Override pour logique de sortie custom
        /// </summary>
        public virtual void OnExit()
        {
        }

        // ═══════════════════════════════════════════════════════════
        // AVOIDANCE HELPERS
        // ═══════════════════════════════════════════════════════════

        protected internal void AddAvoid(AvoidInfo avoidInfo)
        {
            Bots.DungeonBuddy.Avoidance.AvoidanceManager.Add(avoidInfo);
            _avoidInfos.Add(avoidInfo);
        }

        protected void AddAvoidRange(IEnumerable<AvoidInfo> avoidInfos)
        {
            _avoidInfos.AddRange(avoidInfos);
        }

        protected void RemoveAvoid(AvoidInfo avoidInfo)
        {
            _avoidInfos.Remove(avoidInfo);
        }

        protected void ClearAvoids()
        {
            Bots.DungeonBuddy.Avoidance.AvoidanceManager.RemoveAll(a => _avoidInfos.Contains(a));
            _avoidInfos.Clear();
        }

        public virtual MoveResult MoveTo(WoWPoint location)
        {
            return MoveResult.Failed;
        }

        // ═══════════════════════════════════════════════════════════
        // IDISPOSABLE
        // ═══════════════════════════════════════════════════════════

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        ~Dungeon()
        {
            Dispose();
        }
    }
}