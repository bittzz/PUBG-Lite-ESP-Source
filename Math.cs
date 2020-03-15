using System;
using System.Runtime.InteropServices;

namespace PUBG_Lite_test
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
        }

    }

    public struct Matrix
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public Matrix(Vector3 rot)
        {
            float radPitch = (rot.X * (float)Math.PI / 180.0f);
            float radYaw = (rot.Y * (float)Math.PI / 180.0f);
            float radRoll = (rot.Z * (float)Math.PI / 180.0f);

            float SP = (float)Math.Sin((double)radPitch);
            float CP = (float)Math.Cos((double)radPitch);
            float SY = (float)Math.Sin((double)radYaw);
            float CY = (float)Math.Cos((double)radYaw);
            float SR = (float)Math.Sin((double)radRoll);
            float CR = (float)Math.Cos((double)radRoll);

            M11 = CP * CY;
            M12 = CP * SY;
            M13 = SP;
            M14 = 0.0f;

            M21 = SR * SP * CY - CR * SY;
            M22 = SR * SP * SY + CR * CY;
            M23 = -SR * CP;
            M24 = 0.0f;

            M31 = -(CR * SP * CY + SR * SY);
            M32 = CY * SR - CR * SP * SY;
            M33 = CR * CP;
            M34 = 0.0f;

            M41 = 0;
            M42 = 0;
            M43 = 0;
            M44 = 1.0f;
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            Matrix result;

            // First row
            result.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
            result.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
            result.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
            result.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

            // Second row
            result.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
            result.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
            result.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
            result.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

            // Third row
            result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
            result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
            result.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
            result.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

            // Fourth row
            result.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
            result.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
            result.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
            result.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;

            return result;
        }
    }

    public struct FQuat
    {
        public float X;
        public float Y;
        public float Z;
        public float W;
    };

    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public struct FTransform
    {
        [FieldOffset(0)]
        public FQuat Rotation;
        [FieldOffset(16)]
        public Vector3 Translation;
        [FieldOffset(32)]
        public Vector3 Scale3D;

        public Matrix ToMatrixWithScale()
        {
            Matrix m = new Matrix();

            m.M41 = Translation.X;
            m.M42 = Translation.Y;
            m.M43 = Translation.Z;

            float x2 = Rotation.X + Rotation.X;
            float y2 = Rotation.Y + Rotation.Y;
            float z2 = Rotation.Z + Rotation.Z;

            float xx2 = Rotation.X * x2;
            float yy2 = Rotation.Y * y2;
            float zz2 = Rotation.Z * z2;
            m.M11 = (1.0f - (yy2 + zz2)) * Scale3D.X;
            m.M22 = (1.0f - (xx2 + zz2)) * Scale3D.Y;
            m.M33 = (1.0f - (xx2 + yy2)) * Scale3D.Z;


            float yz2 = Rotation.Y * z2;
            float wx2 = Rotation.W * x2;
            m.M32 = (yz2 - wx2) * Scale3D.Z;
            m.M23 = (yz2 + wx2) * Scale3D.Y;


            float xy2 = Rotation.X * y2;
            float wz2 = Rotation.W * z2;
            m.M21 = (xy2 - wz2) * Scale3D.Y;
            m.M12 = (xy2 + wz2) * Scale3D.X;


            float xz2 = Rotation.X * z2;
            float wy2 = Rotation.W * y2;
            m.M31 = (xz2 + wy2) * Scale3D.Z;
            m.M13 = (xz2 - wy2) * Scale3D.X;

            m.M14 = 0.0f;
            m.M24 = 0.0f;
            m.M34 = 0.0f;
            m.M44 = 1.0f;

            return m;
        }
    };
}
