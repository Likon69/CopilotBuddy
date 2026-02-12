using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GreenMagic;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic.Combat
{
	public static class SpellManager
	{
		#region Constants - Offsets 3.3.5a (12340)

		// Cooldown list base
		private const uint CooldownListBase = 0xD3EDB4;  // 13890996 decimal

		#endregion

		private static int _lastKnownSpellCount;
		private static readonly Dictionary<string, WoWSpell> _knownSpells = new Dictionary<string, WoWSpell>(StringComparer.OrdinalIgnoreCase);

		public static Dictionary<string, WoWSpell> KnownSpells => _knownSpells;

		// HB 4.3.4 compatibility aliases
		public static Dictionary<string, WoWSpell> Spells => _knownSpells;
		// BUG-09 fix: Return defensive copy so callers can't corrupt the internal dictionary
		public static Dictionary<string, WoWSpell> RawSpells => new Dictionary<string, WoWSpell>(_knownSpells, StringComparer.OrdinalIgnoreCase);

		public static int NumKnownSpells
		{
			get
			{
				LocalPlayer? me = StyxWoW.Me;
				return me?.KnownSpells.Count ?? 0;
			}
		}

		public static void Refresh()
		{
			Logging.WriteDebug("Refresh() called. LastCount={0}, CurrentCount={1}", _lastKnownSpellCount, NumKnownSpells);
			
			if (_lastKnownSpellCount == 0 || NumKnownSpells != _lastKnownSpellCount)
			{
				Logging.Write("Building spell book");
				_knownSpells.Clear();

				LocalPlayer? me = StyxWoW.Me;
				if (me == null)
				{
					Logging.WriteDebug("ERROR: LocalPlayer is null!");
					return;
				}

				var knownSpells = me.KnownSpells;
				Logging.WriteDebug("Found {0} spells from LocalPlayer.KnownSpells", knownSpells.Count);
				
				foreach (WoWSpell spell in knownSpells)
				{
					if (!_knownSpells.ContainsKey(spell.Name))
					{
						_knownSpells.Add(spell.Name, spell);
						Logging.Write("Adding {0}", spell.Name);
					}
				}

				_lastKnownSpellCount = NumKnownSpells;
				Logging.Write("Spell book built");
			}
		}

		public static bool CastBarVisible
		{
			get
			{
				try
				{
					LocalPlayer? me = StyxWoW.Me;
					return me != null && me.Casting > 0;
				}
				catch
				{
					return false;
				}
			}
		}

		public static bool GlobalCooldown
		{
			get
			{
				try
				{
					Memory? memory = ObjectManager.Wow;
					if (memory == null) return false;

					long frequency;
					long counter;
					QueryPerformanceFrequency(out frequency);
					QueryPerformanceCounter(out counter);
					long currentTime = counter * 1000L / frequency;

					uint cooldownPtr = memory.Read<uint>(CooldownListBase);

					while (cooldownPtr != 0U && (cooldownPtr & 1U) == 0U)
					{
						uint startTime = memory.Read<uint>(cooldownPtr + 16U);
						uint duration = memory.Read<uint>(cooldownPtr + 44U);

						if ((ulong)(startTime + duration) > (ulong)currentTime)
						{
							return true;
						}

						cooldownPtr = memory.Read<uint>(cooldownPtr + 4U);
					}

					return false;
				}
				catch
				{
					return false;
				}
			}
		}

		public static TimeSpan GlobalCooldownLeft
		{
			get
			{
				try
				{
					Memory? memory = ObjectManager.Wow;
					if (memory == null) return TimeSpan.Zero;

					long frequency;
					long counter;
					QueryPerformanceFrequency(out frequency);
					QueryPerformanceCounter(out counter);
					long currentTime = counter * 1000L / frequency;

					uint cooldownPtr = memory.Read<uint>(CooldownListBase);

					while (cooldownPtr != 0U && (cooldownPtr & 1U) == 0U)
					{
						uint startTime = memory.Read<uint>(cooldownPtr + 16U);
						uint duration = memory.Read<uint>(cooldownPtr + 44U);

						long endTime = startTime + duration;
						if (endTime > currentTime)
						{
							return TimeSpan.FromMilliseconds(endTime - currentTime);
						}

						cooldownPtr = memory.Read<uint>(cooldownPtr + 4U);
					}

					return TimeSpan.Zero;
				}
				catch
				{
					return TimeSpan.Zero;
				}
			}
		}

		public static bool HasSpell(string name)
		{
			return _knownSpells.ContainsKey(name);
		}

		public static bool HasSpell(int id)
		{
			foreach (var spell in _knownSpells.Values)
			{
				if (spell.Id == id)
					return true;
			}
			return false;
		}

		public static WoWSpell? GetSpellByName(string name)
		{
			if (_knownSpells.TryGetValue(name, out WoWSpell? spell))
			{
				return spell;
			}
			return null;
		}

		public static bool CanCastSpell(string name)
		{
			if (!HasSpell(name))
			{
				// Only log once per spell to avoid spam
				return false;
			}

			if (GlobalCooldown)
				return false;

			WoWSpell? spell = GetSpellByName(name);
			if (spell == null)
				return false;

			// Check if spell is on cooldown
			if (spell.Cooldown)
				return false;

			return true;
		}

		// HB 4.3.4 compatibility wrappers
		public static bool CanCast(string spellName) => CanCastSpell(spellName);

		public static bool CanCast(int spellId, WoWUnit target, bool checkRange = true, bool checkMovement = false)
		{
			// Convert spellId to name and use existing CanCast
			WoWSpell? spell = Spells.Values.FirstOrDefault(s => s.Id == spellId);
			if (spell == null)
				return false;
			return CanCast(spell.Name, target, checkRange, checkMovement);
		}

		/// <summary>
		/// HB 4.3.4 compatible CanCast with full validation.
		/// Checks: HasSpell, GCD, Cooldown, IsCasting, Movement (if checkMovement), Power/Mana via spell.CanCast
		/// </summary>
		public static bool CanCast(string spellName, WoWUnit target, bool checkRange = true, bool checkMovement = false)
		{
			// Step 1: Check if we know the spell
			if (!HasSpell(spellName))
				return false;

			WoWSpell? spell = GetSpellByName(spellName);
			if (spell == null)
				return false;

			// Step 2: Check if we're currently casting (can't cast while casting)
			LocalPlayer? me = StyxWoW.Me;
			if (me == null)
				return false;

			if (me.IsCasting)
				return false;

			// Step 3: Check Global Cooldown (BUG-19 fix: allow pre-cast within lag tolerance)
			// SUS-06 fix: use dynamic latency instead of hardcoded 150ms
			if (GlobalCooldownLeft.TotalMilliseconds > StyxWoW.WoWClient.Latency * 2)
				return false;

			// Step 4: Check spell-specific cooldown
			if (spell.Cooldown)
				return false;

			// Step 5: Check if spell is already queued (for "on next swing" abilities like Heroic Strike, Cleave)
			// IsCurrentSpell returns true if the spell is already active/queued
			if (IsCurrentSpell(spellName))
				return false;

			// Step 6: Check movement restrictions (cast time spells can't be cast while moving)
			if (checkMovement && spell.CastTime > 0 && me.IsMoving)
				return false;

			// Step 7: Check if spell is usable (mana/power check via WoW's IsUsableSpell)
			// This is the most reliable check as it uses the game's own validation
			if (!spell.CanCast)
				return false;

			// Step 8: Target validation (if target required)
			if (target != null && !target.IsValid)
				return false;

			return true;
		}

		/// <summary>
		/// Checks if a spell is currently active or queued (for "on next swing" abilities).
		/// Uses WoW's Lua IsCurrentSpell API.
		/// </summary>
		private static bool IsCurrentSpell(string spellName)
		{
			try
			{
				// IsCurrentSpell returns 1 if the spell is currently active/queued (e.g., Heroic Strike waiting for next swing)
				var result = Lua.GetReturnVal<int>($"return IsCurrentSpell(\"{spellName}\") and 1 or 0", 0);
				return result == 1;
			}
			catch
			{
				return false;
			}
		}

		public static bool CanCast(WoWSpell spell, WoWUnit target, bool checkRange = true, bool checkMovement = false)
		{
			if (spell == null)
				return false;
			return CanCast(spell.Name, target, checkRange, checkMovement);
		}

		public static bool Cast(string spellName) => CastSpell(spellName);

		/// <summary>
		/// BUG-18 fix: Cast spell on a specific target using GUID-based casting.
		/// No longer uses target-first approach which could lose the target between calls.
		/// </summary>
		public static bool Cast(string spellName, WoWUnit target)
		{
			if (target == null)
				return Cast(spellName);
			
			WoWSpell? spell = GetSpellByName(spellName);
			if (spell == null)
			{
				Logging.WriteDebug("[SpellManager] Cast: spell '{0}' not found", spellName);
				return false;
			}
			
			// Use GUID-based casting — atomic, no target swap needed
			CastSpellById(spell.Id, target.Guid);
			return true;
		}

		public static bool Cast(WoWSpell spell)
		{
			if (spell == null)
				return false;
			return CastSpell(spell.Name);
		}

		/// <summary>
		/// Cast a spell (by WoWSpell) on a specific target using GUID-based casting.
		/// FEAT-06: Merged from SpellManagerEx.
		/// </summary>
		public static bool Cast(WoWSpell spell, WoWUnit target)
		{
			if (spell == null)
				return false;
			CastSpellById(spell.Id, target?.Guid ?? 0UL);
			return true;
		}

		/// <summary>Cast a spell by ID on the current target.</summary>
		public static bool Cast(int spellId) => Cast(spellId, StyxWoW.Me?.CurrentTarget);

		/// <summary>Cast a spell by ID on a specific target using GUID-based casting.</summary>
		public static bool Cast(int spellId, WoWUnit target)
		{
			WoWSpell? spell = _knownSpells.Values.FirstOrDefault(s => s.Id == spellId);
			if (spell == null)
				return false;
			return Cast(spell, target);
		}

		#region FEAT-06: CanBuff / Buff / CastRandom / BuffRandom (merged from SpellManagerEx)

		private static readonly Random _spellRandom = new Random(Environment.TickCount);

		/// <summary>Check if we can cast a buff (CanCast + target doesn't already have aura).</summary>
		public static bool CanBuff(string spellName, WoWUnit target = null, bool checkRange = false)
		{
			target ??= StyxWoW.Me;
			if (!CanCast(spellName, target, checkRange))
				return false;
			return !target.HasAura(spellName);
		}

		/// <summary>Check if we can cast a buff by spell ID.</summary>
		public static bool CanBuff(int spellId, WoWUnit target = null, bool checkRange = false)
		{
			WoWSpell? spell = _knownSpells.Values.FirstOrDefault(s => s.Id == spellId);
			if (spell == null) return false;
			return CanBuff(spell.Name, target, checkRange);
		}

		/// <summary>Check if we can cast a buff (WoWSpell overload).</summary>
		public static bool CanBuff(WoWSpell spell, WoWUnit target = null, bool checkRange = false)
		{
			if (spell == null) return false;
			return CanBuff(spell.Name, target, checkRange);
		}

		/// <summary>Cast a buff on a target (cast only if target doesn't already have the aura).</summary>
		public static bool Buff(string spellName, WoWUnit target = null)
		{
			target ??= StyxWoW.Me;
			WoWSpell? spell = GetSpellByName(spellName);
			if (spell == null) return false;
			return Cast(spell, target);
		}

		/// <summary>Cast a buff by spell ID.</summary>
		public static bool Buff(int spellId, WoWUnit target = null)
		{
			target ??= StyxWoW.Me;
			WoWSpell? spell = _knownSpells.Values.FirstOrDefault(s => s.Id == spellId);
			if (spell == null) return false;
			return Cast(spell, target);
		}

		/// <summary>Cast a buff (WoWSpell overload).</summary>
		public static bool Buff(WoWSpell spell, WoWUnit target = null)
		{
			if (spell == null) return false;
			return Cast(spell, target ?? StyxWoW.Me);
		}

		/// <summary>Cast a random castable spell from the list on a target.</summary>
		public static bool CastRandom(IEnumerable<string> spellNames, WoWUnit target = null, bool checkRange = false)
		{
			target ??= StyxWoW.Me?.CurrentTarget;
			var spells = spellNames.Select(n => GetSpellByName(n)).Where(s => s != null).ToList();
			return CastRandom(spells, target, checkRange);
		}

		/// <summary>Cast a random castable spell from the list on a target.</summary>
		public static bool CastRandom(IEnumerable<WoWSpell> spellList, WoWUnit target = null, bool checkRange = false)
		{
			target ??= StyxWoW.Me?.CurrentTarget;
			var list = spellList.Where(s => s != null).ToList();
			while (list.Count > 0)
			{
				int idx = _spellRandom.Next(0, list.Count);
				if (CanCast(list[idx], target, checkRange))
				{
					Cast(list[idx], target);
					return true;
				}
				list.RemoveAt(idx);
			}
			return false;
		}

		/// <summary>Buff a random castable spell from the list on a target (skips if aura present).</summary>
		public static bool BuffRandom(IEnumerable<string> spellNames, WoWUnit target = null, bool checkRange = false)
		{
			target ??= StyxWoW.Me;
			var spells = spellNames.Select(n => GetSpellByName(n)).Where(s => s != null).ToList();
			return BuffRandom(spells, target, checkRange);
		}

		/// <summary>Buff a random castable spell from the list on a target (skips if aura present).</summary>
		public static bool BuffRandom(IEnumerable<WoWSpell> spellList, WoWUnit target = null, bool checkRange = false)
		{
			target ??= StyxWoW.Me;
			var list = spellList.Where(s => s != null).ToList();
			while (list.Count > 0)
			{
				int idx = _spellRandom.Next(0, list.Count);
				if (CanBuff(list[idx], target, checkRange))
				{
					Buff(list[idx], target);
					return true;
				}
				list.RemoveAt(idx);
			}
			return false;
		}

		#endregion

		public static void CastSpellById(uint spellId)
		{
			CastSpellById((int)spellId);
		}

		public static void CastSpellById(int spellId)
		{
			CastSpellById(spellId, 0UL);
		}

		/// <summary>
		/// Casts a spell by ID on a specific target via native Spell_C::CastSpell.
		/// HB 4.3.4: LegacySpellManager.smethod_1(spellId, 0, targetGuid, 0)
		/// Pushes 8 args onto stack (cdecl, 0x20 cleanup).
		/// </summary>
		public static void CastSpellById(int spellId, ulong targetGuid)
		{
			StyxWoW.ResetAfk();

			ExecutorRand? executor = ObjectManager.Executor;
			if (executor == null)
			{
				Logging.WriteDebug("[SpellManager] Invalid executor for CastSpellById");
				return;
			}

			// Split 64-bit GUID into two 32-bit halves (HB 4.3.4: Struct72.smethod_4)
			uint guidLow = (uint)(targetGuid & 0xFFFFFFFF);
			uint guidHigh = (uint)(targetGuid >> 32);

			Logging.WriteDebug("Spell_C::CastSpell({0}, 0, 0x{1:X}, 0)", spellId, targetGuid);

			try
			{
				lock (executor.AssemblyLock)
				{
					executor.Clear();
					// HB 4.3.4 exact push order (8 args, right-to-left):
					executor.AddLine("push 0");                // arg8: unk3
					executor.AddLine("push 0");                // arg7: unk2
					executor.AddLine("push 0");                // arg6: unk1
					executor.AddLine("push 0");                // arg5: targetFlags
					executor.AddLine("push {0}", guidHigh);    // arg4: GUID high 32 bits
					executor.AddLine("push {0}", guidLow);     // arg3: GUID low 32 bits
					executor.AddLine("push 0");                // arg2: itemIndex (0 = no item)
					executor.AddLine("push {0}", spellId);     // arg1: spellId
					executor.AddLine("call {0}", (uint)Patchables.GlobalOffsets.Spell_C__CastSpell);
					executor.AddLine("add esp, 0x20");         // cdecl cleanup: 8 * 4 = 32 = 0x20
					executor.AddLine("retn");
					executor.Execute();
				}
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
			}
		}

		public static void CastSpellById(int spellId, WoWUnit target)
		{
			CastSpellById(spellId, target.Guid);
		}

		public static bool CastSpell(string name)
		{
			return CastSpell(name, 0UL, true);
		}

		public static bool CastableSpell(WoWSpell spell)
		{
			try
			{
				if (spell.Id == 0)
					return false;

				LocalPlayer? me = StyxWoW.Me;
				if (me == null)
					return false;

				if (me.IsCasting)
					return false;

				if (!_knownSpells.ContainsKey(spell.Name))
					return false;

				// Check if spell is on cooldown
				if (spell.Cooldown)
					return false;

				// Check aura effects
				for (int i = 0; i < 3; i++)
				{
					var effect = spell.GetSpellEffect(i);
					if (effect == null) continue;

					WoWApplyAuraType auraType = effect.AuraType;
					if (auraType == WoWApplyAuraType.ModStat ||
						auraType == WoWApplyAuraType.ModStealth ||
						auraType == WoWApplyAuraType.ModSpeedAlways ||
						auraType == WoWApplyAuraType.ModSpeedFlight ||
						auraType == WoWApplyAuraType.PeriodicHeal ||
						auraType == WoWApplyAuraType.ModResistance ||
						auraType == WoWApplyAuraType.FeignDeath)
					{
						return true;
					}
				}

				// Check power type and cost
				WoWPowerType powerType = spell.PowerType;
				if (powerType == WoWPowerType.Runes)
					return true;

				int powerCost = spell.PowerCost;
				if (powerCost == 0)
					return true;

				// Check if moving (for cast time spells)
				if (spell.CastTime != 0U && me.IsMoving)
					return false;

				// Check available power
				if (me.GetCurrentPower(powerType) >= powerCost)
					return true;

				return false;
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
				return false;
			}
		}

		public static void StopCasting()
		{
			try
			{
				LocalPlayer? me = StyxWoW.Me;
				if (me != null && me.IsCasting)
				{
					Lua.DoString("SpellStopCasting()");
				}
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
			}
		}

		public static bool CastSpell(string name, WoWUnit target)
		{
			return CastSpell(name, target.Guid, true);
		}

		public static bool CastSpell(string name, ulong targetGuid, bool returnImmediately)
		{
			if (!CanCastSpell(name))
			{
				Logging.WriteDebug("[SpellManager] Cannot cast spell: {0}", name);
				return false;
			}

			WoWSpell? spell = GetSpellByName(name);
			if (spell == null)
				return false;

			CastSpellById(spell.Id, targetGuid);

			if (!returnImmediately)
			{
				// Wait for cast to complete
				System.Threading.Thread.Sleep((int)spell.CastTime);

				// Wait for GCD
				while (GlobalCooldown)
				{
					System.Threading.Thread.Sleep(10);
				}
			}

			Logging.WriteDebug("[SpellManager] Cast spell: {0}", name);
			return true;
		}

		// Struct for Spell_C__HandleTerrainClick
		[Flags]
		public enum MouseButton : uint
		{
			None = 0U,
			Left = 1U,
			Middle = 2U,
			Right = 4U,
			XButton1 = 8U,
			XButton2 = 16U
		}

		private enum MouseButtonByte : byte { Left = 0, Right = 1 }
		
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		private struct TerrainClickInfo
		{
			public WoWPoint Location;
			public ulong TargetGuid;
			public MouseButtonByte Button;
		}
		
		// Spell_C__HandleTerrainClick function address (WoW 3.3.5a)
		private const uint Spell_C__HandleTerrainClick = 0x80B740; // 8438592U

		public static bool ClickRemoteLocation(WoWPoint location)
		{
			StyxWoW.ResetAfk();
			
			ExecutorRand? executor = ObjectManager.Executor;
			if (executor == null)
			{
				Logging.WriteDebug("[SpellManager] Invalid executor for ClickRemoteLocation");
				return false;
			}

			var click = new TerrainClickInfo
			{
				Location = location,
				TargetGuid = 0UL,
				Button = MouseButtonByte.Left
			};

			try
			{
				lock (executor.AssemblyLock)
				{
					// Allocate memory for the click struct
					uint structPtr = executor.Memory.AllocateMemory(System.Runtime.InteropServices.Marshal.SizeOf(click));
					if (structPtr == 0U)
					{
						Logging.WriteDebug("[SpellManager] Could not allocate memory for ClickRemoteLocation");
						return false;
					}

					try
					{
						// Write struct to allocated memory
						executor.Memory.Write(structPtr, click);
						
						// Call Spell_C__HandleTerrainClick
						executor.Clear();
						executor.AddLine("push {0}", structPtr);
						executor.AddLine("call {0}", Spell_C__HandleTerrainClick);
						executor.AddLine("add esp, 4");  // Clean up stack (cdecl)
						executor.AddLine("retn");
						executor.Execute();
						
						int result = executor.Memory.Read<int>(executor.ReturnPointer);
						return result != 0;
					}
					finally
					{
						executor.Memory.FreeMemory(structPtr);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
				return false;
			}
		}

		#region Native Methods

		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(out long frequency);

		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceCounter(out long counter);

		#endregion
	}
}
