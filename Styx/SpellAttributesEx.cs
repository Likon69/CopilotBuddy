using System;

namespace Styx
{
    [Flags]
    public enum SpellAttributesEx : uint
    {
        Unk0                      = 1U,
        DrainAllPower             = 2U,
        Channeled1                = 4U,
        CantReflected             = 8U,
        Unk4                      = 16U,
        NotBreakStealth           = 32U,
        Channeled2                = 64U,
        Negative                  = 128U,
        NotInCombatTarget         = 256U,
        Unk9                      = 512U,
        NoThreat                  = 1024U,
        Unk11                     = 2048U,
        Unk12                     = 4096U,
        Farsight                  = 8192U,
        Unk14                     = 16384U,
        DispelAurasOnImmunity     = 32768U,
        UnaffectedBySchoolImmune  = 65536U,
        Unk17                     = 131072U,
        Unk18                     = 262144U,
        Unk19                     = 524288U,
        ReqTargetComboPoints      = 1048576U,
        Unk21                     = 2097152U,
        ReqComboPoints            = 4194304U,
        Unk23                     = 8388608U,
        Unk24                     = 16777216U,
        Unk25                     = 33554432U,
        Unk26                     = 67108864U,
        Unk27                     = 134217728U,
        Unk28                     = 268435456U,
        Unk29                     = 536870912U,
        Unk30                     = 1073741824U,
        Unk31                     = 2147483648U
    }
}
