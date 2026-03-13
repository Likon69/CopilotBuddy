using System;

namespace Styx.WoWInternals
{
    [Flags]
    public enum MoveFlags : uint
    {
        None                = 0U,
        Forward             = 1U,
        Backward            = 2U,
        StrafeLeft          = 4U,
        StrafeRight         = 8U,
        Left                = 16U,
        Right               = 32U,
        PitchUp             = 64U,
        PitchDown           = 128U,
        Walk                = 256U,
        Levitating          = 512U,
        Root                = 1024U,
        Falling             = 2048U,
        FallingFar          = 4096U,
        PendingStop         = 8192U,
        PendingStrafeStop   = 16384U,
        PendingForward      = 32768U,
        PendingBackward     = 65536U,
        PendingStrafeLeft   = 131072U,
        PendingStrafeRight  = 262144U,
        PendingRoot         = 524288U,
        Swimming            = 1048576U,
        Ascending           = 2097152U,
        Descending          = 4194304U,
        CanFly              = 8388608U,
        Flying              = 16777216U,
        SplineElevation     = 33554432U,
        SplineEnabled       = 67108864U,
        WaterWalking        = 134217728U,
        SafeFall            = 268435456U,
        Hover               = 536870912U,
        PlayerFlag          = 2147483648U,
        // Composite masks
        TurnMask            = 48U,
        MoveMask            = 63U,
        StrafeMask          = 12U,
        FallMask            = 6144U,
        PitchMask           = 192U,
        MotionMask          = 6291711U,
        SpellCastMotionMask = 6293519U,
        StoppedMask         = 200719U
    }
}
