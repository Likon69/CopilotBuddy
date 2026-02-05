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
        /// Récupère un bloc d'information item depuis le cache DB.
        /// Appelle la fonction native WoW GetItemInfoBlock.
        /// </summary>
        /// <param name="caller">Pointeur caller (généralement le cache DB)</param>
        /// <param name="index">Index/ID de l'item</param>
        /// <param name="a3">Paramètre de référence (modifié par la fonction)</param>
        /// <param name="a4">Paramètre 4</param>
        /// <param name="a5">Paramètre 5</param>
        /// <param name="a6">Paramètre 6</param>
        /// <returns>Adresse du bloc d'information, ou 0 si non trouvé</returns>
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

                    // Lit le paramètre modifié
                    a3 = executor.Memory.Read<int>(paramPtr);

                    // Lit le résultat
                    return executor.Memory.Read<uint>(executor.ReturnPointer);
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
