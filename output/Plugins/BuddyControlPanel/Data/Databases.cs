// Originally contributed by Chinajade
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 4.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/4.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.
//

#region Usings

using System;
using System.Collections.Generic;

using Styx;
using Styx.Database;

// ReSharper disable RedundantLambdaSignatureParentheses
#endregion

namespace BuddyControlPanel
{
    public static class Databases
    {
        public static readonly List<Func<NpcResult, bool>> ConditionallyBlacklistedInnkeepers = new List<Func<NpcResult, bool>>()
        {
			// Classic: Nethergard Keep Innkeeper, if we're not in the past...
			(npcResult) =>
            {
                return (npcResult.Entry == 44325 /*Mama Morton*/) && (!StyxWoW.Me.HasAura(176111 /*TimeTravel*/));
            },

			// Classic: Surwich Innkeeper, if we're not in the past...
			(npcResult) =>
            {
                return (npcResult.Entry == 44334 /*Donna Berrymore*/) && (!StyxWoW.Me.HasAura(176111 /*TimeTravel*/));
            },

			// BC: Telredor Innkeep presents too much problems...
			(npcResult) =>
            {
                return (npcResult.Entry == 18251 /*Caregiver Abidaar*/);
            }
        };
    }
}
