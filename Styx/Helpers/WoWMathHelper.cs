using System;
using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Helpers
{
    public static class WoWMathHelper
    {
        public const float PI = 3.14159274f;
        public const float TwoPI = 6.28318548f;
        public const float HalfPI = 1.57079637f;
        
        public static float DegreesToRadians(float degrees)
        {
            return (float)(degrees * 0.017453292519943295);
        }
        
        public static float RadiansToDegrees(float radians)
        {
            return (float)(radians * 57.295779513082323);
        }
        
        public static bool IsBehind(WoWPoint me, WoWPoint target, float targetFacingRadians)
        {
            return IsBehind(me, target, targetFacingRadians, PI);
        }
        
        public static bool IsBehind(WoWUnit me, WoWUnit target)
        {
            return me != null && target != null && IsBehind(me.Location, target.Location, target.Rotation);
        }
        
        public static bool IsSafelyBehind(WoWPoint me, WoWPoint target, float targetFacingRadians)
        {
            return IsBehind(me, target, targetFacingRadians, DegreesToRadians(260f));
        }
        
        public static bool IsBehind(WoWPoint me, WoWPoint target, float targetFacingRadians, float arcRadians)
        {
            // Vector from target to me (2D, ignore Z)
            float vx = me.X - target.X;
            float vy = me.Y - target.Y;
            
            // Direction vector the target is facing
            float fx = (float)Math.Cos(targetFacingRadians);
            float fy = (float)Math.Sin(targetFacingRadians);
            
            // Dot product
            float dot = fx * vx + fy * vy;
            
            // Magnitudes
            float vMag = (float)Math.Sqrt(vx * vx + vy * vy);
            float fMag = (float)Math.Sqrt(fx * fx + fy * fy);
            
            if (vMag * fMag == 0) return false;
            
            // Angle between vectors (clamped to [-1, 1] for Acos)
            float cosAngle = Clamp(dot / (vMag * fMag), -1.0f, 1.0f);
            float angle = (float)Math.Acos(cosAngle);
            
            // Behind if angle >= half arc (target not facing us)
            return angle >= arcRadians / 2f;
        }
        
        public static bool IsFacing(WoWPoint me, float myFacingRadians, WoWPoint target)
        {
            return IsFacing(me, myFacingRadians, target, PI);
        }
        
        public static bool IsSafelyFacing(WoWPoint me, float myFacingRadians, WoWPoint target)
        {
            return IsFacing(me, myFacingRadians, target, DegreesToRadians(100f));
        }
        
        public static bool IsFacing(WoWPoint me, float myFacingRadians, WoWPoint target, float arcRadians)
        {
            // Vector from me to target (2D, ignore Z)
            float vx = target.X - me.X;
            float vy = target.Y - me.Y;
            
            // Direction vector I'm facing
            float fx = (float)Math.Cos(myFacingRadians);
            float fy = (float)Math.Sin(myFacingRadians);
            
            // Dot product
            float dot = fx * vx + fy * vy;
            
            // Magnitudes
            float vMag = (float)Math.Sqrt(vx * vx + vy * vy);
            float fMag = (float)Math.Sqrt(fx * fx + fy * fy);
            
            if (vMag * fMag == 0) return true;
            
            // Angle between vectors (clamped to [-1, 1] for Acos)
            float cosAngle = Clamp(dot / (vMag * fMag), -1.0f, 1.0f);
            float angle = (float)Math.Acos(cosAngle);
            
            // Facing if angle <= half arc
            return angle <= arcRadians / 2f;
        }
        
        private static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        
        public static float CalculateNeededFacing(WoWPoint start, WoWPoint faceTarget)
        {
            return NormalizeRadian((float)Math.Atan2(faceTarget.Y - start.Y, faceTarget.X - start.X));
        }
        
        public static WoWPoint CalculatePointBehind(WoWPoint target, float targetFacingRadians, float distanceToTarget)
        {
            targetFacingRadians = NormalizeRadian(targetFacingRadians);
            
            var direction = new WoWPoint
            {
                X = -(float)Math.Cos(targetFacingRadians),
                Y = (float)Math.Sin(targetFacingRadians + HalfPI),
                Z = 0f
            };
            
            return target - direction * distanceToTarget;
        }
        
        public static WoWPoint CalculatePointAtSide(WoWPoint target, float targetFacingInRadians, float distanceToTarget, bool rightSide)
        {
            if (rightSide)
            {
                targetFacingInRadians += HalfPI;
            }
            else
            {
                targetFacingInRadians += HalfPI * 3f;
            }
            
            return CalculatePointBehind(target, targetFacingInRadians, distanceToTarget);
        }
        
        public static WoWPoint GetPointAt(WoWPoint from, float distance, float rotationRadians, float pitchRadians)
        {
            float x = (float)(Math.Cos(pitchRadians) * Math.Cos(rotationRadians) * distance);
            float y = (float)(Math.Cos(pitchRadians) * Math.Sin(rotationRadians) * distance);
            float z = (float)(Math.Sin(pitchRadians) * distance);

            return from + new WoWPoint(x, y, z);
        }

        public static WoWPoint CalculatePointFrom(WoWPoint from, WoWPoint target, float distance)
        {
            var direction = new WoWPoint
            {
                X = target.X - from.X,
                Y = target.Y - from.Y,
                Z = target.Z - from.Z
            };
            
            float length = (float)Math.Sqrt(
                direction.X * direction.X +
                direction.Y * direction.Y +
                direction.Z * direction.Z
            );
            
            if (length == 0) return target;
            
            return new WoWPoint
            {
                X = target.X - distance / length * direction.X,
                Y = target.Y - distance / length * direction.Y,
                Z = target.Z - distance / length * direction.Z
            };
        }
        
        public static float NormalizeRadian(float radian)
        {
            while (radian < 0f)
            {
                radian += TwoPI;
            }
            while (radian >= TwoPI)
            {
                radian -= TwoPI;
            }
            return radian;
        }
        
        public static float GetDistance2D(WoWPoint p1, WoWPoint p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        
        public static float GetDistance3D(WoWPoint p1, WoWPoint p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float dz = p2.Z - p1.Z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static bool IsInPath(WoWUnit unit, WoWPoint start, WoWPoint destination)
        {
            if (unit == null || unit.Dead)
                return false;

            WoWPoint center = unit.Location;
            float radius = unit.MyAggroRange;

            float dx = destination.X - start.X;
            float dy = destination.Y - start.Y;
            float dz = destination.Z - start.Z;

            float segmentLength = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (segmentLength <= 0f)
                return false;

            float invLen = 1f / segmentLength;
            float dirX = dx * invLen;
            float dirY = dy * invLen;
            float dirZ = dz * invLen;

            float mx = start.X - center.X;
            float my = start.Y - center.Y;
            float mz = start.Z - center.Z;

            float b = mx * dirX + my * dirY + mz * dirZ;
            float c = mx * mx + my * my + mz * mz - radius * radius;

            if (c > 0f && b > 0f)
                return false;

            float discriminant = b * b - c;
            if (discriminant < 0f)
                return false;

            float t = -b - (float)Math.Sqrt(discriminant);
            if (t < 0f)
                t = 0f;

            return t < segmentLength;
        }

        public static bool IsInPath(IEnumerable<WoWUnit> units, WoWPoint destination)
        {
            LocalPlayer me = StyxWoW.Me;
            if (me == null)
                return false;
            WoWPoint start = me.Location;
            return units.Any(u => IsInPath(u, start, destination));
        }

        public static WoWUnit? GetClosestInPath(IEnumerable<WoWUnit> units, WoWPoint destination)
        {
            LocalPlayer me = StyxWoW.Me;
            if (me == null)
                return null;
            WoWPoint start = me.Location;

            return units
                .Where(u => IsInPath(u, start, destination))
                .OrderBy(u => u.DistanceSqr)
                .FirstOrDefault();
        }
    }
}
