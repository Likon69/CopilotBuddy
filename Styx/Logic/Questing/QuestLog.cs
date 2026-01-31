#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using GreenMagic;
using Styx.WoWInternals;

namespace Styx.Logic.Questing
{
	/// <summary>
	/// Provides access to the player's quest log.
	/// Matches HB 4.3.4 API while using Lua for completed quests (more reliable than memory reads).
	/// </summary>
	public class QuestLog
	{
		// WoW 3.3.5a Quest Log Offsets
		private const int OFFSET_COMPLETED_QUEST_LIST = 5005;  // Completed quest linked list head

		private static readonly List<uint> _completedQuestIds = new List<uint>();
		private static DateTime _completedQuestCacheTime = DateTime.MinValue;
		private static readonly TimeSpan CompletedQuestCacheDuration = TimeSpan.FromMinutes(1);

		/// <summary>
		/// Number of quests in the log.
		/// Address: 12729040 (0xC24350)
		/// </summary>
		public uint QuestCount
		{
			get
			{
				return ObjectManager.Wow.Read<uint>(12729040U);
			}
		}

		/// <summary>
		/// Gets the quest log index for a quest ID.
		/// </summary>
		public int GetIndexForQuest(uint questId)
		{
			for (int i = 0; i < 25; i++)
			{
				uint offset = (uint)(158 + i * 5);
				uint id = ObjectManager.Me.ReadDescriptor<uint>(offset);
				if (id == questId)
					return i;
			}
			return -1;
		}

		/// <summary>
		/// Gets all quests in the log.
		/// </summary>
		public List<PlayerQuest> GetAllQuests()
		{
			List<PlayerQuest> list = new List<PlayerQuest>();
			for (byte i = 0; i < 25; i++)
			{
				PlayerQuest quest = GetQuest((uint)i);
				if (quest != null)
					list.Add(quest);
			}
			return list;
		}

		/// <summary>
		/// Gets a quest by log index.
		/// </summary>
		public PlayerQuest GetQuest(uint index)
		{
			if (index >= 25U)
				throw new ArgumentOutOfRangeException("index");

			uint questId = GetQuestIdAtIndex(index);
			if (questId <= 0U)
				return null;

			return PlayerQuest.FromId(questId);
		}

		/// <summary>
		/// Gets the quest ID at a log index.
		/// </summary>
		public uint GetQuestId(uint index)
		{
			if (index >= 25U)
				throw new ArgumentOutOfRangeException("index");

			return GetQuestIdAtIndex(index);
		}

		private uint GetQuestIdAtIndex(uint index)
		{
			uint offset = 158U + index * 5U;
			return ObjectManager.Me.ReadDescriptor<uint>(offset);
		}

		/// <summary>
		/// Checks if a quest is in the log.
		/// </summary>
		public bool ContainsQuest(uint questId)
		{
			for (int i = 0; i < 25; i++)
			{
				uint offset = (uint)(158 + i * 5);
				uint id = ObjectManager.Me.ReadDescriptor<uint>(offset);
				if (id == questId)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a quest by ID.
		/// </summary>
		public PlayerQuest GetQuestById(uint questId)
		{
			if (!ContainsQuest(questId))
				return null;
			return PlayerQuest.FromId(questId);
		}

		/// <summary>
		/// Gets quest info at a log index.
		/// </summary>
		public QuestLogEntry GetQuestInfo(int index)
		{
			uint offset = (uint)(158 + index * 5 * 4);
			uint descriptorPtr = ObjectManager.Wow.Read<uint>(StyxWoW.Me.BaseAddress + 8U);
			return ObjectManager.Wow.ReadStruct<QuestLogEntry>(descriptorPtr + offset);
		}

		/// <summary>
		/// Abandons a quest by ID.
		/// Address: 12729052 (0xC2435C), Call: 6163648 (0x5E1CC0)
		/// </summary>
		public void AbandonQuestById(uint questId)
		{
			ExecutorRand executor = ObjectManager.Executor;
			if (executor == null)
				throw new Exception("Invalid executor used in AbandonQuestById.");

			lock (executor.AssemblyLock)
			{
				executor.Clear();
				executor.AddLine("mov esi, {0}", 12729052U);
				executor.AddLine("mov ebx, [esi]");
				executor.AddLine("mov eax, {0}", questId);
				executor.AddLine("mov [esi], eax");
				executor.AddLine("call {0}", 6163648U);
				executor.AddLine("mov [esi], ebx");
				executor.AddLine("retn");
				executor.Execute();
			}
		}

		/// <summary>
		/// Abandons a quest by log slot.
		/// </summary>
		public void AbandonQuest(byte slot)
		{
			if (slot >= 25)
				throw new ArgumentOutOfRangeException("slot");

			RemoveQuestFromLog(slot);
		}

		/// <summary>
		/// Removes a quest from the log.
		/// Call: ClntObjMgrGetActivePlayerObj (4208880), CGPlayer_C__QuestLogRemoveQuest (7163776)
		/// </summary>
		private static void RemoveQuestFromLog(byte slot)
		{
			ExecutorRand executor = ObjectManager.Executor;
			if (executor == null)
				throw new Exception("Invalid executor used in CGPlayer_C__QuestLogRemoveQuest.");

			lock (executor.AssemblyLock)
			{
				executor.Clear();
				executor.AddLine("call {0}", 4208880U);
				executor.AddLine("mov ecx, eax");
				executor.AddLine("push {0}", slot);
				executor.AddLine("call {0}", 7163776U);
				executor.AddLine("retn");
				executor.Execute();
			}
		}

		/// <summary>
		/// Gets a read-only collection of quest IDs that have been completed.
		/// Uses Lua QueryQuestsCompleted() which triggers QUEST_QUERY_COMPLETE event.
		/// Then reads the completed quest linked list from memory.
		/// Results are cached for 1 minute.
		/// </summary>
		public ReadOnlyCollection<uint> GetCompletedQuests()
		{
			if (ShouldRefreshCompletedQuestCache())
			{
				if (!TryRefreshCompletedQuestCache())
					return _completedQuestIds.AsReadOnly();
			}
			return _completedQuestIds.AsReadOnly();
		}

		/// <summary>
		/// Checks if the completed quest cache should be refreshed.
		/// </summary>
		private static bool ShouldRefreshCompletedQuestCache()
		{
			return _completedQuestIds.Count == 0 || DateTime.Now - _completedQuestCacheTime > CompletedQuestCacheDuration;
		}

		/// <summary>
		/// Refreshes the completed quest cache using Lua QueryQuestsCompleted().
		/// Waits for QUEST_QUERY_COMPLETE event, then reads memory.
		/// </summary>
		private static bool TryRefreshCompletedQuestCache()
		{
			try
			{
				using (LuaEventWait questQueryWait = new LuaEventWait("QUEST_QUERY_COMPLETE"))
				{
					Lua.DoString("QueryQuestsCompleted()");
					if (!questQueryWait.Wait(5000))
					{
						Styx.Helpers.Logging.Write("[QuestLog] Timeout waiting for QUEST_QUERY_COMPLETE event");
						return false;
					}
				}

				PopulateCompletedQuestCacheFromMemory();
				return true;
			}
			catch (Exception ex)
			{
				Styx.Helpers.Logging.WriteException(ex);
				return false;
			}
		}

		/// <summary>
		/// Reads completed quest IDs from the WoW memory linked list.
		/// Structure: CompletedQuestNode { padding, next_ptr, quest_id }
		/// </summary>
		private static void PopulateCompletedQuestCacheFromMemory()
		{
			Memory wow = ObjectManager.Wow;
			if (wow == null)
				return;

			uint completedQuestListHead = StyxWoW.Offsets.GetOffsetByIndex(OFFSET_COMPLETED_QUEST_LIST);
			if (completedQuestListHead == 0)
			{
				Styx.Helpers.Logging.Write("[QuestLog] COMPLETED_QUEST_LIST offset is 0");
				return;
			}

			// Read the head pointer of the linked list
			uint nodeAddress = wow.Read<uint>(completedQuestListHead);

			_completedQuestIds.Clear();

			// Traverse the linked list
			while (nodeAddress != 0 && (nodeAddress & 1U) == 0U)
			{
				var node = wow.Read<CompletedQuestNode>(nodeAddress);
				if (node.QuestId != 0)
					_completedQuestIds.Add(node.QuestId);
				nodeAddress = node.Next;
			}

			_completedQuestCacheTime = DateTime.Now;
			// Debug log removed - not in HB 4.3.4
		}

		/// <summary>
		/// Adds a quest ID to the completed quests cache.
		/// </summary>
		public void AddCompletedQuest(uint questId)
		{
			if (questId != 0 && !_completedQuestIds.Contains(questId))
				_completedQuestIds.Add(questId);
		}

		/// <summary>
		/// Adds a quest ID to the completed quests cache (HB 3.3.5a alias).
		/// </summary>
		public void AddCompletedQuestId(uint questId) => AddCompletedQuest(questId);

		/// <summary>
		/// Completed quest linked list node structure (WoW 3.3.5a).
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct CompletedQuestNode
		{
			private readonly uint _padding;
			public readonly uint Next;
			public readonly uint QuestId;
		}
	}
}
