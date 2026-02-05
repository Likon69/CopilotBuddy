// MathEx.cs - Ported from HB 4.3.4/5.4.8
// Common math utilities

using System;

namespace Styx.Common
{
    /// <summary>
    /// Extended math utilities.
    /// Ported from HB 4.3.4/5.4.8
    /// </summary>
    public static class MathEx
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Returns a random double between min and max.
        /// </summary>
        public static double Random(double min, double max)
        {
            return min + _random.NextDouble() * (max - min);
        }

        /// <summary>
        /// Returns a random int between min and max (exclusive).
        /// </summary>
        public static int Random(int min, int max)
        {
            return _random.Next(min, max);
        }

        /// <summary>
        /// Linear interpolation between min and max.
        /// </summary>
        public static float Lerp(float min, float max, float amount)
        {
            return min + (max - min) * amount;
        }

        /// <summary>
        /// Inverse linear interpolation.
        /// </summary>
        public static float InverseLerp(float min, float max, float amount)
        {
            return (amount - min) / (max - min);
        }

        /// <summary>
        /// Get the amount (0-1) of value between min and max.
        /// </summary>
        public static float GetAmount(float min, float max, float value)
        {
            return (value - min) / (max - min);
        }

        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        public static float ToRadians(float degrees)
        {
            return (float)(degrees * Math.PI / 180.0);
        }

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        public static float ToDegrees(float radians)
        {
            return (float)(radians * 180.0 / Math.PI);
        }

        /// <summary>
        /// Clamp a value between min and max.
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
            if (value <= min)
                return min;
            if (value >= max)
                return max;
            return value;
        }

        /// <summary>
        /// Clamp a value between min and max.
        /// </summary>
        public static int Clamp(int value, int min, int max)
        {
            if (value <= min)
                return min;
            if (value >= max)
                return max;
            return value;
        }

        /// <summary>
        /// Clamp a value between min and max.
        /// </summary>
        public static double Clamp(double value, double min, double max)
        {
            if (value <= min)
                return min;
            if (value >= max)
                return max;
            return value;
        }

        /// <summary>
        /// Wrap an angle in radians to [-PI, PI].
        /// </summary>
        public static float WrapAngle(float radian)
        {
            double normalizedRadian = Math.IEEERemainder(radian, Math.PI * 2.0);
            if (normalizedRadian <= -Math.PI)
                normalizedRadian += Math.PI * 2.0;
            else if (normalizedRadian > Math.PI)
                normalizedRadian -= Math.PI * 2.0;
            return (float)normalizedRadian;
        }

        /// <summary>
        /// Check if a point is on a line segment.
        /// </summary>
        public static bool IsOnSegment(double xi, double yi, double xj, double yj, double xk, double yk)
        {
            return (xi <= xk || xj <= xk) && (xk <= xi || xk <= xj) && 
                   (yi <= yk || yj <= yk) && (yk <= yi || yk <= yj);
        }

        /// <summary>
        /// Compute direction of three points.
        /// </summary>
        public static int ComputeDirection(double xi, double yi, double xj, double yj, double xk, double yk)
        {
            double crossProduct1 = (xk - xi) * (yj - yi);
            double crossProduct2 = (xj - xi) * (yk - yi);
            if (crossProduct1 < crossProduct2)
                return -1;
            if (crossProduct1 <= crossProduct2)
                return 0;
            return 1;
        }

        /// <summary>
        /// Check if two line segments intersect.
        /// </summary>
        public static bool DoLineSegmentsIntersect(double x1, double y1, double x2, double y2, 
                                                    double x3, double y3, double x4, double y4)
        {
            int d1 = ComputeDirection(x3, y3, x4, y4, x1, y1);
            int d2 = ComputeDirection(x3, y3, x4, y4, x2, y2);
            int d3 = ComputeDirection(x1, y1, x2, y2, x3, y3);
            int d4 = ComputeDirection(x1, y1, x2, y2, x4, y4);

            return (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && 
                    ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0))) ||
                   (d1 == 0 && IsOnSegment(x3, y3, x4, y4, x1, y1)) ||
                   (d2 == 0 && IsOnSegment(x3, y3, x4, y4, x2, y2)) ||
                   (d3 == 0 && IsOnSegment(x1, y1, x2, y2, x3, y3)) ||
                   (d4 == 0 && IsOnSegment(x1, y1, x2, y2, x4, y4));
        }
    }
}
