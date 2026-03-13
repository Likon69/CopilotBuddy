using System;

namespace Styx
{
    [Flags]
    public enum SpellAttributesEx5 : uint
    {
        Unk0                  = 1U,
        NoReagentWhilePrep    = 2U,
        Unk2                  = 4U,
        UsableWhileStunned    = 8U,
        Unk4                  = 16U,
        SingleTargetSpell     = 32U,
        Unk6                  = 64U,
        Unk7                  = 128U,
        Unk8                  = 256U,
        StartPeriodicAtApply  = 512U,
        HideDuration          = 1024U,
        Unk11                 = 2048U,
        Unk12                 = 4096U,
        Unk13                 = 8192U,
        Unk14                 = 16384U,
        Unk15                 = 32768U,
        Unk16                 = 65536U,
        UsableWhileFeared     = 131072U,
        UsableWhileConfused   = 262144U,
        Unk19                 = 524288U,
        Unk20                 = 1048576U,
        Unk21                 = 2097152U,
        Unk22                 = 4194304U,
        Unk23                 = 8388608U,
        Unk24                 = 16777216U,
        Unk25                 = 33554432U,
        Unk26                 = 67108864U,
        Unk27                 = 134217728U,
        Unk28                 = 268435456U,
        Unk29                 = 536870912U,
        Unk30                 = 1073741824U,
        Unk31                 = 2147483648U
    }
}
