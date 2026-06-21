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
using System.Diagnostics;
using System.Linq;
using System.Numerics;

// using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.CommonBot;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.World;
using Styx.WoWInternals.WoWObjects;
using Styx.Common;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    // By its nature, only static elements belong in the Utility class...
    public static class FlightorMonitor
    {
        /// <summary>
        /// <para>Returns 'true', if Flightor is stuck in one of its back-and-forth patterns,
        /// because it can't figure out how to get to destination.  This is not the same thing
        /// as a 'stuck', because Flightor recovers from stucks just fine.</para>
        /// <para>Once we declare Flightor as 'confused', we'll maintain the assertion
        /// for a few seconds.  This provides hysteresis so the caller will prefer
        /// ground travel for a bit.</para>
        /// </summary>
        /// <returns></returns>
        public static bool IsFlightorConfused()
        {
            // Confusion only happen when flying...
            if (!StyxWoW.Me.IsFlying)
            {
                Clear();
                return false;
            }

            CaptureSample();

            // If not enough information to make 'confused' determination, assume we're good...
            if (s_sampleCount < Tunable_MinSampleThreshold)
                return false;

            // If we're too close to our average position, then Flightor is confused...
            return Vector3.Distance(StyxWoW.Me.Location, s_averagePosition) <= Tunable_DistanceToAverageForConfusionDeclaration;
        }

        private static readonly Stopwatch s_sampleTimer = new Stopwatch();
        private static readonly TimeSpan s_sampleTime = TimeSpan.FromMilliseconds(1000);

        private const int Tunable_MinSampleThreshold = 7;
        private const double Tunable_DistanceToAverageForConfusionDeclaration = 10.0;

        private static Vector3 s_averagePosition;
        private static int s_sampleCount;

        private static void Clear()
        {
            s_sampleTimer.Reset();
            s_averagePosition = Vector3.Zero;
            s_sampleCount = 0;
        }

        private static void CaptureSample()
        {
            // Place upper bound on our sampling interval...
            if (s_sampleTimer.IsRunning && (s_sampleTimer.Elapsed <= s_sampleTime))
                return;

            // N.B.: We use low-pass filter rather than an actual average for this...
            // This makes detection much more responsive, as we don't care about where we were
            // >60 seconds ago to determine if we're gummed up.
            const float lowpassFilterCoefficient = 0.75f; // [0.0..1.0]
            var myLocation = StyxWoW.Me.Location;

            if (s_sampleCount == 0)
                s_averagePosition = myLocation;

            s_averagePosition.X = (lowpassFilterCoefficient * s_averagePosition.X)
                                + ((1 - lowpassFilterCoefficient) * myLocation.X);

            s_averagePosition.Y = (lowpassFilterCoefficient * s_averagePosition.Y)
                                + ((1 - lowpassFilterCoefficient) * myLocation.Y);

            s_averagePosition.Z = (lowpassFilterCoefficient * s_averagePosition.Z)
                                + ((1 - lowpassFilterCoefficient) * myLocation.Z);

            ++s_sampleCount;
            s_sampleTimer.Restart();
        }
    }
}