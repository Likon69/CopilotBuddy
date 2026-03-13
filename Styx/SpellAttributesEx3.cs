using System;

namespace Styx
{
    [Flags]
    public enum SpellAttributesEx3 : uint
    {
        Unk0              = 1U,
        Unk1              = 2U,
        Unk2              = 4U,
        Unk3              = 8U,
        Unk4              = 16U,
        Unk5              = 32U,
        Unk6              = 64U,
        Unk7              = 128U,
        TargetOnlyPlayer  = 256U,
        Unk9              = 512U,
        MainHand          = 1024U,
        Battleground      = 2048U,
        CastOnDead        = 4096U,
        Unk13             = 8192U,
        Unk14             = 16384U,
        Unk15             = 32768U,
        Unk16             = 65536U,
        NoInitialAggro    = 131072U,
        CantMiss          = 262144U,
        Unk19             = 524288U,
        DeathPersistent   = 1048576U,
        Unk21             = 2097152U,
        ReqWand           = 4194304U,
        Unk23             = 8388608U,
        ReqOffhand        = 16777216U,
        Unk25             = 33554432U,
        Unk26             = 67108864U,
        Unk27             = 134217728U,
        Unk28             = 268435456U,
        Unk29             = 536870912U,
        Unk30             = 1073741824U,
        Unk31             = 2147483648U
    }
}
