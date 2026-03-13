using System.Runtime.InteropServices;
using System.Text;
using Styx.Logic.Combat;

namespace Styx.WoWInternals.Misc
{
    // Internal equivalent of ns13.Struct38 from HB 4.3.4 (obfuscated native struct).
    [StructLayout(LayoutKind.Sequential)]
    internal struct StabledPetNative
    {
        public uint PetNumber;
        public int  PetLevel;
        public uint CreatureId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public byte[] NameBytes;
        private uint _padding;
    }

    public class StabledPet
    {
        private string? _name;

        internal StabledPet(StabledPetNative info, int index)
        {
            _info  = info;
            Index  = index;
        }

        private readonly StabledPetNative _info;

        public int Index { get; }

        public bool CanSummon => Index >= 0 && Index < 5;

        public uint PetNumber => _info.PetNumber;

        public int PetLevel => _info.PetLevel;

        public uint CreatureId => _info.CreatureId;

        public string Name
        {
            get
            {
                if (_name != null) return _name;
                if (_info.NameBytes == null) return string.Empty;
                // Null-terminated UTF-8 string
                int len = 0;
                while (len < _info.NameBytes.Length && _info.NameBytes[len] != 0)
                    len++;
                _name = Encoding.UTF8.GetString(_info.NameBytes, 0, len);
                return _name;
            }
        }

        public bool Summon()
        {
            if (!CanSummon) return false;
            return SpellManager.Cast("Call Pet " + (Index + 1));
        }

        public int GetSummonSpellId()
        {
            if (!CanSummon) return 0;
            WoWSpell? spell;
            return SpellManager.Spells.TryGetValue("Call Pet " + (Index + 1), out spell) ? spell.Id : 0;
        }
    }
}
