#nullable disable
using System.Collections.Generic;
using System.Linq;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic.Inventory
{
    /// <summary>
    /// Helper class for finding consumable items (food and drink).
    /// </summary>
    public static class Consumable
    {
        /// <summary>
        /// Gets all food items in the player's bags.
        /// </summary>
        public static List<WoWItem> GetFood()
        {
            return ObjectManager.Me.BagItems
                .Where(IsFood)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Gets all drink items in the player's bags.
        /// </summary>
        public static List<WoWItem> GetDrinks()
        {
            return ObjectManager.Me.BagItems
                .Where(IsDrink)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Gets the best food item.
        /// Picks the highest RequiredLevel consumable the player can use; on tie, the largest stack.
        /// HB 4.3.4 Consumable.GetBestFood uses `StackCount > num && RequiredLevel > num2` (both strict)
        /// which is logically broken — a single item cannot have BOTH more stack AND higher required
        /// level than itself, so the loop only ever accepts the very first item that passes against
        /// the initial `num=0, num2=-1` and never upgrades to a better one with the same stack count.
        /// We port the same intent (pick the best consumable the player can use) but with correct
        /// ordering: highest RequiredLevel first, largest StackCount on tie.
        /// </summary>
        /// <param name="includeSpecialtyItems">Include items with special effects.</param>
        public static WoWItem GetBestFood(bool includeSpecialtyItems)
        {
            var food = GetFood();
            if (food.Count == 0)
                return null;

            WoWItem bestItem = null;
            int playerLevel = StyxWoW.Me.Level;

            foreach (var item in food)
            {
                if (item.ItemInfo.RequiredLevel > playerLevel)
                    continue;
                if (!includeSpecialtyItems && !item.ItemSpells.All(IsBasicFoodOrDrink))
                    continue;

                if (bestItem == null ||
                    item.ItemInfo.RequiredLevel > bestItem.ItemInfo.RequiredLevel ||
                    (item.ItemInfo.RequiredLevel == bestItem.ItemInfo.RequiredLevel &&
                     item.StackCount > bestItem.StackCount))
                {
                    bestItem = item;
                }
            }

            return bestItem;
        }

        /// <summary>
        /// Gets the best drink item.
        /// Same fix as GetBestFood: HB 4.3.4 has the same broken `> && >` strict-greater condition.
        /// We use highest RequiredLevel with largest StackCount tie-break.
        /// </summary>
        /// <param name="includeSpecialtyItems">Include items with special effects.</param>
        public static WoWItem GetBestDrink(bool includeSpecialtyItems)
        {
            var drinks = GetDrinks();
            if (drinks.Count == 0)
                return null;

            WoWItem bestItem = null;
            int playerLevel = StyxWoW.Me.Level;

            foreach (var item in drinks)
            {
                if (item.ItemInfo.RequiredLevel > playerLevel)
                    continue;
                if (!includeSpecialtyItems && !item.ItemSpells.All(IsBasicFoodOrDrink))
                    continue;

                if (bestItem == null ||
                    item.ItemInfo.RequiredLevel > bestItem.ItemInfo.RequiredLevel ||
                    (item.ItemInfo.RequiredLevel == bestItem.ItemInfo.RequiredLevel &&
                     item.StackCount > bestItem.StackCount))
                {
                    bestItem = item;
                }
            }

            return bestItem;
        }

        /// <summary>
        /// Checks if an item is food.
        /// HB 4.3.4 Consumable.smethod_1: spell name "Food" OR "Refreshment".
        /// "Refreshment" covers WotLK Mage conjured food (Conjured Mana Strudel, etc.).
        /// </summary>
        private static bool IsFood(WoWItem item)
        {
            if ((int)item.ItemInfo.ItemClass != (int)WoWItemClass.Consumable)
                return false;

            return item.ItemSpells.Any(s =>
                s.ActualSpell != null &&
                (s.ActualSpell.Name == "Food" || s.ActualSpell.Name == "Refreshment"));
        }

        /// <summary>
        /// Checks if an item is a drink.
        /// HB 4.3.4 Consumable.smethod_3: spell name "Drink", "Starfire Espresso" or "Refreshment".
        /// "Refreshment" covers WotLK Mage conjured water; "Starfire Espresso" is a buff food.
        /// </summary>
        private static bool IsDrink(WoWItem item)
        {
            if ((int)item.ItemInfo.ItemClass != (int)WoWItemClass.Consumable)
                return false;

            return item.ItemSpells.Any(s =>
                s.ActualSpell != null &&
                (s.ActualSpell.Name == "Drink" ||
                 s.ActualSpell.Name == "Starfire Espresso" ||
                 s.ActualSpell.Name == "Refreshment"));
        }

        /// <summary>
        /// Checks if a spell is basic food or drink (no special effects).
        /// </summary>
        private static bool IsBasicFoodOrDrink(WoWItem.WoWItemSpell spell)
        {
            return spell.ActualSpell.Name == "Food" || spell.ActualSpell.Name == "Drink";
        }
    }
}
