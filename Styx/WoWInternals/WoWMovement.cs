using System;
using System.Collections.Generic;
using System.Threading;
using GreenMagic;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;

namespace Styx.WoWInternals
{
	using Tripper.Tools.Math;
	using Vector3 = Tripper.Tools.Math.Vector3;
	using Matrix = Tripper.Tools.Math.Matrix;

	public static class WoWMovement
	{
		#region Constants - Offsets 3.3.5a (12340)

		// Click to move function address - FROM 335offsetsall.txt
		private const uint CTM_Function = 0x00727400;  // 7500800 decimal (CGPlayer_C__ClickToMove)
		// Stop movement function - FROM HB 3.3.5a GlobalOffsets.cs
		private const uint CTM_Stop_Function = 0x0072B3A0;  // 7517088 decimal (CGPlayer_C__ClickToMoveStop)
		// Click to move base address - FROM HB 3.3.5a (0xCA11D8)
		private const uint ClickToMove_Base = 0xCA11D8;  // 13243864 decimal
		// Active input control pointer
		private const uint ActiveInputControl_Ptr = 0xC24D54;  // 12732756 decimal
		// GetActivePlayerObject - FROM 335offsetsall.txt
		private const uint GetActivePlayerObject_Function = 0x004038F0;  // 4208880 decimal


		#endregion

		#region FEAT-01: Timed movement queue

		private class TimedMovementEntry
		{
			public MovementDirection Direction;
			public DateTime StopTime;
		}

		private static readonly List<TimedMovementEntry> _timedMovements = new List<TimedMovementEntry>();

		/// <summary>
		/// FEAT-01: Pulse processes timed movement entries, stopping directions whose timer has expired.
		/// Called from WoWPulsator on each tick.
		/// </summary>
		public static void Pulse()
		{
			if (_timedMovements.Count == 0)
				return;

			DateTime now = DateTime.Now;
			MovementDirection expired = MovementDirection.None;
			for (int i = _timedMovements.Count - 1; i >= 0; i--)
			{
				if (_timedMovements[i].StopTime <= now)
				{
					Logging.WriteDebug("Flushing timed movement. Direction: {0}", _timedMovements[i].Direction);
					expired |= _timedMovements[i].Direction;
					_timedMovements.RemoveAt(i);
				}
			}
			if (expired != MovementDirection.None)
			{
				MoveStop(expired);
			}
		}

		#endregion

		public static ClickToMoveInfoStruct ClickToMoveInfo
		{
			get
			{
				Memory? memory = ObjectManager.Wow;
				if (memory == null)
					return new ClickToMoveInfoStruct();
				try
				{
					return memory.ReadStruct<ClickToMoveInfoStruct>(ClickToMove_Base);
				}
				catch
				{
					return new ClickToMoveInfoStruct();
				}
			}
		}

		// Bug fix #11: Should check ConstantFace, not Face (HB 3.3.5a & 4.3.4)
		public static bool IsFacing => ClickToMoveInfo.Type == ClickToMoveType.ConstantFace;

		public static InputControl ActiveInputControl
		{
			get
			{
				Memory? memory = ObjectManager.Wow;
				if (memory == null)
					return new InputControl();
				try
				{
					uint controlPtr = memory.Read<uint>(ActiveInputControl_Ptr);
					if (controlPtr == 0)
						return new InputControl();
					return memory.ReadStruct<InputControl>(controlPtr);
				}
				catch
				{
					return new InputControl();
				}
			}
		}

		[Flags]
		public enum MovementDirection : uint
		{
			None = 0,
			RMouse = 1,
			LMouse = 2,
			Forward = 16,           // 0x00000010
			Backwards = 32,         // 0x00000020
			StrafeLeft = 64,        // 0x00000040
			StrafeRight = 128,      // 0x00000080
			TurnLeft = 256,         // 0x00000100
			TurnRight = 512,        // 0x00000200
			PitchUp = 1024,         // 0x00000400
			PitchDown = 2048,       // 0x00000800
			AutoRun = 4096,         // 0x00001000
			JumpAscend = 8192,      // 0x00002000
			Descend = 16384,        // 0x00004000
			ClickToMove = 4194304,  // 0x00400000
			IsCTMing = 2097152,     // 0x00200000
			ForwardBackMovement = 65536,  // 0x00010000
			StrafeMovement = 131072,      // 0x00020000
			TurnMovement = 262144,        // 0x00040000
			StrafeMask = StrafeMovement | StrafeRight | StrafeLeft,  // 0x000200C0
			TurnMask = TurnMovement | TurnRight | TurnLeft,          // 0x00040300
			MoveMask = ForwardBackMovement | AutoRun | Backwards | Forward, // 0x00011030
			All = MoveMask | TurnMask | StrafeMask,                  // 0x000713F0
			AllAllowed = Descend | JumpAscend | AutoRun | TurnRight | TurnLeft | StrafeRight | StrafeLeft | Backwards | Forward, // 0x000073F0
		}

		public enum ClickToMoveType
		{
			LeftClick = 1,
			Face = 2,
			StopThrowsException = 3,
			Move = 4,
			NpcInteract = 5,
			Loot = 6,
			ObjInteract = 7,
			FaceOther = 8,
			Skin = 9,
			AttackPosition = 10,
			AttackGuid = 11,
			ConstantFace = 12,
			None = 13,
		}

		public static bool IsMoving
		{
			get
			{
				LocalPlayer? me = ObjectManager.Me;
				if (me == null) return false;
				return me.IsMoving;
			}
		}

		/// <summary>
		/// FEAT-02: GUID of the active mover (player, or vehicle/possession target).
		/// </summary>
		public static ulong ActiveMoverGuid
		{
			get
			{
				Memory? memory = ObjectManager.Wow;
				if (memory == null) return 0UL;
				return memory.Read<ulong>(Styx.Offsets.GlobalOffsets.ActiveMoverGuid);
			}
		}

		/// <summary>
		/// BUG-02: Returns the active mover as WoWUnit (not WoWPoint).
		/// Falls back to LocalPlayer if the guid resolves to null.
		/// </summary>
		public static WoWUnit? ActiveMover
		{
			get
			{
				ulong guid = ActiveMoverGuid;
				if (guid != 0UL)
				{
					var unit = ObjectManager.GetObjectByGuid<WoWUnit>(guid);
					if (unit != null)
						return unit;
				}
				return ObjectManager.Me;
			}
		}

		public static void MoveStop()
		{
			// Stop keyboard movement — single batched DoString (see StopMovement)
			StopMovement(MovementDirection.AllAllowed);
			
			// HB 4.3.4: always stop CTM unconditionally (smethod_5).
			// Previous code only stopped for Move/NpcInteract/Loot, missing
			// ObjInteract, Skin, AttackPosition, AttackGuid, ConstantFace.
			ClickToMoveStop();
		}

		public static void MoveStop(MovementDirection direction)
		{
			StopMovement(direction);
		}

		public static void StopMovement(MovementDirection direction)
		{
			StyxWoW.ResetAfk();
			
			// HB 3.3.5a pattern: batch all direction stops into a single Lua.DoString
			// call so only ONE Execute() is needed instead of up to 6.
			var sb = new System.Text.StringBuilder(128);
			if ((direction & MovementDirection.Forward) != 0)
				sb.Append("MoveForwardStop();");
			if ((direction & MovementDirection.Backwards) != 0)
				sb.Append("MoveBackwardStop();");
			if ((direction & MovementDirection.StrafeLeft) != 0)
				sb.Append("StrafeLeftStop();");
			if ((direction & MovementDirection.StrafeRight) != 0)
				sb.Append("StrafeRightStop();");
			if ((direction & MovementDirection.JumpAscend) != 0)
				sb.Append("AscendStop();");
			if ((direction & MovementDirection.Descend) != 0)
				sb.Append("DescendStop();");
			
			if (sb.Length > 0)
				Lua.DoString(sb.ToString());
		}

		public static void StopFace()
		{
			// HB 3.3.5a/4.3.4: StopFace does a final face-toward-current-target.
			if (ObjectManager.Me == null) return;
			WoWUnit? currentTarget = ObjectManager.Me.CurrentTarget;
			if (currentTarget != null && !ObjectManager.Me.IsMoving)
			{
				ConstantFaceStop(currentTarget.Guid);
			}
		}

		// HB 3.3.5a / 4.3.4: throttle redundant CTM calls.
		// Without this guard, calling CTM ~13x/sec to the same destination
		// resets WoW's internal movement state machine each time, preventing
		// smooth turns and causing the character to walk straight.
		public static void ClickToMove(WoWPoint destination)
		{
			ClickToMoveInfoStruct ctmInfo = ClickToMoveInfo;
			if (!ctmInfo.IsClickMoving
				|| (double)destination.DistanceSqr(ctmInfo.ClickPos) > 0.05
				|| !ObjectManager.Me.IsMoving)
			{
				CallClickToMove(ClickToMoveType.Move, 0UL, destination, 0f);
			}
		}

		public static void ClickToMove(WoWPoint destination, ulong interactGuid)
		{
			ClickToMoveType moveType = interactGuid == 0UL ? ClickToMoveType.Move : ClickToMoveType.NpcInteract;
			CallClickToMove(moveType, interactGuid, destination, 0f);
		}

		public static void ClickToMove(ulong guid, WoWPoint loc, ClickToMoveType type)
		{
			CallClickToMove(type, guid, loc, 0f);
		}

		public static void ClickToMove(ulong guid, ClickToMoveType type)
		{
			CallClickToMove(type, guid, WoWPoint.Empty, 0f);
		}

		public static void ClickToMove(ulong guid, WoWPoint loc, float precision, ClickToMoveType type)
		{
			CallClickToMove(type, guid, loc, precision);
		}

		public static void ClickToMove(float x, float y, float z)
		{
			ClickToMove(new WoWPoint(x, y, z));
		}

		// HB 3.3.5a style CTM execution
		private static void CallClickToMove(ClickToMoveType clickToMoveType, ulong guid, WoWPoint clickPos, float facing)
		{
			StyxWoW.ResetAfk();

			// Validate coordinates - NaN would crash WoW with INT_DIVIDE_BY_ZERO
			if (float.IsNaN(clickPos.X) || float.IsNaN(clickPos.Y) || float.IsNaN(clickPos.Z))
			{
				Logging.WriteDebug("[CTM] ERROR: ClickPos contains NaN coordinates, aborting CTM");
				return;
			}

			ExecutorRand? executor = ObjectManager.Executor;
			if (executor == null)
				throw new Exception("Invalid executor used in CGPlayer_C__ClickToMove");

			// HB 3.3.5a/4.3.4: Transform world coordinates to transport-local
			// when the player is on a transport (boat, zeppelin, elevator).
			WoWGameObject? transport = ObjectManager.Me?.Transport;
			if (transport != null)
			{
				Matrix worldMatrix = transport.GetWorldMatrix();
				Matrix.Invert(ref worldMatrix, out worldMatrix);
				Vector3 vec = new Vector3(clickPos.X, clickPos.Y, clickPos.Z);
				Vector3 transformed = Vector3.Transform(vec, worldMatrix);
				clickPos = new WoWPoint(transformed.X, transformed.Y, transformed.Z);
			}

			using (AllocatedMemory allocatedMemory = new AllocatedMemory(20))
			{
				allocatedMemory.Write<WoWPoint>("ClickPos", clickPos);
				allocatedMemory.Write<ulong>("GUID", guid);

				lock (executor.AssemblyLock)
				{
					executor.Clear();
					executor.AddLine("push {0}", (uint)BitConverter.SingleToInt32Bits(facing));
					executor.AddLine("push {0}", allocatedMemory["ClickPos"]);
					executor.AddLine("push {0}", allocatedMemory["GUID"]);
					executor.AddLine("push {0}", (uint)clickToMoveType);
					executor.AddLine("call {0}", GetActivePlayerObject_Function);
					executor.AddLine("mov ecx, eax");
					executor.AddLine("call {0}", CTM_Function);
					executor.AddLine("retn");
					executor.Execute();
				}
			}
		}

		/// <summary>
		/// Stops the current Click-to-Move action.
		/// Calls CGPlayer_C__ClickToMoveStop like HB 3.3.5a.
		/// </summary>
		public static void ClickToMoveStop()
		{
			StyxWoW.ResetAfk();

			ExecutorRand? executor = ObjectManager.Executor;
			if (executor == null)
				throw new Exception("Invalid executor used in CGPlayer_C__ClickToMoveStop");

			LocalPlayer? me = ObjectManager.Me;
			if (me == null) return;

			lock (executor.AssemblyLock)
			{
				executor.Clear();
				executor.AddLine("mov ecx, {0}", me.BaseAddress);
				executor.AddLine("call {0}", CTM_Stop_Function);
				executor.AddLine("retn");
				executor.Execute();
			}
		}

		public static void Face(WoWPoint target)
		{
			LocalPlayer? me = ObjectManager.Me;
			if (me == null) return;

			float angle = (float)Math.Atan2(target.Y - me.Location.Y, target.X - me.Location.X);
			me.SetFacing(angle);
		}

		public static void Face(WoWUnit target)
		{
			if (target == null) return;
			Face(target.Location);
		}

		/// <summary>FEAT-03: Face with no args — faces current target.
		/// HB 4.3.4: only faces when standing still, uses ConstantFace.</summary>
		public static void Face()
		{
			LocalPlayer? me = ObjectManager.Me;
			if (me == null) return;
			WoWUnit? currentTarget = me.CurrentTarget;
			if (currentTarget != null && !me.IsMoving)
				ConstantFace(currentTarget.Guid);
		}

		/// <summary>HB 3.3.5a/4.3.4 shared pattern (smethod_0):
		/// stop=false → CTM LeftClick (causes game to face the target)
		/// stop=true  → SetFacing toward the object (final face), fallback to CTM StopThrowsException</summary>
		private static void FaceOrStopInternal(ulong guid, bool stop)
		{
			if (stop)
			{
				WoWObject? obj = ObjectManager.GetObjectByGuid<WoWObject>(guid);
				if (obj != null)
				{
					ObjectManager.Me?.SetFacing(obj.Location);
					return;
				}
			}
			ClickToMove(guid, stop ? ClickToMoveType.StopThrowsException : ClickToMoveType.LeftClick);
		}

		/// <summary>HB 4.3.4+ compatibility: Face an object by its GUID.
		/// Uses CTM LeftClick to trigger game-internal facing (HB 3.3.5a smethod_0).</summary>
		public static void Face(ulong guid)
		{
			if (ObjectManager.Me == null) return;
			FaceOrStopInternal(guid, false);
		}

		public static void Move(MovementDirection direction)
		{
			Move(direction, true);
		}

		public static void Move(MovementDirection direction, TimeSpan duration)
		{
			Move(direction, true);
			StyxWoW.Sleep(duration);
			Move(direction, false);
		}

		public static void Move(MovementDirection direction, bool start)
		{
			StyxWoW.ResetAfk();

			// Batch all direction commands into a single Lua.DoString to avoid
			// up to 8 separate Execute() calls (~16ms each = 128ms freeze).
			var sb = new System.Text.StringBuilder(128);

			if ((direction & MovementDirection.Forward) != 0)
				sb.Append(start ? "MoveForwardStart();" : "MoveForwardStop();");
			if ((direction & MovementDirection.Backwards) != 0)
				sb.Append(start ? "MoveBackwardStart();" : "MoveBackwardStop();");
			if ((direction & MovementDirection.StrafeLeft) != 0)
				sb.Append(start ? "StrafeLeftStart();" : "StrafeLeftStop();");
			if ((direction & MovementDirection.StrafeRight) != 0)
				sb.Append(start ? "StrafeRightStart();" : "StrafeRightStop();");
			if ((direction & MovementDirection.TurnLeft) != 0)
				sb.Append(start ? "TurnLeftStart();" : "TurnLeftStop();");
			if ((direction & MovementDirection.TurnRight) != 0)
				sb.Append(start ? "TurnRightStart();" : "TurnRightStop();");
			if ((direction & MovementDirection.JumpAscend) != 0)
				sb.Append(start ? "JumpOrAscendStart();" : "AscendStop();");
			if ((direction & MovementDirection.Descend) != 0)
			{
				if (start)
				{
					// Only descend if actually flying/swimming - on ground SitStandOrDescendStart() toggles sit!
					if (StyxWoW.Me != null && (StyxWoW.Me.IsFlying || StyxWoW.Me.IsSwimming))
						sb.Append("SitStandOrDescendStart();");
				}
				else
					sb.Append("DescendStop();");
			}

			if (sb.Length > 0)
				Lua.DoString(sb.ToString());
		}

		public static void Jump()
		{
			StyxWoW.ResetAfk();
			Lua.DoString("JumpOrAscendStart()");
		}

		public static void Ascend()
		{
			StyxWoW.ResetAfk();
			Lua.DoString("JumpOrAscendStart()");
		}

		public static void Descend()
		{
			StyxWoW.ResetAfk();
			Move(MovementDirection.Descend);
		}

		public static void DescendStop()
		{
			StyxWoW.ResetAfk();
			MoveStop(MovementDirection.Descend);
		}

		public static void ConstantFace(float angle)
		{
			LocalPlayer? me = ObjectManager.Me;
			if (me == null) return;
			me.SetFacing(angle);
		}

		public static void ConstantFace(ulong guid)
		{
			if (ObjectManager.Me == null) return;
			FaceOrStopInternal(guid, false);
		}

		/// <summary>FEAT-04: Stop constant-facing a specific target GUID.
		/// HB 3.3.5a/4.3.4: does a final SetFacing toward the target, then stops.</summary>
		public static void ConstantFaceStop(ulong guid)
		{
			if (ObjectManager.Me == null) return;
			FaceOrStopInternal(guid, true);
		}

		public static void ConstantFaceStop()
		{
			StopFace();
		}

		public static WoWPoint CalculatePointFrom(WoWPoint target, float distance)
		{
			LocalPlayer? me = ObjectManager.Me;
			if (me == null)
				return target;
			return WoWMathHelper.CalculatePointFrom(me.Location, target, distance);
		}

		public static float GetHeadingDiff(float heading1, float heading2)
		{
			float diff = Math.Abs(heading1 - heading2);
			if (diff > Math.PI)
				diff = (float)(2 * Math.PI - diff);
			return diff;
		}

		public static void GetHeadingDiff(double currentHeading, double destHeading, out double headingDiff, out int directionCoeff)
		{
			headingDiff = currentHeading - destHeading;
			directionCoeff = (int)(headingDiff / Math.Abs(headingDiff));
			headingDiff = Math.Abs(headingDiff);
			if (headingDiff > 3.1415926535897931)
			{
				headingDiff = 6.2831853071795862 - headingDiff;
				directionCoeff *= -1;
			}
		}

		public static void Navigate(WoWPoint destination)
		{
			Navigate(destination, 1.5f);
		}

		public static void Navigate(WoWPoint destination, float precision)
		{
			Navigator.MoveTo(destination, precision);
		}

		#region Structs and Enums

		public struct ClickToMoveInfoStruct
		{
			private float _reserved1;
			public float Velocity;
			public float InteractDistSqrd;
			public float InteractDist;
			private float _reserved2;
			public float FaceAngle;
			public uint CurrentTime;
			public ClickToMoveType Type;
			public ulong InteractGuid;
			[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 22)]
			private uint[] _reserved3;
			public WoWPoint CurrentPos;
			public WoWPoint ClickPos;
			private float _reserved4;

			public bool IsClickMoving => Type == ClickToMoveType.Move;
			public bool IsUsing => Type != ClickToMoveType.None;

			public override string ToString()
			{
				return $"Velocity: {Velocity}, InteractDistSqrd: {InteractDistSqrd}, InteractDist: {InteractDist}, FaceAngle: {FaceAngle}, CurrentTime: {CurrentTime}, Type: {Type}, InteractGuid: {InteractGuid:X}, CurrentPos: {CurrentPos}, ClickPos: {ClickPos}";
			}
		}

		/// <summary>FEAT-42: InputControl with Flags field matching HB 4.3.4 layout.</summary>
		public struct InputControl
		{
			public uint Time;
			[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
			public MovementDirection Flags;
			[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 5)]
			private uint[] _reserved1;
			public MovementControl Movement;
			[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 11)]
			private uint[] _reserved2;
		}

		/// <summary>HB 3.3.5a/4.3.4: MovementControl is a 40-byte struct, not an enum.
		/// The internal fields are private/opaque in both HB versions.</summary>
		public struct MovementControl
		{
			private uint _flags;
			[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 9)]
			private uint[] _reserved;
		}

		#endregion
	}
}
