using System;
using GreenMagic;
using Styx.Helpers;
using Styx.Patchables;

namespace Styx.WoWInternals.WoWCache
{
    /// <summary>
    /// Cache pour les items de la base de données WoW.
    /// WoW 3.3.5a build 12340.
    /// </summary>
    public static class DBItemCache
    {

        /// <summary>
        /// Retrieves an item info block from the DB cache.
        /// Calls the native WoW GetItemInfoBlock function.
        /// </summary>
        /// <param name="caller">Caller pointer (typically the DB cache)</param>
        /// <param name="index">Index/ID de l'item</param>
        /// <param name="a3">Reference parameter (modified by the function)</param>
        /// <param name="a4">Parameter 4</param>
        /// <param name="a5">Parameter 5</param>
        /// <param name="a6">Parameter 6</param>
        /// <returns>Address of the info block, or 0 if not found</returns>
        public static uint GetInfoBlockByID(uint caller, uint index, ref int a3, int a4, int a5, int a6)
        {
            ExecutorRand? executor = ObjectManager.Executor;
            if (executor == null)
                throw new InvalidOperationException("Invalid Executor used in GetInfoBlockByID");

            lock (executor.AssemblyLock)
            {
                uint paramPtr = executor.Memory.AllocateMemory(4);
                executor.Memory.Write(paramPtr, a3);

                try
                {
                    executor.Clear();
                    executor.AddLine($"push {a6}");
                    executor.AddLine($"push {a5}");
                    executor.AddLine($"push {a4}");
                    executor.AddLine($"push {paramPtr}");
                    executor.AddLine($"push {index}");
                    executor.AddLine($"mov ecx, {caller}");
                    executor.AddLine($"call {GlobalOffsets.DBItemCache_GetInfoBlockByID}");
                    executor.AddLine("retn");
                    executor.Execute();

                    // Disable cache while reading results of the remote call
                    uint result;
                    using (StyxWoW.Memory.TemporaryCacheState(false))
                    {
                        // Read the modified parameter
                        a3 = executor.Memory.Read<int>(paramPtr);
                        // Read the result
                        result = executor.Memory.Read<uint>(executor.ReturnPointer);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Logging.WriteDebug($"Exception in GetInfoBlockByID: {ex.Message} - {ex.StackTrace} - {ex.Source}");
                    return 0;
                }
                finally
                {
                    if (paramPtr != 0)
                        executor.Memory.FreeMemory(paramPtr);
                }
            }
        }
    }
}
