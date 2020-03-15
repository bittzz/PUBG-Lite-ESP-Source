using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace PUBG_Lite_test
{
    public static class Offset
    {
        public const int oUWorld = 0x45342F8;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct UWorld
    {
        [FieldOffset(840)]
        private IntPtr encryptedCurrentLevelPtr; //0x0348

        [FieldOffset(2712)]
        private IntPtr encryptedOwningGameInstancePtr; //0x0A98

        public ULevel CurrentLevel()
        {
            IntPtr CurrentLevelPtr = Decrypt.ULevel(encryptedCurrentLevelPtr);
            return Driver.Read<ULevel>(CurrentLevelPtr);
        }

        public UGameInstance OwningGameInstance()
        {
            IntPtr GameInstancePtr = Decrypt.GameInstance(encryptedOwningGameInstancePtr);
            return Driver.Read<UGameInstance>(GameInstancePtr);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct UGameInstance
    {
        [FieldOffset(336)]
        private IntPtr localPlayersPtr; //0x0150

        public ULocalPlayers LocalPlayers()
        {
            return Driver.Read<ULocalPlayers>(localPlayersPtr);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct ULocalPlayers
    {
        [FieldOffset(0)]
        private IntPtr encryptedLocalPlayerPtr; //0x0000

        public ULocalPlayer LocalPlayer()
        {
            IntPtr LocalPlayerPtr = Decrypt.LocalPlayer(encryptedLocalPlayerPtr);
            return Driver.Read<ULocalPlayer>(LocalPlayerPtr);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct ULocalPlayer
    {
        [FieldOffset(56)]
        private IntPtr EncryptedPlayerConstrollerPtr; //0x0038

        public APlayerController PlayerController()
        {
            IntPtr PlayerControllerPtr = Decrypt.PlayerController(EncryptedPlayerConstrollerPtr);
            return Driver.Read<APlayerController>(PlayerControllerPtr);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct APlayerController
    {
        [FieldOffset(1096)]
        private IntPtr encryptedAknowledgedPawnPtr; //0x0448

        [FieldOffset(1128)]
        private IntPtr playerCameraManagerPtr; //0x0468

        public AActor AknowledgedPawn()
        {
            IntPtr AknowledgedPawnPtr = Decrypt.InPawn(encryptedAknowledgedPawnPtr);
            return Driver.Read<AActor>(AknowledgedPawnPtr);
        }

        public APlayerCameraManager PlayerCameraManager()
        {
            return Driver.Read<APlayerCameraManager>(playerCameraManagerPtr);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct APlayerCameraManager
    {
        [FieldOffset(960)]
        private FCameraCacheEntry CameraCache; //0x03C0

        public bool WorldToScreen(Vector3 WorldLocation, Vector2 screenSize, out Vector2 Screenlocation)
        {
            Matrix tempMatrix = new Matrix(CameraCache.POV.Rotation);

            Vector3 vAxisX, vAxisY, vAxisZ;

            vAxisX = new Vector3(tempMatrix.M11, tempMatrix.M12, tempMatrix.M13);
            vAxisY = new Vector3(tempMatrix.M21, tempMatrix.M22, tempMatrix.M23);
            vAxisZ = new Vector3(tempMatrix.M31, tempMatrix.M32, tempMatrix.M33);

            Vector3 vDelta = WorldLocation - CameraCache.POV.Location;
            Vector3 vTransformed = new Vector3(Vector3.Dot(vDelta, vAxisY), Vector3.Dot(vDelta, vAxisZ), Vector3.Dot(vDelta, vAxisX));

            if (vTransformed.Z < 1.0f)
                vTransformed.Z = 1.0f;

            float FovAngle = CameraCache.POV.FOV;
            float ScreenCenterX = screenSize.X / 2.0f;
            float ScreenCenterY = screenSize.Y / 2.0f;

            Screenlocation.X = ScreenCenterX + vTransformed.X * (ScreenCenterX / (float)Math.Tan(FovAngle * (float)Math.PI / 360.0f)) / vTransformed.Z;
            Screenlocation.Y = ScreenCenterY - vTransformed.Y * (ScreenCenterX / (float)Math.Tan(FovAngle * (float)Math.PI / 360.0f)) / vTransformed.Z;

            if (Screenlocation.X <= (int)screenSize.X + 50 && Screenlocation.X >= -0 && Screenlocation.Y <= (screenSize.Y) && Screenlocation.Y >= 0)
            {
                return true;
            }

            return false;
        }

        public Vector2 WorldToScreen(Vector3 WorldLocation, Vector2 screenSize)
        {
            Vector2 Screenlocation;

            Matrix tempMatrix = new Matrix(CameraCache.POV.Rotation); // Matrix

            Vector3 vAxisX, vAxisY, vAxisZ;

            vAxisX = new Vector3(tempMatrix.M11, tempMatrix.M12, tempMatrix.M13);
            vAxisY = new Vector3(tempMatrix.M21, tempMatrix.M22, tempMatrix.M23);
            vAxisZ = new Vector3(tempMatrix.M31, tempMatrix.M32, tempMatrix.M33);

            Vector3 vDelta = WorldLocation - CameraCache.POV.Location;
            Vector3 vTransformed = new Vector3(Vector3.Dot(vDelta, vAxisY), Vector3.Dot(vDelta, vAxisZ), Vector3.Dot(vDelta, vAxisX));

            if (vTransformed.Z < 1.0f)
                vTransformed.Z = 1.0f;

            float FovAngle = CameraCache.POV.FOV;
            float ScreenCenterX = screenSize.X / 2.0f;
            float ScreenCenterY = screenSize.Y / 2.0f;

            Screenlocation.X = ScreenCenterX + vTransformed.X * (ScreenCenterX / (float)Math.Tan(FovAngle * (float)Math.PI / 360.0f)) / vTransformed.Z;
            Screenlocation.Y = ScreenCenterY - vTransformed.Y * (ScreenCenterX / (float)Math.Tan(FovAngle * (float)Math.PI / 360.0f)) / vTransformed.Z;

            return Screenlocation;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct FCameraCacheEntry
    {
        [FieldOffset(16)]
        public FMinimalViewInfo POV; //0x0010
    }

    [StructLayout(LayoutKind.Explicit)]
    struct FMinimalViewInfo
    {
        [FieldOffset(12)]
        public Vector3 Rotation; //0x000C

        [FieldOffset(36)]
        public float FOV; //0x0024

        [FieldOffset(40)]
        public Vector3 Location; //0x0028
    }

    [StructLayout(LayoutKind.Explicit)]
    struct ULevel
    {
        [FieldOffset(88)]
        private IntPtr encryptedActorArrayPtr; //0x0058

        public AActor[] AActors()
        {
            IntPtr AActorsPtr = Decrypt.ActorArray(encryptedActorArrayPtr);
            ActorArray actorArray = Driver.Read<ActorArray>(AActorsPtr);
            IntPtr[] actorPtrArray = Driver.ReadArray<IntPtr>(actorArray.Actors, actorArray.NumActors);

            AActor[] aActors = new AActor[actorPtrArray.Length];
            for (int i = 0; i < actorPtrArray.Length; i++)
            {
                aActors[i] = Driver.Read<AActor>(actorPtrArray[i]);
            }
            return aActors;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct ActorArray
    {
        [FieldOffset(0)]
        public IntPtr Actors; //0x0000

        [FieldOffset(8)]
        public int NumActors; //0x0008
    }

    enum Bones : int
    {
        Root = 0,
        Head = 11,
    };

    [StructLayout(LayoutKind.Explicit)]
    struct AActor
    {
        [FieldOffset(0)]
        public int ObjectID; //0x0000

        [FieldOffset(1160)]
        private IntPtr encryptedMeshPtr; //0x0488

        [FieldOffset(2012)]
        public int TeamID; //0x07DC

        [FieldOffset(3640)]
        public float Health; //0x0E38

        private USkeletalMeshComponent Mesh()
        {
            IntPtr MeshPtr = Decrypt.InPawn(encryptedMeshPtr);
            return Driver.Read<USkeletalMeshComponent>(MeshPtr);
        }

        public Vector3 GetBoneLocation(Bones bone)
        {
            USkeletalMeshComponent Mesh = this.Mesh();
            FTransform boneTransform = Driver.Read<FTransform>(Mesh.BoneArray + ((int)bone * Marshal.SizeOf<FTransform>()));
            Matrix boneMatrix = boneTransform.ToMatrixWithScale();
            Matrix componentToWorldMatrix = Mesh.ComponentToWorld.ToMatrixWithScale();

            Matrix newMatrix = boneMatrix * componentToWorldMatrix;

            return new Vector3(newMatrix.M41, newMatrix.M42, newMatrix.M43);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct USkeletalMeshComponent
    {
        [FieldOffset(512)]
        public FTransform ComponentToWorld; //0x0200

        [FieldOffset(1784)]
        public IntPtr BoneArray; //0x06F8
    }

    public class Decrypt
    {
        public static unsafe IntPtr ULevel(IntPtr ptr)
        {
            long v20;
            *((uint*)&(v20)) = (uint)(((ulong)ptr - 536271410) ^ 0x505010B0);
            *((uint*)&(v20) + 1) = ((*((uint*)&(ptr) + 1)) - 1028982112) ^ 0xB050B050;
            return (IntPtr)v20;
        }

        public static unsafe IntPtr GameInstance(IntPtr ptr)
        {
            long v3; // [rsp+30h] [rbp+8h]
            *((uint*)&(v3)) = __ROL4__(__ROL4__((uint)(ulong)ptr, 16) + 1557949668, 16) ^ 0x5CDC6CE4;
            *((uint*)&(v3) + 1) = __ROR4__(__ROR4__(*((uint*)&(ptr) + 1), 8) + 1533238116, 8) ^ 0xA49CA49C;
            return (IntPtr)v3;
        }

        public static unsafe IntPtr LocalPlayer(IntPtr ptr)
        {
            long v12; // [rsp+50h] [rbp+8h]
            *((uint*)&(v12)) = (uint)((ulong)ptr - 1960050811) ^ 0x8B2BFF85;
            *((uint*)&(v12) + 1) = (*((uint*)&(ptr) + 1) + 174328485) ^ 0xF59BF55B;
            return (IntPtr)v12;
        }

        public static unsafe IntPtr PlayerController(IntPtr ptr)
        {
            ulong ptr2 = (ulong)ptr;
            long v8; // [rsp+50h] [rbp+8h]
            *((uint*)&(v8)) = __ROR4__(__ROL4__((uint)ptr2 - 748909937, 8) + 574385568, 16) ^ 0xBFDFE311;
		    *((uint*)&(v8) + 1) = __ROR4__(__ROR4__(*((uint*)&(ptr2) + 1) + 1157485944, 16) + 309204009, 8) ^ 0xC18FC14F;
            return (IntPtr)v8;
        }

        public static unsafe IntPtr InPawn(IntPtr ptr)
        {
            ulong ptr2 = (ulong)ptr;
            ulong v11 = ptr2; // rcx
            ulong v12; // r9
            long v19; // [rsp+78h] [rbp+20h]
            v12 = ptr2 >> 32;

            *((uint*)&(v19)) = (uint)((((uint)((((uint)v11 >> 16) ^ (ushort)v11) | ((ushort)__ROL2__(*((ushort*)&(v11) + 1), 8) << 16)) - 1555843091) & 0xFFFF0000 | (ushort)((*((ushort*)&(v11) + 1) ^ v11) - 18451) ^ (((uint)(((uint)v11 >> 16) ^ (ushort)v11 | ((ushort)__ROL2__(*((ushort*)&(v11) + 1), 8) << 16)) - 1555843091) >> 16)) ^ 0xA343B7ED);
            *((uint*)&(v11)) = (*((uint*)&(v11) + 1) & 0xFFFF0000 | (ushort)__ROR2__((ushort)(*((ushort*)&(v11) + 2) ^ *((ushort*)&(v12) + 1)), 8)) +583803405;
            *((uint*)&(v19) + 1) = (ushort)(((ushort)__ROR2__((ushort)(v11 ^ *((ushort*)&(v11) + 1)), 8) | ((ushort)__ROL2__(*((ushort*)&(v11) + 1), 8) << 16)) ^ 0xDD33DDF3);
            return (IntPtr)v19;
        }

        public static unsafe IntPtr ActorArray(IntPtr ptr)
        {
            long v3; // [rsp+30h] [rbp+8h]
            *((uint*)&(v3)) = __ROR4__(__ROR4__((uint)(ulong)ptr, 8) - 671638663, 8) ^ 0xD7F79B79;
            *((uint*)&(v3) + 1) = (*((uint*)&(ptr) + 1) + 1457018393) ^ 0xA927A9E7;
            return (IntPtr)v3;
        }

        /*
         #define LOBYTE(x)   *((byte*)&(x)) 
         #define LOWORD(x)   *((ushort*)&(x)) 
         #define LODWORD(x)  *((uint*)&(x))  
         #define HIBYTE(x)   *((byte*)&(x)+1)
         #define HIWORD(x)   *((ushort*)&(x)+1)
         #define HIDWORD(x)  *((uint*)&(x)+1)
         #define WORD1(x)  *((ushort*)&(x)+n)
         */
        #region Rotate
        private static Dictionary<Type, object> RolByType = new Dictionary<Type, object>();


        private static T __ROL__<T>(T value, int count)
            where T : struct
        {
            // Lookup in cache first.
            Func<T, int, T> rol;
            object tmp;
            if (!RolByType.TryGetValue(typeof(T), out tmp))
            {
                // Prepare variables and parameters.
                var nbits = Marshal.SizeOf(typeof(T)) * 8;
                var valParam = Expression.Parameter(typeof(T));
                var countParam = Expression.Parameter(typeof(int));
                var lowHighVar = Expression.Variable(typeof(T));

                rol = Expression.Lambda<Func<T, int, T>>(
                    Expression.Block(new[] { lowHighVar },
                        // if (count > 0)
                        Expression.IfThenElse(Expression.GreaterThan(countParam, Expression.Constant(0)),
                            // {
                            Expression.Block(
                                // count %= nbits;
                                Expression.ModuloAssign(countParam, Expression.Constant(nbits)),
                                // T high = value >> (nbits - count);
                                Expression.Assign(lowHighVar,
                                    Expression.RightShift(valParam,
                                        Expression.Subtract(Expression.Constant(nbits), countParam))),
                                // if ( T(-1) < 0 ) // signed value
                                Expression.IfThen(Expression.LessThan(Expression.Convert(Expression.Constant(-1), typeof(T)), Expression.Constant(default(T))),
                                    // high &= ~((T(-1) << count));
                                    Expression.AndAssign(lowHighVar, Expression.Not(
                                        Expression.LeftShift(Expression.Convert(Expression.Constant(-1), typeof(T)), countParam)))),
                                // value <<= count;
                                Expression.LeftShiftAssign(valParam, countParam)
                                ),
                            // }
                            // else
                            // {
                            Expression.Block(
                                // count = -count % nbits;
                                Expression.Assign(countParam, Expression.Modulo(Expression.Negate(countParam), Expression.Constant(nbits))),
                                // T low = value << (nbits - count);
                                Expression.Assign(lowHighVar, Expression.LeftShift(valParam, Expression.Subtract(Expression.Constant(nbits), countParam))),
                                // value >>= count;
                                Expression.RightShiftAssign(valParam, countParam)
                                )
                            // }
                            ),
                            // return value | lowOrHigh;
                            Expression.Or(valParam, lowHighVar)
                        ), valParam, countParam).Compile();

                RolByType.Add(typeof(T), rol);
            }
            else
            {
                rol = (Func<T, int, T>)tmp;
            }

            return rol(value, count);
        }

        public static byte __ROL1__(byte value, int count) { return __ROL__(value, count); }
        public static ushort __ROL2__(ushort value, int count) { return __ROL__(value, count); }
        public static uint __ROL4__(uint value, int count) { return __ROL__(value, count); }
        public static ulong __ROL8__(ulong value, int count) { return __ROL__(value, count); }
        public static byte __ROR1__(byte value, int count) { return __ROL__(value, -count); }
        public static ushort __ROR2__(ushort value, int count) { return __ROL__(value, -count); }
        public static uint __ROR4__(uint value, int count) { return __ROL__(value, -count); }
        public static ulong __ROR8__(ulong value, int count) { return __ROL__(value, -count); }
    }
    #endregion
}
