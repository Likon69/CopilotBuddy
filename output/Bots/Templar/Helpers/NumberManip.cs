using System;

namespace Templar.Helpers {
    public class NumberManip {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        private static readonly Random Rand = new Random();

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static double RoundToDigit(double input, double digit) {
            return Math.Round(input / digit, MidpointRounding.AwayFromZero) * digit;
        }

        /*
	     * Returns a psuedo-random number between min and max, inclusive. The
	     * difference between min and max can be at most
	     * <code>Integer.MAX_VALUE - 1</code>.
	     */
        public static int RandomInt(int pMinVal, int pMaxVal) {
            // Next is normally exclusive of the top value (pMaxVal), so add 1 to make it inclusive
            return Rand.Next((pMaxVal - pMinVal) + 1) + pMinVal;
        }

        public static float RandomFloat(float pMinVal, float pMaxVal) {
            var randomFloat = RandomInt((int)(pMinVal * 100), (int)(pMaxVal * 100));

            return randomFloat / 100.0f;
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}