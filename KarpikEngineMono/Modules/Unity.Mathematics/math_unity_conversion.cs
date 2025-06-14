#if !UNITY_DOTSPLAYER
using Microsoft.Xna.Framework;

#pragma warning disable 0660, 0661

namespace Unity.Mathematics
{
    public partial struct float2
    {
        /// <summary>
        /// Converts a float2 to Vector2.
        /// </summary>
        /// <param name="v">float2 to convert.</param>
        /// <returns>The converted Vector2.</returns>
        public static implicit operator Vector2(float2 v)     { return new Vector2(v.x, v.y); }

        /// <summary>
        /// Converts a Vector2 to float2.
        /// </summary>
        /// <param name="v">Vector2 to convert.</param>
        /// <returns>The converted float2.</returns>
        public static implicit operator float2(Vector2 v)     { return new float2(v.X, v.Y); }
    }

    public partial struct float3
    {
        /// <summary>
        /// Converts a float3 to Vector3.
        /// </summary>
        /// <param name="v">float3 to convert.</param>
        /// <returns>The converted Vector3.</returns>
        public static implicit operator Vector3(float3 v)     { return new Vector3(v.x, v.y, v.z); }

        /// <summary>
        /// Converts a Vector3 to float3.
        /// </summary>
        /// <param name="v">Vector3 to convert.</param>
        /// <returns>The converted float3.</returns>
        public static implicit operator float3(Vector3 v)     { return new float3(v.X, v.Y, v.Z); }
    }

    public partial struct float4
    {
        /// <summary>
        /// Converts a Vector4 to float4.
        /// </summary>
        /// <param name="v">Vector4 to convert.</param>
        /// <returns>The converted float4.</returns>
        public static implicit operator float4(Vector4 v)     { return new float4(v.X, v.Y, v.Z, v.W); }

        /// <summary>
        /// Converts a float4 to Vector4.
        /// </summary>
        /// <param name="v">float4 to convert.</param>
        /// <returns>The converted Vector4.</returns>
        public static implicit operator Vector4(float4 v)     { return new Vector4(v.x, v.y, v.z, v.w); }
    }

    public partial struct quaternion
    {
        /// <summary>
        /// Converts a quaternion to Quaternion.
        /// </summary>
        /// <param name="q">quaternion to convert.</param>
        /// <returns>The converted Quaternion.</returns>
        public static implicit operator Quaternion(quaternion q)  { return new Quaternion(q.value.x, q.value.y, q.value.z, q.value.w); }

        /// <summary>
        /// Converts a Quaternion to quaternion.
        /// </summary>
        /// <param name="q">Quaternion to convert.</param>
        /// <returns>The converted quaternion.</returns>
        public static implicit operator quaternion(Quaternion q)  { return new quaternion(q.X, q.Y, q.Z, q.W); }
    }

    public partial struct float4x4
    {
        /// <summary>
        /// Converts a Matrix4x4 to float4x4.
        /// </summary>
        /// <param name="m">Matrix4x4 to convert.</param>
        /// <returns>The converted float4x4.</returns>
        public static implicit operator float4x4(Matrix m) { return new float4x4(
            new float4(m.M11, m.M12, m.M13, m.M14),
            new float4(m.M21, m.M22, m.M23, m.M24),
            new float4(m.M31, m.M32, m.M33, m.M34),
            new float4(m.M41, m.M42, m.M43, m.M44)); }

        /// <summary>
        /// Converts a float4x4 to Matrix4x4.
        /// </summary>
        /// <param name="m">float4x4 to convert.</param>
        /// <returns>The converted Matrix4x4.</returns>
        public static implicit operator Matrix(float4x4 m) { return new Matrix(m.c0, m.c1, m.c2, m.c3); }
    }
}
#endif
