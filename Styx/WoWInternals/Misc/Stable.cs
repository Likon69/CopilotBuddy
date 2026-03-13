using System.Collections.Generic;
using GreenMagic;

namespace Styx.WoWInternals.Misc
{
    public class Stable
    {
        // WoW 3.3.5a: max 25 pet slots (indices 0-4 = active/carried, 5-24 = stabled).
        private const int TotalSlots    = 25;
        private const int CarriedSlots  = 5;

        // TODO: Resolve 3.3.5a memory offset for the stable pet data array.
        // In HB 4.3.4 this is StyxWoW.Offsets.method_0(5061) (relative base address).
        // When the offset is known for 3.3.5a, replace 0 below with the correct relative address.
        private const uint StablePetArrayOffset = 0;

        public StabledPet? GetPet(int index)
        {
            if (index < 0 || index >= TotalSlots) return null;
            if (StablePetArrayOffset == 0) return null;

            var info = ObjectManager.Wow!.ReadRelative<StabledPetNative>(
                new uint[] { StablePetArrayOffset + (uint)(index * FastSize<StabledPetNative>.Size) });

            return info.PetNumber != 0 ? new StabledPet(info, index) : null;
        }

        public List<StabledPet> GetCarriedPets() => GetRange(0, CarriedSlots);

        public List<StabledPet> GetNonCarriedPets() => GetRange(CarriedSlots, TotalSlots - CarriedSlots);

        public List<StabledPet> GetPets() => GetRange(0, TotalSlots);

        private List<StabledPet> GetRange(int start, int count)
        {
            var result = new List<StabledPet>();
            if (StablePetArrayOffset == 0) return result;

            for (int i = start; i < start + count; i++)
            {
                var pet = GetPet(i);
                if (pet != null) result.Add(pet);
            }
            return result;
        }
    }
}
