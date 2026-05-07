using System;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

namespace Bots.DungeonBuddy.Avoidance
{
    /// <summary>
    /// Définit une zone à éviter (ability de boss, feu au sol, etc.)
    /// </summary>
    public class AvoidInfo
    {
        /// <summary>
        /// Créer un avoid basé sur un objet
        /// </summary>
        /// <param name="condition">Condition pour activer l'avoid</param>
        /// <param name="objectSelector">Sélecteur d'objets à éviter</param>
        /// <param name="radiusSelector">Sélecteur de rayon d'évitement</param>
        public AvoidInfo(
            CanRunDecoratorDelegate condition,
            Predicate<WoWObject> objectSelector,
            Func<float> radiusSelector)
            : this(condition, objectSelector, radiusSelector, null, 40f, true)
        {
        }

        public AvoidInfo(
            CanRunDecoratorDelegate condition,
            Predicate<WoWObject> objectSelector,
            Func<float> radiusSelector,
            bool isBlocking)
            : this(condition, objectSelector, radiusSelector, null, 40f, isBlocking)
        {
        }

        public AvoidInfo(
            CanRunDecoratorDelegate condition,
            Predicate<WoWObject> objectSelector,
            Func<float> radiusSelector,
            Func<WoWPoint> leashPointSelector,
            float leashRadius,
            bool isBlocking,
            AvoidancePriority priority = AvoidancePriority.Medium)
        {
            Condition = condition;
            ObjectSelector = objectSelector;
            LocationSelector = null;
            RadiusSelector = radiusSelector;
            LeashPointSelector = leashPointSelector;
            LeashRadius = leashRadius;
            IsBlocking = isBlocking;
            Priority = priority;
        }

        /// <summary>
        /// Créer un avoid basé sur une position fixe
        /// </summary>
        public AvoidInfo(
            CanRunDecoratorDelegate condition,
            Func<WoWPoint> locationSelector,
            Func<float> radiusSelector)
            : this(condition, locationSelector, radiusSelector, null, 40f, true)
        {
        }

        public AvoidInfo(
            CanRunDecoratorDelegate condition,
            Func<WoWPoint> locationSelector,
            Func<float> radiusSelector,
            Func<WoWPoint> leashPointSelector,
            float leashRadius,
            bool isBlocking,
            AvoidancePriority priority = AvoidancePriority.Medium)
        {
            Condition = condition;
            ObjectSelector = null;
            LocationSelector = locationSelector;
            RadiusSelector = radiusSelector;
            LeashPointSelector = leashPointSelector;
            LeashRadius = leashRadius;
            IsBlocking = isBlocking;
            Priority = priority;
        }

        /// <summary>
        /// Condition pour activer l'avoid
        /// </summary>
        public CanRunDecoratorDelegate Condition { get; private set; }
        
        /// <summary>
        /// Sélecteur d'objets (pour avoid dynamique)
        /// </summary>
        public Predicate<WoWObject> ObjectSelector { get; private set; }
        
        /// <summary>
        /// Sélecteur de position (pour avoid statique)
        /// </summary>
        public Func<WoWPoint> LocationSelector { get; private set; }
        
        /// <summary>
        /// Sélecteur de rayon
        /// </summary>
        public Func<float> RadiusSelector { get; private set; }
        
        /// <summary>
        /// Point d'ancrage (ne pas fuir plus loin que LeashRadius de ce point)
        /// </summary>
        public Func<WoWPoint> LeashPointSelector { get; private set; }
        
        /// <summary>
        /// Rayon maximum de fuite depuis le point d'ancrage
        /// </summary>
        public float LeashRadius { get; private set; }
        
        /// <summary>
        /// Si true, bloque la navigation à travers cette zone
        /// </summary>
        public bool IsBlocking { get; private set; }

        /// <summary>
        /// HB 6.2.3 AvoidancePriority — weights the escape-route score in
        /// GetBestLocationOutsideCluster. High-priority avoids dominate the
        /// scoring so the bot exits through them first.
        /// Default: Medium (10).
        /// </summary>
        public AvoidancePriority Priority { get; private set; } = AvoidancePriority.Medium;

        public bool CanRun(object ctx)
        {
            try
            {
                return Condition == null || Condition(ctx);
            }
            catch
            {
                return false;
            }
        }
    }
}