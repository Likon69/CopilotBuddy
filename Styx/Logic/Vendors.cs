using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic.Inventory;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.MailBox;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.Logic.Inventory.Frames.Trainer;
using Styx.Logic.POI;
using Styx.Logic.Profiles;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic
{
	/// <summary>
	/// Provides functionality for interacting with vendors, trainers, and mailboxes.
	/// </summary>
	public static class Vendors
	{
		private static readonly TrainerFrame _trainerFrame;
		private static readonly MerchantFrame _merchantFrame;
		private static readonly MailFrame _mailFrame;
		private static readonly GossipFrame _gossipFrame;

		public static VendorItemsEventHandler? OnVendorItems;
		public static EventHandler? OnRepairItems;
		public static MailItemsEventHandler? OnMailItems;
		public static BuyItemsEventHandler? OnBuyItems;

		public static bool ForceTrainer { get; set; }
		public static bool ForceSell { get; set; }
		public static bool ForceRepair { get; set; }
		public static bool ForceMail { get; set; }
		public static bool ForceBuy { get; set; }
		public static bool NeedClassTraining { get; set; }

		/// <summary>
		/// Gets or sets whether repair functionality is disabled.
		/// </summary>
		public static bool RepairDisabled { get; set; }

		static Vendors()
		{
			_trainerFrame = new TrainerFrame();
			_merchantFrame = new MerchantFrame();
			_mailFrame = new MailFrame();
			_gossipFrame = new GossipFrame();
			BotEvents.Player.OnLevelUp += OnLevelUp;
			// HB 3.3.5a: NeedClassTraining is ONLY set on level up, not on startup
			// This prevents the bot from going to trainer when all spells are already learned
		}

		/// <summary>
		/// Gets the nearest flight master with taxi available.
		/// </summary>
		public static WoWUnit? NearestFlightMerchant
		{
			get
			{
				// leverage cached units for better performance
				return ObjectManager.CachedUnits
					.Where(u => u.IsFlightMaster && u.InteractType == WoWInteractType.TaxiPathAvailable)
					.OrderBy(u => u.Distance)
					.FirstOrDefault();
			}
		}

		private static void OnLevelUp(BotEvents.Player.LevelUpEventArgs args)
		{
			// Use CharacterSettings because it's bound to the UI checkbox
			if (!CharacterSettings.Instance.TrainNewSkills)
				return;

			// In 3.3.5a, new spells available at even levels
			if (args != null && args.NewLevel % 2 == 0)
			{
				NeedClassTraining = true;
				Logging.Write("New spells available at trainer (level {0})!", args.NewLevel);
			}
		}

		/// <summary>
		/// Trains all available skills at the current trainer.
		/// </summary>
		public static void TrainSkills()
		{
			_trainerFrame.BuyAll();
			NeedClassTraining = false;
			ForceTrainer = false;
			
			// Refresh SpellManager cache so newly learned spells are available
			Styx.Logic.Combat.SpellManager.Refresh();
		}

		/// <summary>
		/// Mails all items that should be mailed.
		/// </summary>
		public static void MailAllItems()
		{
			var items = new List<WoWItem>();
			items.AddRange(InventoryManager.GetItemsToMail());

			if (OnMailItems != null)
			{
				var args = new MailItemsEventArgs { AdditionalItems = new List<WoWItem>() };

				foreach (Delegate handler in OnMailItems.GetInvocationList())
				{
					try
					{
						handler.DynamicInvoke(args);
					}
					catch (Exception ex)
					{
						Logging.WriteException(ex);
						continue;
					}

					foreach (var item in args.AdditionalItems)
					{
						if (!items.Contains(item))
							items.Add(item);
					}
					args.AdditionalItems.Clear();
				}
			}

			_mailFrame.SendMailWithManyAttachments(LevelbotSettings.Instance.MailRecipient, 0, items.ToArray());
			ForceMail = false;
		}

		/// <summary>
		/// Repairs all items.
		/// </summary>
		public static void RepairAllItems()
		{
			OnRepairItems?.Invoke(null, EventArgs.Empty);
			_merchantFrame.RepairAllItems();
			ForceRepair = false;
		}

		/// <summary>
		/// Sells all items according to profile settings.
		/// </summary>
		public static void SellAllItems()
		{
			ItemQuality qualityMask = ItemQuality.None;
			Profile? currentProfile = ProfileManager.CurrentProfile;

			if (currentProfile == null)
				return;

			if (currentProfile.SellGrey)
				qualityMask |= ItemQuality.Poor;
			if (currentProfile.SellWhite)
				qualityMask |= ItemQuality.Common;
			if (currentProfile.SellGreen)
				qualityMask |= ItemQuality.Uncommon;
			if (currentProfile.SellBlue)
				qualityMask |= ItemQuality.Rare;
			if (currentProfile.SellPurple)
				qualityMask |= ItemQuality.Epic;

			var protectedNames = new List<string>();
			var protectedIds = new List<uint>();

			protectedNames.AddRange(ProtectedItemsManager.GetAllItemNames());
			protectedIds.AddRange(ProtectedItemsManager.GetAllItemIds());

			// Protect food and drink
			if (uint.TryParse(LevelbotSettings.Instance.FoodName, out uint foodId))
				protectedIds.Add(foodId);
			else
				protectedNames.Add(LevelbotSettings.Instance.FoodName.ToLower());

			if (uint.TryParse(LevelbotSettings.Instance.DrinkName, out uint drinkId))
				protectedIds.Add(drinkId);
			else
				protectedNames.Add(LevelbotSettings.Instance.DrinkName.ToLower());

			// Fire event for plugins to add exclusions
			if (OnVendorItems != null)
			{
				var args = new SellItemsEventArgs
				{
					NameExceptions = new List<string>(),
					IdExceptions = new List<uint>()
				};

				foreach (Delegate handler in OnVendorItems.GetInvocationList())
				{
					try
					{
						handler.DynamicInvoke(args);
					}
					catch (Exception ex)
					{
						Logging.WriteException(ex);
						args.NameExceptions.Clear();
						args.IdExceptions.Clear();
						continue;
					}

					foreach (string name in args.NameExceptions)
					{
						if (!protectedNames.Contains(name))
							protectedNames.Add(name);
					}

					foreach (uint id in args.IdExceptions)
					{
						if (!protectedIds.Contains(id))
							protectedIds.Add(id);
					}

					args.NameExceptions.Clear();
					args.IdExceptions.Clear();
				}

				protectedNames.AddRange(args.NameExceptions);
				protectedIds.AddRange(args.IdExceptions);
			}

			_merchantFrame.SellItemQualities(qualityMask, protectedNames, protectedIds);
			ForceSell = false;
		}

		/// <summary>
		/// Buys items from vendor based on OnBuyItems event handlers and food/drink settings.
		/// Ported from HB 4.3.4.
		/// </summary>
		public static void BuyItems()
		{
			var itemsToBuy = new Dictionary<uint, int>();

			// Handle OnBuyItems event
			if (OnBuyItems != null)
			{
				var args = new BuyItemsEventArgs();
				foreach (Delegate handler in OnBuyItems.GetInvocationList())
				{
					try
					{
						handler.DynamicInvoke(args);
						foreach (var kvp in args.BuyItemsIds)
						{
							if (!itemsToBuy.ContainsKey(kvp.Key))
								itemsToBuy.Add(kvp.Key, kvp.Value);
						}
					}
					catch (Exception ex)
					{
						Logging.WriteException(ex);
						args.BuyItemsIds.Clear();
					}
				}
			}

			if (itemsToBuy.Count > 0)
			{
				foreach (var merchantItem in _merchantFrame.GetAllMerchantItems())
				{
					if (itemsToBuy.ContainsKey(merchantItem.ItemId))
						_merchantFrame.BuyItem(merchantItem.Index, itemsToBuy[merchantItem.ItemId]);
				}
			}

			// Handle automatic food/drink buying based on settings (HB 4.3.4 logic)
			Vendor asVendor = BotPoi.Current.AsVendor;
			if (asVendor == null || (asVendor.Type != Vendor.VendorType.Food && asVendor.Type != Vendor.VendorType.Restock))
				return;

			bool usesMana = StyxWoW.Me.PowerType == WoWPowerType.Mana || StyxWoW.Me.Class == WoWClass.Druid;
			int bestDrinkIndex = _merchantFrame.GetBestDrinkFromVendor();
			int bestFoodIndex = _merchantFrame.GetBestFoodFromVendor();

			// If vendor doesn't sell food or drink, blacklist it
			if (bestDrinkIndex == -1 && bestFoodIndex == -1)
			{
				Logging.Write("Vendor does not sell food or water. Blacklisting it.");
				ProfileManager.CurrentProfile?.VendorManager?.Blacklist.Add(BotPoi.Current.AsVendor);
				BotPoi.Clear("Blacklisted Vendor");
				return;
			}

			// Buy drinks if needed
			if (Consumable.GetBestDrink(false) == null && bestDrinkIndex != -1 && usesMana && 
			    CharacterSettings.Instance.DrinkAmount > 0)
			{
				var drinkItem = _merchantFrame.GetMerchantItemByIndex(bestDrinkIndex);
				if (drinkItem != null)
				{
					Logging.Write("Buying {0}x {1}", CharacterSettings.Instance.DrinkAmount, drinkItem.Name);
					_merchantFrame.BuyItem(bestDrinkIndex, CharacterSettings.Instance.DrinkAmount);
				}
			}

			// Buy food if needed
			if (Consumable.GetBestFood(false) == null && bestFoodIndex != -1 &&
			    CharacterSettings.Instance.FoodAmount > 0)
			{
				var foodItem = _merchantFrame.GetMerchantItemByIndex(bestFoodIndex);
				if (foodItem != null)
				{
					Logging.Write("Buying {0}x {1}", CharacterSettings.Instance.FoodAmount, foodItem.Name);
					_merchantFrame.BuyItem(bestFoodIndex, CharacterSettings.Instance.FoodAmount);
				}
			}

			Thread.Sleep(2000);
			_merchantFrame.Close();
			ForceBuy = false;
			BotPoi.Clear("Restocked");
		}
	}
}



