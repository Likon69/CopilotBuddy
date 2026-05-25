using System;
using System.Runtime.InteropServices;
using GreenMagic;
using Styx.Logic.Pathing;

namespace Styx.WoWInternals.WoWObjects
{
    /// <summary>
    /// Represents movement information for a WoW unit.
    /// Contains position, speed, flags and other movement-related data.
    /// </summary>
    public class WoWMovementInfo
    {
        private readonly uint _ptr;
        private CMovementData _movementData;

        /// <summary>
        /// Creates a new WoWMovementInfo from a pointer to the movement data structure.
        /// </summary>
        /// <param name="ptr">Pointer to the CMovementData structure in WoW memory</param>
        public WoWMovementInfo(uint ptr)
        {
            _ptr = ptr;
            if (ptr != 0)
            {
                Memory? wow = ObjectManager.Wow;
                if (wow != null)
                {
                    _movementData = wow.Read<CMovementData>(_ptr);
                }
            }
        }

        /// <summary>
        /// Whether the movement info pointer is valid.
        /// </summary>
        public bool IsValid => _ptr != 0U;

        /// <summary>
        /// Current position of the unit.
        /// </summary>
        public WoWPoint Position
        {
            get
            {
                if (!IsValid) return WoWPoint.Zero;
                return GetStorageField<WoWPoint>(MoveInfoOffsets.Pos);
            }
        }

        /// <summary>
        /// Current facing direction in radians.
        /// </summary>
        public float Heading
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.Heading);
            }
        }

        /// <summary>
        /// GUID of the transport the unit is on (if any).
        /// </summary>
        public ulong TransportGuid
        {
            get
            {
                if (!IsValid) return 0UL;
                return GetStorageField<ulong>(MoveInfoOffsets.TransportGuid);
            }
        }

        /// <summary>
        /// Movement flags (Forward, Backward, Strafe, etc.).
        /// </summary>
        public uint MovementFlags
        {
            get
            {
                if (!IsValid) return 0U;
                return GetStorageField<uint>(MoveInfoOffsets.Flags);
            }
        }

        /// <summary>
        /// Secondary movement flags.
        /// </summary>
        public uint MovementFlags2
        {
            get
            {
                if (!IsValid) return 0U;
                return GetStorageField<uint>(MoveInfoOffsets.Flags2);
            }
        }

        /// <summary>
        /// Time the unit has been moving (in milliseconds).
        /// 0 if the unit is not moving.
        /// </summary>
        public uint TimeMoved
        {
            get
            {
                if (!IsValid) return 0U;
                return GetStorageField<uint>(MoveInfoOffsets.TimeMoved);
            }
        }

        /// <summary>
        /// Sin of the strafe angle.
        /// </summary>
        public float SinAngle
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.SinAngle);
            }
        }

        /// <summary>
        /// Cos of the strafe angle.
        /// </summary>
        public float CosAngle
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.CosAngle);
            }
        }

        /// <summary>
        /// Time spent falling (in milliseconds).
        /// </summary>
        public uint FallTime
        {
            get
            {
                if (!IsValid) return 0U;
                return GetStorageField<uint>(MoveInfoOffsets.FallTime);
            }
        }

        /// <summary>
        /// Height at which the fall started.
        /// </summary>
        public float FallStartHeight
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.FallStartHeight);
            }
        }

        /// <summary>
        /// Last recorded fall height.
        /// </summary>
        public float LastFallHeight
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.LastFallHeight);
            }
        }

        /// <summary>
        /// Current movement speed.
        /// </summary>
        public float CurrentSpeed
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.CurrentSpeed);
            }
        }

        /// <summary>
        /// Walk speed.
        /// </summary>
        public float WalkSpeed
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.WalkSpeed);
            }
        }

        /// <summary>
        /// Run speed.
        /// </summary>
        public float RunSpeed
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.RunSpeed);
            }
        }

        /// <summary>
        /// Run backward speed.
        /// </summary>
        public float RunBackSpeed
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.RunBackSpeed);
            }
        }

        /// <summary>
        /// Swim speed.
        /// </summary>
        public float SwimSpeed
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.SwimSpeed);
            }
        }

        /// <summary>
        /// Swim backward speed.
        /// </summary>
        public float SwimBackSpeed
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.SwimBackSpeed);
            }
        }

        /// <summary>
        /// Fly speed.
        /// </summary>
        public float FlySpeed
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.FlySpeed);
            }
        }

        /// <summary>
        /// Fly backward speed.
        /// </summary>
        public float FlyBackSpeed
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.FlyBackSpeed);
            }
        }

        /// <summary>
        /// Turn speed (rotation rate).
        /// </summary>
        public float TurnSpeed
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.TurnSpeed);
            }
        }

        /// <summary>
        /// Jump velocity.
        /// </summary>
        public float JumpVelocity
        {
            get
            {
                if (!IsValid) return 0f;
                return GetStorageField<float>(MoveInfoOffsets.JumpVelocity);
            }
        }

        /// <summary>
        /// Whether the unit is currently moving.
        /// Uses movement flags like HB 4.3.4 instead of TimeMoved for accurate detection.
        /// This prevents false positives during facing/turning operations.
        /// </summary>
        public bool IsMoving => (MovementFlags & (uint)MovementFlag.MotionMask) != 0;

        /// <summary>
        /// Whether the unit is moving forward.
        /// </summary>
        public bool MovingForward => (MovementFlags & (uint)MovementFlag.Forward) != 0;

        /// <summary>
        /// Whether the unit is moving backward.
        /// </summary>
        public bool MovingBackward => (MovementFlags & (uint)MovementFlag.Backward) != 0;

        /// <summary>
        /// Whether the unit is strafing (left or right).
        /// </summary>
        public bool IsStrafing => (MovementFlags & ((uint)MovementFlag.StrafeLeft | (uint)MovementFlag.StrafeRight)) != 0;

        /// <summary>
        /// Whether the unit is strafing left.
        /// </summary>
        public bool StrafingLeft => (MovementFlags & (uint)MovementFlag.StrafeLeft) != 0;

        /// <summary>
        /// Whether the unit is strafing right.
        /// </summary>
        public bool StrafingRight => (MovementFlags & (uint)MovementFlag.StrafeRight) != 0;

        /// <summary>
        /// Whether the unit is turning left.
        /// </summary>
        public bool TurningLeft => (MovementFlags & (uint)MovementFlag.TurnLeft) != 0;

        /// <summary>
        /// Whether the unit is turning right.
        /// </summary>
        public bool TurningRight => (MovementFlags & (uint)MovementFlag.TurnRight) != 0;

        /// <summary>
        /// Whether the unit is falling.
        /// </summary>
        public bool IsFalling => (MovementFlags & (uint)MovementFlag.Falling) != 0;

        /// <summary>
        /// Whether the unit is swimming.
        /// </summary>
        public bool IsSwimming => (MovementFlags & (uint)MovementFlag.Swimming) != 0;

        /// <summary>
        /// Whether the unit is flying.
        /// </summary>
        public bool IsFlying => (MovementFlags & (uint)MovementFlag.Flying) != 0;

        /// <summary>
        /// Whether the unit can fly (has flight capability enabled).
        /// Ported from HB 4.3.4.
        /// </summary>
        public bool CanFly => (MovementFlags & (uint)MovementFlag.CanFly) != 0;

        /// <summary>
        /// Whether the unit is ascending.
        /// </summary>
        public bool IsAscending => (MovementFlags & (uint)MovementFlag.Ascending) != 0;

        /// <summary>
        /// Whether the unit is descending.
        /// </summary>
        public bool IsDescending => (MovementFlags & (uint)MovementFlag.Descending) != 0;

        /// <summary>
        /// Whether the unit is jumping or in a short fall.
        /// Ported from HB 4.3.4.
        /// </summary>
        public bool JumpingOrShortFalling => (MovementFlags & (uint)MovementFlag.Falling) != 0;

        /// <summary>
        /// Movement destination (if using click-to-move or pathing).
        /// </summary>
        public WoWPoint Destination
        {
            get
            {
                // In 3.3.5a the destination is stored in the spline data
                // For simplicity, return current position if not available
                return Position;
            }
        }

        /// <summary>
        /// Reads a field from the movement data structure at the given offset.
        /// </summary>
        private T GetStorageField<T>(MoveInfoOffsets offset) where T : struct
        {
            Memory? wow = ObjectManager.Wow;
            if (wow == null) return default;
            return wow.Read<T>(_ptr + (uint)offset);
        }

        /// <summary>
        /// Offsets within the CMovementData structure for WoW 3.3.5a (Build 12340).
        /// </summary>
        public enum MoveInfoOffsets
        {
            Pos = 16,
            Heading = 32,
            TransportGuid = 8,
            Flags = 68,
            Flags2 = 72,
            TimeMoved = 96,
            SinAngle = 112,
            CosAngle = 116,
            FallTime = 128,
            FallStartHeight = 132,
            LastFallHeight = 136,
            CurrentSpeed = 140,
            WalkSpeed = 144,
            RunSpeed = 148,
            RunBackSpeed = 152,
            SwimSpeed = 156,
            SwimBackSpeed = 160,
            FlySpeed = 164,
            FlyBackSpeed = 168,
            TurnSpeed = 172,
            JumpVelocity = 184
        }

        /// <summary>
        /// Movement flags for WoW 3.3.5a.
        /// Ported from HB 4.3.4.
        /// </summary>
        [Flags]
        public enum MovementFlag : uint
        {
            None = 0x00000000,
            Forward = 0x00000001,
            Backward = 0x00000002,
            StrafeLeft = 0x00000004,
            StrafeRight = 0x00000008,
            TurnLeft = 0x00000010,
            TurnRight = 0x00000020,
            PitchUp = 0x00000040,
            PitchDown = 0x00000080,
            Walk = 0x00000100,
            Walking = Walk,
            Levitating = 0x00000200,
            Root = 0x00000400,
            Falling = 0x00000800,
            FallingFar = 0x00001000,
            PendingStop = 0x00002000,
            PendingStrafeStop = 0x00004000,
            PendingSTrFlagStop = PendingStrafeStop,
            PendingForward = 0x00008000,
            PendingBackward = 0x00010000,
            PendingStrafeLeft = 0x00020000,
            PendingSTrFlagLeft = PendingStrafeLeft,
            PendingStrafeRight = 0x00040000,
            PendingSTrFlagRight = PendingStrafeRight,
            PendingRoot = 0x00080000,
            // 0x00100000 unused in 3.3.5a (gap between pending flags and swim/fly flags)
            Swimming = 0x00200000,          // IsSwimming_Mask = 0x200000 (Offsets335.txt)
            Ascending = 0x00400000,
            Descending = 0x00800000,
            CanFly = 0x01000000,
            Flying = 0x02000000,            // IsFlying_Mask = 0x2000000 (Offsets335.txt)
            SplineElevation = 0x04000000,
            SplineEnabled = 0x08000000,     // CMovement__SetMovementFlag_0x8000000
            WaterWalking = 0x10000000,      // CMovement__ToggleMovementFlag_0x10000000_WaterWalk
            Waterwalking = WaterWalking,
            SafeFall = 0x20000000,          // CMovement__ToggleMovementFlag_0x20000000_FeatherFall
            FallingSlow = SafeFall,
            Hover = 0x40000000,             // CMovement__ToggleMovementFlag_0x40000000_Hover
            PlayerFlag = 0x80000000,
            
            // Masks (ported from HB 4.3.4)
            TurnMask = TurnRight | TurnLeft,
            StrafeMask = StrafeRight | StrafeLeft,
            PitchMask = PitchDown | PitchUp,
            MoveMask = TurnMask | StrafeMask | Backward | Forward,
            FallMask = FallingFar | Falling,
            MotionMask = PitchMask | StrafeMask | TurnMask | Backward | Forward
        }

        /// <summary>
        /// The raw CMovementData structure as stored in WoW memory.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CMovementData
        {
            public uint moveLink;
            public uint transportLink;
            public ulong TransportGuid;
            public WoWPoint Position;
            public int Unk00;
            public float Facing;
            public float Pitch;
            public float NormalX;
            public float NormalY;
            public float NormalZ1;
            public float NormalZ2;
            public float NormalZ3;
            public int Flags;
            public uint dwUnk0;
            public uint dwMoveFlags;
            public uint dwUnk3;
            public WoWPoint AnchorPosition;
            public float AnchorFacing;
            public float AnchorPitch;
            public uint dwUnk12;
            public float directionX;
            public float directionY;
            public float directionZ;
            public float direction2dX;
            public float direction2dY;
            public float cosAnchorPitch;
            public float sinAnchorPitch;
            public int FallDuration;
            public float FallUnk;
            public float CurrentSpeed;
            public float WalkSpeed;
            public float RunSpeed;
            public float UnkSpeed;
            public float SwimSpeed;
            public float TurnRate;
            public float CollisionBoxHalfDepth;
            public float CollisionBoxHeight;
            public float StepUpHeight;
            public float turnRate;
            public float WaterSurfaceElevation;
            public uint Spline;
            public float Pi;
        }
    }
}
