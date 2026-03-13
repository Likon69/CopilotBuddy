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

		// Cooldown linked list head — HB 3.3.5a decompile: address 13890996 = SpellCooldownPtr(0xD3F5AC) + 8.
		private const uint CooldownListBase = 0xD3F5B4;  // 13890996 decimal

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

				// Druid form-specific spell aliases (ported from HB 4.3.4 smethod_4).
				// In WotLK 3.3.5a, bear/cat variants share the same base name from the
				// client, so only one survives the ContainsKey check above. This second
				// pass adds form-qualified keys by spell ID so CRs that call e.g.
				// HasSpell("Swipe (Bear)") or CanCast("Mangle (Cat)") resolve correctly.
				foreach (WoWSpell spell in knownSpells)
				{
					switch (spell.Id)
					{
						case 779:   // Swipe (Bear)
							_knownSpells["Swipe (Bear)"] = spell;
							break;
						case 62078: // Swipe (Cat)
							_knownSpells["Swipe (Cat)"] = spell;
							break;
						case 33876: // Mangle (Cat)
							_knownSpells["Mangle (Cat)"] = spell;
							break;
						case 33878: // Mangle (Bear)
							_knownSpells["Mangle (Bear)"] = spell;
							break;
						case 16979: // Feral Charge (Bear)
							_knownSpells["Feral Charge (Bear)"] = spell;
							break;
						case 49376: // Feral Charge (Cat) — WotLK 3.0.2+
							_knownSpells["Feral Charge (Cat)"] = spell;
							break;
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
				// HB 4.3.4 reads the cooldown list directly without grabbing a frame.
				// Using AcquireFrame(true) injects code and can clash with other
				// executor calls; remove it to mimic the original behavior and
				// eliminate freezes.
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
				// Mirror HB 4.3.4: raw walk of cooldown list, no frame lock.
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

		/// <summary>
		/// Gets the remaining cooldown for a specific spell by walking the cooldown
		/// linked list via pure ReadProcessMemory. Zero Execute() overhead.
		/// Node layout (HB 4.3.4 Struct78): +0x04=Next, +0x08=SpellId,
		/// +0x10=StartTime, +0x14=SpellCooldown, +0x2C=GCDDuration.
		/// </summary>
		public static TimeSpan GetSpellCooldownTimeLeft(int spellId)
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

				uint nodePtr = memory.Read<uint>(CooldownListBase);

				while (nodePtr != 0U && (nodePtr & 1U) == 0U)
				{
					uint nodeSpellId = memory.Read<uint>(nodePtr + 0x08U);
					if (nodeSpellId == (uint)spellId)
					{
						uint startTime = memory.Read<uint>(nodePtr + 0x10U);
						uint spellCd = memory.Read<uint>(nodePtr + 0x14U);
						uint gcdDuration = memory.Read<uint>(nodePtr + 0x2CU);
						uint effectiveDuration = spellCd > gcdDuration ? spellCd : gcdDuration;
						long remaining = (long)(startTime + effectiveDuration) - currentTime;
						if (remaining > 0)
							return TimeSpan.FromMilliseconds(remaining);
						return TimeSpan.Zero;
					}
					nodePtr = memory.Read<uint>(nodePtr + 0x04U);
				}

				return TimeSpan.Zero; // not in list = not on cooldown
			}
			catch
			{
				return TimeSpan.Zero;
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

		public static bool HasSpell(WoWSpell spell)
		{
			return spell != null && HasSpell(spell.Id);
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
		public static bool CanCast(string spellName) => CanCast(spellName, false);

		/// <summary>
		/// HB 4.3.4 overload: CanCast(string, bool checkRange) — uses current target.
		/// </summary>
		public static bool CanCast(string spellName, bool checkRange)
			=> CanCast(spellName, StyxWoW.Me?.CurrentTarget!, checkRange);

		// HB convenience overload: allow calling CanCast with just an int spellId
		public static bool CanCast(int spellId)
		{
			return CanCast(spellId, StyxWoW.Me?.CurrentTarget);
		}

		/// <summary>
		/// HB 4.3.4 overload: CanCast(int, bool checkRange) — uses current target.
		/// </summary>
		public static bool CanCast(int spellId, bool checkRange)
			=> CanCast(spellId, StyxWoW.Me?.CurrentTarget!, checkRange);

		public static bool CanCast(int spellId, WoWUnit target, bool checkRange = true, bool checkMovement = false)
		{
			// Convert spellId to name and use existing CanCast
			WoWSpell? spell = Spells.Values.FirstOrDefault(s => s.Id == spellId);
			if (spell == null)
				return false;
			return CanCast(spell.Name, target, checkRange, checkMovement);
		}

		/// <summary>
		/// HB 4.3.4 SpellManager.cs line 166: CanCast with full validation.
		/// Ported exactly from HB 4.3.4 — uses spell.CooldownTimeLeft with lag
		/// tolerance, checks IsCasting, movement, range, and power.
		/// </summary>
		public static bool CanCast(string spellName, WoWUnit target, bool checkRange = true, bool checkMovement = false)
		{
			WoWSpell? spell = GetSpellByName(spellName);
			if (spell == null)
				return false;
			return CanCast(spell, target, checkRange, checkMovement);
		}

		/// <summary>
		/// HB 4.3.4 overload: CanCast(WoWSpell) — no target, no range check.
		/// </summary>
		public static bool CanCast(WoWSpell spell)
			=> CanCast(spell, false);

		/// <summary>
		/// HB 4.3.4 overload: CanCast(WoWSpell, bool checkRange) — uses current target.
		/// </summary>
		public static bool CanCast(WoWSpell spell, bool checkRange)
			=> CanCast(spell, StyxWoW.Me?.CurrentTarget!, checkRange);

		public static bool CanCast(WoWSpell spell, WoWUnit target, bool checkRange = true, bool checkMovement = false)
		{
			return CanCast(spell, target, checkRange, checkMovement, true);
		}

		/// <summary>
		/// HB 4.3.4 SpellManager.cs line 166-222: CanCast with accountForLagTolerance.
		/// Ported exactly from the decompiled source.
		/// </summary>
		public static bool CanCast(WoWSpell spell, WoWUnit target, bool checkRange, bool checkMovement, bool accountForLagTolerance)
		{
			if (spell == null)
				return false;

			if (!HasSpell(spell.Name))
				return false;

			LocalPlayer? me = StyxWoW.Me;
			if (me == null)
				return false;

			// HB 4.3.4: Range checks
			if (checkRange && target != null)
			{
				if (!target.InLineOfSpellSight)
					return false;
				if (spell.MaxRange != 0f && target.Distance > (double)spell.MaxRange)
					return false;
				if (spell.MaxRange == 0f && !target.IsWithinMeleeRange)
					return false;
				if (spell.MinRange != 0f && target.Distance < (double)spell.MinRange)
					return false;
			}

			// HB 4.3.4: Movement check (cast time or funnel spells can't be cast while moving)
			if (checkMovement && (spell.CastTime != 0U || spell.IsFunnel) && me.IsMoving)
				return false;

			// HB 4.3.4: Lag tolerance path
			if (accountForLagTolerance && me.ChanneledCastingSpellId == 0)
			{
				uint lag = StyxWoW.WoWClient.Latency * 2U;
				if (me.IsCasting && (me.CurrentCastTimeLeft.TotalMilliseconds > lag || spell.CooldownTimeLeft.TotalMilliseconds > lag))
					return false;
				else if (!me.IsCasting && spell.CooldownTimeLeft.TotalMilliseconds > lag)
					return false;
				else
					return spell.CanCast;
			}
			// HB 4.3.4: Non-lag-tolerance path
			else if (!me.IsCasting)
			{
				if (!spell.Cooldown)
					return spell.CanCast;
				return false;
			}
			else
			{
				// IsCasting = true
				return false;
			}
		}

		public static bool Cast(string spellName) => Cast(spellName, StyxWoW.Me?.CurrentTarget);

		/// <summary>
		/// HB 4.3.4 line 308: Resolves spell by name, delegates to Cast(WoWSpell, WoWUnit).
		/// No CanCast guard — callers check CanCast separately before calling Cast.
		/// </summary>
		public static bool Cast(string spellName, WoWUnit target)
		{
			WoWSpell? spell = GetSpellByName(spellName);
			if (spell == null)
				return false;
			return Cast(spell, target);
		}

		public static bool Cast(WoWSpell spell)
		{
			if (spell == null)
				return false;
			return Cast(spell, StyxWoW.Me?.CurrentTarget);
		}

		/// <summary>
		/// HB 4.3.4 line 333: Cast spell on target via GUID-based CastSpellById.
		/// No CanCast guard — callers check CanCast separately before calling Cast.
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
				StyxWoW.Sleep((int)spell.CastTime);

				// Wait for GCD
				while (GlobalCooldown)
				{
					StyxWoW.Sleep(10);
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
						
						int result;
						using (StyxWoW.Memory.TemporaryCacheState(false))
						{
							result = executor.Memory.Read<int>(executor.ReturnPointer);
						}
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

		#region LuaEvent Auto-Refresh (ported from HB 4.3.4 smethod_0/1/2/3)

		/// <summary>
		/// HB 4.3.4 smethod_1: Called once during engine initialization.
		/// Hooks BotEvents.OnBotStarted so each bot run re-subscribes the Lua
		/// events and forces a spellbook rebuild.
		/// </summary>
		internal static void Initialize()
		{
			BotEvents.OnBotStarted += OnBotStarted_RefreshSpells;
			_knownSpells.Clear();
			_lastKnownSpellCount = 0;
			Refresh();
			Logging.WriteDebug("[SpellManager] Initialize \u2014 subscribed to BotEvents.OnBotStarted");
		}

		/// <summary>
		/// HB 4.3.4 smethod_2: Called during engine teardown.
		/// Unhooks BotEvents.OnBotStarted and clears the spellbook.
		/// </summary>
		internal static void Shutdown()
		{
			_knownSpells.Clear();
			_lastKnownSpellCount = 0;
			BotEvents.OnBotStarted -= OnBotStarted_RefreshSpells;
			Logging.WriteDebug("[SpellManager] Shutdown \u2014 unsubscribed from BotEvents.OnBotStarted");
		}

		/// <summary>
		/// HB 4.3.4 smethod_0: OnBotStarted handler. Rebuilds the spellbook, then
		/// detach+reattach the two Lua events (idempotent pattern from HB 4.3.4).
		/// </summary>
		private static void OnBotStarted_RefreshSpells(EventArgs args)
		{
			_lastKnownSpellCount = 0;
			Refresh();

			// Idempotent: detach first to avoid duplicate subscriptions (HB 4.3.4 pattern)
			Lua.Events.DetachEvent("LEARNED_SPELL_IN_TAB", new LuaEventHandlerDelegate(OnSpellBookChanged));
			Lua.Events.DetachEvent("ACTIVE_TALENT_GROUP_CHANGED", new LuaEventHandlerDelegate(OnSpellBookChanged));
			Lua.Events.AttachEvent("LEARNED_SPELL_IN_TAB", new LuaEventHandlerDelegate(OnSpellBookChanged));
			Lua.Events.AttachEvent("ACTIVE_TALENT_GROUP_CHANGED", new LuaEventHandlerDelegate(OnSpellBookChanged));

			Logging.WriteDebug("[SpellManager] Subscribed to LEARNED_SPELL_IN_TAB, ACTIVE_TALENT_GROUP_CHANGED");
		}

		/// <summary>
		/// HB 4.3.4 smethod_3: Lua event handler. Forces a full spellbook rebuild
		/// when the player learns a new spell or switches talent spec.
		/// </summary>
		private static void OnSpellBookChanged(object sender, LuaEventArgs e)
		{
			Logging.Write("[SpellManager] Spellbook change detected ({0}) \u2014 rebuilding", e.EventName);
			_lastKnownSpellCount = 0;
			Refresh();
		}

		#endregion

		#region Native Methods

		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(out long frequency);

		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceCounter(out long counter);

		#endregion
	}
}
