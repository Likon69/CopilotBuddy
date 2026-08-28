using System;
using GreenMagic;
using Styx.Helpers;

namespace Styx.WoWInternals.WoWObjects
{
    /// <summary>
    /// Represents a WoW sub-object (chair, door, bobber, etc.).
    /// WoW 3.3.5a build 12340.
    /// </summary>
    public class WoWSubObject
    {
        /// <summary>
        /// Creates a new WoW sub-object.
        /// </summary>
        /// <param name="baseAddress">Base address in memory</param>
        internal WoWSubObject(uint baseAddress)
        {
            BaseAddress = baseAddress;
        }

        /// <summary>
        /// Base address of the sub-object in memory.
        /// </summary>
        public uint BaseAddress { get; private set; }

        /// <summary>
        /// Interaction distance with the sub-object.
        /// Offset +12 (0xC).
        /// </summary>
        public float InteractDistance
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null) return 0f;
                return wow.Read<float>(BaseAddress + 12);
            }
        }

        /// <summary>
        /// Owner GameObject of this sub-object.
        /// Offset +4 pour le GUID.
        /// </summary>
        public WoWGameObject? OwnerObject
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null) return null;

                try
                {
                    uint guidLow = wow.Read<uint>(BaseAddress + 4);
                    // Construction du GUID complet depuis le low GUID
                    // Pour 3.3.5a, les GameObjects utilisent le type HIGHGUID_GAMEOBJECT (0xF11)
                    ulong guid = ((ulong)0xF110000000000000) | guidLow;
                    return ObjectManager.GetObjectByGuid<WoWGameObject>(guid);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Checks if the sub-object can be used.
        /// Calls the virtual method CanUse() via vtable.
        /// </summary>
        public bool CanUse()
        {
            if (BaseAddress == 0)
                return false;

            ExecutorRand? executor = ObjectManager.Executor;
            if (executor == null)
                throw new InvalidOperationException("Invalid executor in WoWSubObject.CanUse!");

            lock (executor.AssemblyLock)
            {
                executor.Clear();
                executor.AddLine($"mov ecx, {BaseAddress}");
                executor.AddLine("mov eax, [ecx]");
                executor.AddLine("mov eax, [eax+24]"); // Offset +24 pour vtable CanUse
                executor.AddLine("call eax");
                executor.AddLine("retn");
                executor.Execute();

                Memory? wow = ObjectManager.Wow;
                if (wow == null) return false;

                byte result = wow.Read<byte>(executor.ReturnPointer);
                return result != 0;
            }
        }

        /// <summary>
        /// Checks if the sub-object can be used now.
        /// </summary>
        public bool CanUseNow()
        {
            if (BaseAddress == 0)
                return false;

            ExecutorRand? executor = ObjectManager.Executor;
            if (executor == null)
                throw new InvalidOperationException("Invalid executor in WoWSubObject.CanUseNow");

            lock (executor.AssemblyLock)
            {
                executor.Clear();
                BuildCanUseNowAsm(executor, 0, 0, 0);
                executor.AddLine("retn");
                executor.Execute();

                Memory? wow = ObjectManager.Wow;
                if (wow == null) return false;

                byte result;
                using (StyxWoW.Memory.TemporaryCacheState(false))
                {
                    result = wow.Read<byte>(executor.ReturnPointer);
                }
                return result != 0;
            }
        }

        /// <summary>
        /// Checks if the sub-object can be used now with failure reason.
        /// </summary>
        public bool CanUseNow(out GameError reason)
        {
            if (BaseAddress == 0)
            {
                reason = (GameError)0;
                return false;
            }

            ExecutorRand? executor = ObjectManager.Executor;
            if (executor == null)
                throw new InvalidOperationException("Invalid executor in WoWSubObject.CanUseNow");

            lock (executor.AssemblyLock)
            {
                using (AllocatedMemory reasonMem = new AllocatedMemory(4))
                {
                    executor.Clear();
                    BuildCanUseNowAsm(executor, reasonMem.Address, 0, 0);
                    executor.AddLine("retn");
                    executor.Execute();

                    Memory? wow = ObjectManager.Wow;
                    if (wow == null)
                    {
                        reason = (GameError)0;
                        return false;
                    }

                    using (StyxWoW.Memory.TemporaryCacheState(false))
                    {
                        reason = (GameError)wow.Read<uint>(reasonMem.Address);
                        byte result = wow.Read<byte>(executor.ReturnPointer);
                        return result != 0;
                    }
                }
            }
        }

        /// <summary>
        /// Uses the sub-object (calls the Use method via vtable).
        /// </summary>
        public void Use()
        {
            ExecutorRand? executor = ObjectManager.Executor;
            if (executor == null)
                throw new InvalidOperationException("Invalid executor in WoWSubObject.Use");

            lock (executor.AssemblyLock)
            {
                executor.Clear();
                executor.AddLine($"mov ecx, {BaseAddress}");
                executor.AddLine("mov eax, [ecx]");
                executor.AddLine("mov eax, [eax+36]"); // Offset +36 pour vtable Use
                executor.AddLine("call eax");
                executor.AddLine("retn");
                executor.Execute();
            }
        }

        /// <summary>
        /// Builds the assembler for CanUseNow.
        /// Calls the virtual method CanUseNow via vtable (+28).
        /// </summary>
        private void BuildCanUseNowAsm(ExecutorRand executor, uint reason, uint interactDistance, uint a4)
        {
            executor.AddLine($"mov ecx, {BaseAddress}");
            executor.AddLine("mov eax, [ecx]");
            executor.AddLine("mov eax, [eax+28]"); // Offset +28 pour vtable CanUseNow
            executor.AddLine($"push {a4}");
            executor.AddLine($"push {interactDistance}");
            executor.AddLine($"push {reason}");
            executor.AddLine("call eax");
        }
    }
}
