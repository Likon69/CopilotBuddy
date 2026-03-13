using System;

namespace Styx
{
    [Flags]
    public enum SpellAttributes : uint
    {
        Unk0                      = 1U,
        Ranged                    = 2U,
        OnNextSwing1              = 4U,
        Unk3                      = 8U,
        Unk4                      = 16U,
        TradeSkillSpell           = 32U,
        Passive                   = 64U,
        Unk7                      = 128U,
        Unk8                      = 256U,
        Unk9                      = 512U,
        OnNextSwing2              = 1024U,
        Unk11                     = 2048U,
        DaytimeOnly               = 4096U,
        NightOnly                 = 8192U,
        IndoorsOnly               = 16384U,
        OutdoorsOnly              = 32768U,
        NotShapeshift             = 65536U,
        OnlyStealthed             = 131072U,
        Unk18                     = 262144U,
        LevelDamageCalculation    = 524288U,
        StopAttackTarget          = 1048576U,
        ImpossibleDodgeParryBlock = 2097152U,
        SetTrackingTarget         = 4194304U,
        Unk23                     = 8388608U,
        CastableWhileMounted      = 16777216U,
        DisabledWhileActive       = 33554432U,
        Unk26                     = 67108864U,
        CastableWhileSitting      = 134217728U,
        CantUsedInCombat          = 268435456U,
        UnaffectedByInvulnerability = 536870912U,
        Unk30                     = 1073741824U,
        CantCancel                = 2147483648U
    }
}
