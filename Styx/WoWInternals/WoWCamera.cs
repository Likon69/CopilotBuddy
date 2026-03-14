using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Styx.Logic.Pathing;

#nullable disable
namespace Styx.WoWInternals
{
    /// <summary>
    /// Provides access to the WoW camera for position, orientation, and projection data.
    /// Note: Camera offsets for 3.3.5a need to be verified. This is a basic implementation.
    /// </summary>
    public class WoWCamera
    {
        // Camera struct offset in WoW 3.3.5a client
        // These need verification - using known working offsets
        private static readonly uint CameraPtr = 0x00B7436C;  // Pointer to world frame
        private static readonly uint CameraOffset = 0x7E20;   // Offset to camera struct

        internal WoWCamera()
        {
        }

        /// <summary>
        /// Gets the camera position in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                try
                {
                    var data = GetCameraStruct();
                    return data.Position;
                }
                catch
                {
                    // Fallback to player position if camera read fails
                    if (StyxWoW.Me != null)
                    {
                        var loc = StyxWoW.Me.Location;
                        return new Vector3(loc.X, loc.Y, loc.Z + 2f);
                    }
                    return Vector3.Zero;
                }
            }
        }

        /// <summary>
        /// Gets the camera field of view in radians.
        /// </summary>
        public float FieldOfView
        {
            get
            {
                try
                {
                    return GetCameraStruct().FieldOfView;
                }
                catch
                {
                    return 0.785398f; // 45 degrees default
                }
            }
        }

        /// <summary>
        /// Gets the near clipping plane distance.
        /// </summary>
        public float NearZ
        {
            get
            {
                try
                {
                    return GetCameraStruct().NearZ;
                }
                catch
                {
                    return 0.33333f; // Default
                }
            }
        }

        /// <summary>
        /// Gets the far clipping plane distance.
        /// </summary>
        public float FarZ
        {
            get
            {
                try
                {
                    return GetCameraStruct().FarZ;
                }
                catch
                {
                    return 1277.77f; // Default
                }
            }
        }

        /// <summary>
        /// Gets the aspect ratio (width/height).
        /// </summary>
        public float Aspect
        {
            get
            {
                try
                {
                    return GetCameraStruct().Aspect;
                }
                catch
                {
                    return 1.333f; // 4:3 default
                }
            }
        }

        /// <summary>
        /// Gets the projection matrix.
        /// </summary>
        public Matrix4x4 Projection
        {
            get
            {
                return CreatePerspectiveFieldOfView(
                    FieldOfView * 0.6f,
                    Aspect,
                    NearZ,
                    FarZ);
            }
        }

        /// <summary>
        /// Gets the world matrix (identity).
        /// </summary>
        public Matrix4x4 World => Matrix4x4.Identity;

        /// <summary>
        /// Gets the view matrix for rendering.
        /// </summary>
        public Matrix4x4 View
        {
            get
            {
                Vector3 position = Position;
                Vector3 target = position + Forward;
                return CreateLookAt(position, target, new Vector3(0f, 0f, 1f));
            }
        }

        /// <summary>
        /// Gets the camera forward direction vector.
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                Matrix4x4 matrix = Matrix;
                return new Vector3(matrix.M11, matrix.M12, matrix.M13);
            }
        }

        /// <summary>
        /// Gets the camera right direction vector.
        /// </summary>
        public Vector3 Right
        {
            get
            {
                Matrix4x4 matrix = Matrix;
                return new Vector3(matrix.M21, matrix.M22, matrix.M23);
            }
        }

        /// <summary>
        /// Gets the camera up direction vector.
        /// </summary>
        public Vector3 Up
        {
            get
            {
                Matrix4x4 matrix = Matrix;
                return new Vector3(matrix.M31, matrix.M32, matrix.M33);
            }
        }

        /// <summary>
        /// Gets the camera orientation matrix.
        /// </summary>
        public Matrix4x4 Matrix
        {
            get
            {
                try
                {
                    var data = GetCameraStruct();
                    return new Matrix4x4(
                        data.Matrix.M00, data.Matrix.M01, data.Matrix.M02, 0f,
                        data.Matrix.M10, data.Matrix.M11, data.Matrix.M12, 0f,
                        data.Matrix.M20, data.Matrix.M21, data.Matrix.M22, 0f,
                        0f, 0f, 0f, 1f);
                }
                catch
                {
                    return Matrix4x4.Identity;
                }
            }
        }

        /// <summary>
        /// Converts a 3D world position to 2D screen coordinates.
        /// </summary>
        /// <param name="worldPos">The world position to convert.</param>
        /// <param name="screenX">Output screen X coordinate (0-1).</param>
        /// <param name="screenY">Output screen Y coordinate (0-1).</param>
        /// <returns>True if the point is on screen, false if behind the camera.</returns>
        public bool WorldToScreen(WoWPoint worldPos, out float screenX, out float screenY)
        {
            screenX = 0;
            screenY = 0;

            Vector3 pos = new Vector3(worldPos.X, worldPos.Y, worldPos.Z);
            Matrix4x4 viewProj = View * Projection;

            // Transform world position by view-projection matrix
            Vector4 clipSpace = Vector4.Transform(new Vector4(pos, 1f), viewProj);

            // Check if behind camera
            if (clipSpace.W <= 0)
                return false;

            // Perspective divide
            float x = clipSpace.X / clipSpace.W;
            float y = clipSpace.Y / clipSpace.W;

            // Convert from [-1,1] to [0,1]
            screenX = (x + 1f) * 0.5f;
            screenY = (1f - y) * 0.5f;

            return screenX >= 0 && screenX <= 1 && screenY >= 0 && screenY <= 1;
        }

        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        private static Matrix4x4 CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
        {
            float yScale = 1.0f / (float)Math.Tan(fieldOfView * 0.5f);
            float xScale = yScale / aspectRatio;
            float zRange = farPlane / (nearPlane - farPlane);

            return new Matrix4x4(
                xScale, 0, 0, 0,
                0, yScale, 0, 0,
                0, 0, zRange, -1,
                0, 0, zRange * nearPlane, 0);
        }

        /// <summary>
        /// Creates a view matrix looking at a target.
        /// </summary>
        private static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            Vector3 zAxis = Vector3.Normalize(cameraPosition - cameraTarget);
            Vector3 xAxis = Vector3.Normalize(Vector3.Cross(cameraUpVector, zAxis));
            Vector3 yAxis = Vector3.Cross(zAxis, xAxis);

            return new Matrix4x4(
                xAxis.X, yAxis.X, zAxis.X, 0,
                xAxis.Y, yAxis.Y, zAxis.Y, 0,
                xAxis.Z, yAxis.Z, zAxis.Z, 0,
                -Vector3.Dot(xAxis, cameraPosition), -Vector3.Dot(yAxis, cameraPosition), -Vector3.Dot(zAxis, cameraPosition), 1);
        }

        private CameraData GetCameraStruct()
        {
            if (!ObjectManager.IsInGame)
                return new CameraData();

            try
            {
                uint worldFrame = ObjectManager.Wow.ReadRelative<uint>(CameraPtr);
                if (worldFrame == 0)
                    return new CameraData();

                uint cameraAddress = ObjectManager.Wow.Read<uint>(worldFrame + CameraOffset);
                if (cameraAddress == 0)
                    return new CameraData();

                return ObjectManager.Wow.ReadStruct<CameraData>(cameraAddress);
            }
            catch
            {
                return new CameraData();
            }
        }

        /// <summary>
        /// Internal camera data structure for 3.3.5a.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CameraData
        {
            public uint VTable;
            public uint Dword4;
            public Vector3 Position;
            public CameraMatrix3x3 Matrix;
            public float FieldOfView;
            public uint Model;
            public int Timestamp;
            public float NearZ;
            public float FarZ;
            public float Aspect;
        }

        /// <summary>
        /// 3x3 rotation matrix for camera orientation.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CameraMatrix3x3
        {
            public float M00, M01, M02;
            public float M10, M11, M12;
            public float M20, M21, M22;
        }
    }
}
