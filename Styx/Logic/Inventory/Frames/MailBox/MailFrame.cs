using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic.Inventory.Frames.MailBox
{
	/// <summary>
	/// Represents the mail frame in WoW.
	/// </summary>
	public class MailFrame : Frame
	{
		public static readonly MailFrame Instance = new MailFrame();

		public MailFrame() : base("MailFrame")
		{
		}

		/// <summary>
		/// Gets the number of mail items in the inbox.
		/// </summary>
		public int MailCount
		{
			get
			{
				var result = Lua.GetReturnValues("return GetInboxNumItems()", "Economist.lua");
				return result != null && result.Count > 0 ? Convert.ToInt32(result[0]) : 0;
			}
		}

		/// <summary>
		/// Gets whether there is new mail.
		/// </summary>
		public bool HasNewMail
		{
			get
			{
				Lua.DoString("CheckInbox()");
				var result = Lua.GetReturnValues("return HasNewMail()", "Economist.lua");
				return result != null && result.Count > 0 && !string.IsNullOrEmpty(result[0])
					&& Convert.ToBoolean(Convert.ToInt32(result[0]));
			}
		}

		/// <summary>
		/// Deletes a mail item.
		/// </summary>
		public void DeleteMail(int mailIndex)
		{
			Lua.DoString($"DeleteInboxItem({mailIndex})", "Economist.lua");
		}

		/// <summary>
		/// Returns a mail item to sender.
		/// </summary>
		public void ReturnMail(int mailIndex)
		{
			Lua.DoString($"ReturnInboxItem({mailIndex})", "Economist.lua");
		}

		/// <summary>
		/// Gets all attachments from a mail item.
		/// </summary>
		public void GetMailAttachments(int mailIndex)
		{
			int mailCount = MailCount;
			var moneyResult = Lua.GetReturnValues("return GetMoney()", "Economist.lua");
			int currentMoney = (moneyResult == null || moneyResult.Count <= 0) ? 0 : int.Parse(moneyResult[0]);

			var headerResult = Lua.GetReturnValues($"return GetInboxHeaderInfo({mailIndex})", "Economist.lua");
			if (headerResult != null && headerResult.Count >= 8)
			{
				int mailMoney = 0;
				if (!string.IsNullOrEmpty(headerResult[4]))
					mailMoney = int.Parse(headerResult[4]);

				Lua.DoString($"AutoLootMailItem({mailIndex})", "Economist.lua");

				while (int.Parse(Lua.GetReturnValues("return GetMoney()", "Economist.lua")[0]) < currentMoney + mailMoney)
					StyxWoW.Sleep(250);

				while (MailCount >= mailCount)
					StyxWoW.Sleep(250);
			}
		}

		/// <summary>
		/// Opens all mail and collects attachments.
		/// </summary>
		public void OpenAllMail()
		{
			if (IsVisible)
			{
				int count = MailCount;
				for (int i = 0; i < count; i++)
					GetMailAttachments(1);
			}
		}

		/// <summary>
		/// Opens all mail from a specific sender.
		/// </summary>
		public void OpenAllMailFrom(string senderName)
		{
			if (IsVisible)
			{
				Lua.DoString("CheckInbox()");
				int count = MailCount;
				var mailIndicesToCollect = new List<int>(count);

				for (int i = 1; i <= count; i++)
				{
					var headerResult = Lua.GetReturnValues($"return GetInboxHeaderInfo({i})", "Economist.lua");
					if (headerResult != null && headerResult.Count >= 8)
					{
						string sender = headerResult[2];
						if (sender.ToLower().Contains(senderName.ToLower()))
							mailIndicesToCollect.Add(i);
					}
				}

				mailIndicesToCollect.Sort((a, b) => b.CompareTo(a));

				foreach (int idx in mailIndicesToCollect)
					GetMailAttachments(idx);
			}
		}

		/// <summary>
		/// Switches to the send mail tab.
		/// </summary>
		public void SwitchToSendMailTab()
		{
			Lua.DoString("MailFrameTab2:Click()");
		}

		/// <summary>
		/// Sends a mail with attachments.
		/// </summary>
		public void SendMail(string recipient, string subject, string body, int copper, params WoWItem[] attachmentItems)
		{
			if (!IsVisible)
				return;

			if (attachmentItems.Length > 12)
				throw new ArgumentException("You can not attach more than 12 items!", nameof(attachmentItems));

			Lua.DoString("ClearSendMail()");
			SwitchToSendMailTab();
			Lua.DoString($"SendMailNameEditBox:SetText(\"{recipient}\")", "_main.lua");
			Lua.DoString($"SetSendMailMoney({copper})", "_main.lua");

			if (attachmentItems.Length > 0)
			{
				for (int i = 0; i < attachmentItems.Length; i++)
				{
					attachmentItems[i].UseContainerItem();
					StyxWoW.Sleep(100);
				}

				if (SendMailItems.Length != attachmentItems.Length)
					Logging.Write("Something is messed up, we didn't attach all of our items.");
			}

			Lua.DoString($"SendMail(\"{recipient}\", \"{subject}\", \"{body}\")", "_main.lua");
		}

		/// <summary>
		/// Sends a mail with many attachments (splits into multiple mails if needed).
		/// </summary>
		public void SendMailWithManyAttachments(string recipient, int copper, params WoWItem[] attachments)
		{
			int numMails = (int)Math.Ceiling((float)attachments.Length / 12f);
			var itemList = attachments.ToList();

			for (int i = 0; i < numMails; i++)
			{
				var batch = itemList.Take(12).ToList();
				if (batch.Count > 0)
					itemList.RemoveRange(0, batch.Count);

				// Snapshot item count BEFORE send, with fresh memory read.
				// HB 3.3.5a: clear cache so we read the actual in-game bag state,
				// not the tick-cached snapshot from before entering the mail behavior.
				StyxWoW.Memory.ClearCache();
				var currentItems = ObjectManager.GetObjectsOfType<WoWItem>();
				int count = currentItems.Count;

				SendMail(recipient, batch.Count > 0 ? batch[0].Name : "items", "", copper, batch.ToArray());

				// Wait for WoW to remove the mailed items from the bag (server acknowledgment).
				// Must clear cache each iteration so we see the updated bag state.
				int startTick = Environment.TickCount;
				while (Environment.TickCount - startTick < 10000)
				{
					StyxWoW.Sleep(250);
					StyxWoW.Memory.ClearCache();
					currentItems = ObjectManager.GetObjectsOfType<WoWItem>();
					if (currentItems.Count < count)
						break; // items removed — mail confirmed
				}
			}
		}

		/// <summary>
		/// Hides the mail frame.
		/// </summary>
		public new void Hide()
		{
			Lua.DoString("CloseMail()");
		}

		/// <summary>
		/// Closes the mail frame.
		/// </summary>
		public void Close() => Hide();

		/// <summary>
		/// FEAT-33: Gets the GUIDs of items currently attached to the outgoing mail.
		/// Uses Lua to check ITEM_QUALITY_COLORS-based attachment slots (max 12 slots in WotLK).
		/// </summary>
		public ulong[] SendMailItemGuids
		{
			get
			{
				var guids = new List<ulong>();
				try
				{
					for (int i = 1; i <= 12; i++)
					{
						var results = Lua.GetReturnValues(
							$"local name, itemId, texture, count, quality = GetSendMailItem({i}); return itemId or 0");
						if (results != null && results.Count > 0)
						{
							int itemId = Lua.ParseLuaValue<int>(results[0]);
							if (itemId > 0)
							{
								// Find first matching item in bags with this entry
								foreach (var item in ObjectManager.GetObjectsOfType<WoWItem>())
								{
									if (item.Entry == (uint)itemId && item.IsValid)
									{
										guids.Add(item.Guid);
										break;
									}
								}
							}
						}
					}
				}
				catch { }
				return guids.ToArray();
			}
		}

		/// <summary>
		/// FEAT-33: Gets the WoWItem objects attached to the outgoing mail.
		/// </summary>
		public WoWItem[] SendMailItems
		{
			get
			{
				var items = new List<WoWItem>();
				foreach (ulong guid in SendMailItemGuids)
				{
					var item = ObjectManager.GetObjectByGuid<WoWItem>(guid);
					if (item != null)
						items.Add(item);
				}
				return items.ToArray();
			}
		}
	}
}
