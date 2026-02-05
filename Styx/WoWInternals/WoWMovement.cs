using System;
using System.Threading;
using GreenMagic;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;

namespace Styx.WoWInternals
{
	public static class WoWMovement
	{
		#region Constants - Offsets 3.3.5a (12340)

		// Click to move function address - FROM 335offsetsall.txt
		private const uint CTM_Function = 0x00727400;  // 7509504 decimal (CGPlayer_C__ClickToMove)
		// Stop movement function - FROM HB 3.3.5a GlobalOffsets.cs
		private const uint CTM_Stop_Function = 0x0072B3A0;  // 7517088 decimal (CGPlayer_C__ClickToMoveStop)
		// Click to move base address - FROM HB 3.3.5a (0xCA11D8)
		private const uint ClickToMove_Base = 0xCA11D8;  // 13243864 decimal
		// Active input control pointer
		private const uint ActiveInputControl_Ptr = 0xC24D54;  // 12732756 decimal
		// GetActivePlayerObject - FROM 335offsetsall.txt
		private const uint GetActivePlayerObject_Function = 0x004038F0;  // 4208880 decimal


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

		public static WoWPoint ActiveMover
		{
			get
			{
				LocalPlayer? me = ObjectManager.Me;
				return me?.Location ?? WoWPoint.Zero;
			}
		}

		public static void MoveStop()
		{
			// Stop keyboard movement
			StopMovement(MovementDirection.AllAllowed);
			Lua.DoString("MoveForwardStop();MoveBackwardStop();StrafeLeftStop();StrafeRightStop();");
			
			// Stop Click-to-Move if active (like HB 3.3.5a)
			if (ClickToMoveInfo.Type == ClickToMoveType.Move || 
			    ClickToMoveInfo.Type == ClickToMoveType.NpcInteract ||
			    ClickToMoveInfo.Type == ClickToMoveType.Loot)
			{
				ClickToMoveStop();
			}
		}

		public static void MoveStop(MovementDirection direction)
		{
			StopMovement(direction);
		}

		public static void StopMovement(MovementDirection direction)
		{
			StyxWoW.ResetAfk();
			
			if ((direction & MovementDirection.Forward) != 0)
				Lua.DoString("MoveForwardStop()");
			if ((direction & MovementDirection.Backwards) != 0)
				Lua.DoString("MoveBackwardStop()");
			if ((direction & MovementDirection.StrafeLeft) != 0)
				Lua.DoString("StrafeLeftStop()");
			if ((direction & MovementDirection.StrafeRight) != 0)
				Lua.DoString("StrafeRightStop()");
		}

		public static void StopFace()
		{
			MoveStop();
		}

		public static void ClickToMove(WoWPoint destination)
		{
			CallClickToMove(ClickToMoveType.Move, 0UL, destination, 0f);
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

		// HB 3.3.5a style CTM execution
		private static void CallClickToMove(ClickToMoveType clickToMoveType, ulong guid, WoWPoint clickPos, float facing)
		{
			StyxWoW.ResetAfk();

			ExecutorRand? executor = ObjectManager.Executor;
			if (executor == null)
				throw new Exception("Invalid executor used in CGPlayer_C__ClickToMove");

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

		public static void Move(MovementDirection direction)
		{
			Move(direction, true);
		}

		public static void Move(MovementDirection direction, TimeSpan duration)
		{
			Move(direction, true);
			Thread.Sleep(duration);
			Move(direction, false);
		}

		public static void Move(MovementDirection direction, bool start)
		{
			StyxWoW.ResetAfk();

			if ((direction & MovementDirection.Forward) != 0)
			{
				if (start)
					Lua.DoString("MoveForwardStart()");
				else
					Lua.DoString("MoveForwardStop()");
			}
			if ((direction & MovementDirection.Backwards) != 0)
			{
				if (start)
					Lua.DoString("MoveBackwardStart()");
				else
					Lua.DoString("MoveBackwardStop()");
			}
			if ((direction & MovementDirection.StrafeLeft) != 0)
			{
				if (start)
					Lua.DoString("StrafeLeftStart()");
				else
					Lua.DoString("StrafeLeftStop()");
			}
			if ((direction & MovementDirection.StrafeRight) != 0)
			{
				if (start)
					Lua.DoString("StrafeRightStart()");
				else
					Lua.DoString("StrafeRightStop()");
			}
			if ((direction & MovementDirection.TurnLeft) != 0)
			{
				if (start)
					Lua.DoString("TurnLeftStart()");
				else
					Lua.DoString("TurnLeftStop()");
			}
			if ((direction & MovementDirection.TurnRight) != 0)
			{
				if (start)
					Lua.DoString("TurnRightStart()");
				else
					Lua.DoString("TurnRightStop()");
			}
			if ((direction & MovementDirection.JumpAscend) != 0)
			{
				if (start)
					Lua.DoString("JumpOrAscendStart()");
				else
					Lua.DoString("AscendStop()");
			}
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
			Lua.DoString("DescendStop()");  // There's no DescendStart in WoW 3.3.5
		}

		public static void ConstantFace(float angle)
		{
			LocalPlayer? me = ObjectManager.Me;
			if (me == null) return;
			me.SetFacing(angle);
		}

		public static void ConstantFace(ulong guid)
		{
			WoWObject? obj = ObjectManager.GetObjectByGuid<WoWObject>(guid);
			if (obj is WoWUnit unit)
			{
				Face(unit);
			}
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

		public struct InputControl
		{
			public uint Time;
			public MovementControl MovementControl;
		}

		[Flags]
		public enum MovementControl : uint
		{
			None = 0,
			Forward = 1,
			Backward = 2,
			StrafeLeft = 4,
			StrafeRight = 8,
			TurnLeft = 16,
			TurnRight = 32,
			Jump = 64
		}

		#endregion
	}
}
