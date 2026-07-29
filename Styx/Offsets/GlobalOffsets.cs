using System;

namespace Styx.Offsets
{
	/// <summary>
	/// Global memory offsets for WoW 3.3.5a (Build 12340).
	/// These are offsets from the WoW.exe base address.
	/// </summary>
	public static class GlobalOffsets
	{
		// ==================== Client Info ====================
		public const uint GameBuild = 12340;
		public const string GameVersion = "3.3.5a";

		// ==================== Object Manager ====================
		/// <summary>Pointer to the client connection structure.</summary>
		public const uint ClientConnection = 0x00C79CE0;

		/// <summary>Offset from ClientConnection to the object manager.</summary>
		public const uint ObjectManagerOffset = 0x2ED0;

		/// <summary>Offset from ObjectManager to first object pointer.</summary>
		public const uint FirstObjectOffset = 0xAC;

		/// <summary>Offset from ObjectManager to local player GUID.</summary>
		public const uint LocalGuidOffset = 0xC0;

		/// <summary>Offset within object to get next object in list.</summary>
		public const uint NextObjectOffset = 0x3C;

		// ==================== Object Structure ====================
		/// <summary>Offset to object type field.</summary>
		public const uint ObjectTypeOffset = 0x14;

		/// <summary>Offset to object GUID field.</summary>
		public const uint ObjectGuidOffset = 0x30;

		/// <summary>Offset to object descriptors pointer.</summary>
		public const uint DescriptorOffset = 0x8;

		/// <summary>Size of one descriptor field (4 bytes = uint).</summary>
		public const uint DescriptorMultiplier = 0x4;

		// ==================== Unit Structure ====================
		/// <summary>Offset to unit position X (from unit base).</summary>
		public const uint UnitPosXOffset = 0x798;

		/// <summary>Offset to unit position Y (from unit base).</summary>
		public const uint UnitPosYOffset = 0x79C;

		/// <summary>Offset to unit position Z (from unit base).</summary>
		public const uint UnitPosZOffset = 0x7A0;

		/// <summary>Offset to unit facing/rotation (from unit base).</summary>
		public const uint UnitRotationOffset = 0x7A8;

		// ==================== Movement ====================
		/// <summary>Offset to movement info structure.</summary>
		public const uint MovementInfoOffset = 0xD8;

		/// <summary>Offset to movement flags within movement info.</summary>
		public const uint MovementFlagsOffset = 0x44;

		/// <summary>Offset to transport GUID within movement info.</summary>
		public const uint TransportGuidOffset = 0x20;

		// ==================== Player Data ====================
		/// <summary>Pointer to local player name.</summary>
		public const uint PlayerName = 0x00C79D18;

		/// <summary>Pointer to name cache (for other units).</summary>
		public const uint NameCacheBase = 0x00C5D940;

		/// <summary>Offset within name cache entry to name string.</summary>
		public const uint NameCacheNameOffset = 0x21;

		/// <summary>Offset within name cache entry to next entry.</summary>
		public const uint NameCacheNextOffset = 0xC;

		// ==================== Camera ====================
		/// <summary>Pointer to world frame (camera parent).</summary>
		public const uint WorldFrame = 0x00B7436C;

		/// <summary>Offset from world frame to camera pointer.</summary>
		public const uint CameraOffset = 0x7E20;

		/// <summary>Offset to camera position within camera struct.</summary>
		public const uint CameraPosOffset = 0x8;

		/// <summary>Offset to camera matrix within camera struct.</summary>
		public const uint CameraMatrixOffset = 0x14;

		// ==================== Target & Focus ====================
		/// <summary>GUID of current target.</summary>
		public const uint TargetGuid = 0x00BD07B0;

		/// <summary>GUID of current focus.</summary>
		public const uint FocusGuid = 0x00BD07C0;

		/// <summary>GUID of last target.</summary>
		public const uint LastTargetGuid = 0x00BD07B8;

		/// <summary>GUID of mouseover target.</summary>
		public const uint MouseoverGuid = 0x00BD07A0;

		/// <summary>GUID of the active mover (usually player, vehicle when possessed).</summary>
		public const uint ActiveMoverGuid = 0x00BD07A8;

		// ==================== In-Game Checks ====================
		/// <summary>Non-zero when in game world.</summary>
		public const uint InGame = 0x00BD0792;

		/// <summary>Non-zero when loading screen is active.</summary>
		public const uint IsLoadingOrConnecting = 0x00B6AA38;

		// ==================== Corpse ====================
		/// <summary>X position of player's corpse.</summary>
		public const uint CorpsePositionX = 0x00BD0A58;

		/// <summary>Y position of player's corpse.</summary>
		public const uint CorpsePositionY = 0x00BD0A5C;

		/// <summary>Z position of player's corpse.</summary>
		public const uint CorpsePositionZ = 0x00BD0A60;

		// ==================== Zone & Map ====================
		/// <summary>Current continent/map ID.</summary>
		public const uint MapId = 0x00BD088C;

		/// <summary>Current zone ID.</summary>
		public const uint ZoneId = 0x00BD080C;

		/// <summary>Current area ID (subzone).</summary>
		public const uint AreaId = 0x00BD0810;

		// ==================== Lua ====================
		/// <summary>Lua state pointer.</summary>
		public const uint LuaState = 0x00D3F78C;

		/// <summary>Function to execute Lua.</summary>
		public const uint FrameScript_Execute = 0x00819210;

		/// <summary>Function to register Lua function.</summary>
		public const uint FrameScript_RegisterFunction = 0x004181B0;

		/// <summary>Function to get Lua return value.</summary>
		public const uint FrameScript_GetText = 0x00819D40;

		// ==================== Spells ====================
		/// <summary>Pointer to spellbook structure.</summary>
		public const uint SpellBook = 0x00BE5D88;

		/// <summary>Number of spells in spellbook.</summary>
		public const uint SpellCount = 0x00BE8D9C;

		/// <summary>Current casting spell ID.</summary>
		public const uint CastingSpellId = 0x00CEA888;

		/// <summary>Current channeling spell ID.</summary>
		public const uint ChannelingSpellId = 0x00CEA8A8;

		// ==================== Cooldowns ====================
		/// <summary>Spell_C cooldown pointer (SpellCooldownPtr). List head at +8.</summary>
		public const uint CooldownList = 0x00D3F5AC;

		// ==================== Chat ====================
		/// <summary>Last chat message received.</summary>
		public const uint LastChatMessage = 0x00B75A60;

		// ==================== Hardware Events ====================
		/// <summary>Last hardware action timestamp.</summary>
		public const uint LastHardwareAction = 0x00B499A4;

		// ==================== CTM (Click-To-Move) ====================
		/// <summary>CTM base address.</summary>
		public const uint CTMBase = 0x00CA11D8;

		/// <summary>CTM destination X offset from base.</summary>
		public const uint CTMDestX = 0x8C;

		/// <summary>CTM destination Y offset from base.</summary>
		public const uint CTMDestY = 0x90;

		/// <summary>CTM destination Z offset from base.</summary>
		public const uint CTMDestZ = 0x94;

		/// <summary>CTM action type offset from base.</summary>
		public const uint CTMAction = 0x1C;

		/// <summary>CTM GUID offset from base.</summary>
		public const uint CTMGuid = 0x20;

		// ==================== Battleground ====================
		/// <summary>Current battleground status.</summary>
		public const uint BattlegroundStatus = 0x00BEB9C0;

		/// <summary>Time in battleground.</summary>
		public const uint BattlegroundTime = 0x00BEB9F0;

		// ==================== VMT Indices (Virtual Method Table) ====================
		/// <summary>VMT index for GetPosition.</summary>
		public const uint VMT_GetPosition = 0xC;

		/// <summary>VMT index for GetFacing.</summary>
		public const uint VMT_GetFacing = 0x10;

		/// <summary>VMT index for Interact.</summary>
		public const uint VMT_Interact = 0xB0;

		/// <summary>VMT index for GetName.</summary>
		public const uint VMT_GetName = 0xD4;

		// ==================== World Intersection ====================
		/// <summary>Function: CGWorldFrame::Intersect - Native WoW line of sight check.</summary>
		/// <remarks>Used for TraceLine/InLineOfSpellSight checks through walls/terrain.</remarks>
		public const uint CGWorldFrame_Intersect = 0x0077F310;
	}
}
