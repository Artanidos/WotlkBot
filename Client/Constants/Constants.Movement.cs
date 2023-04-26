using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WotlkClient.Constants
{
    public enum MovementFlags
    {
        MOVEMENTFLAG_NONE = 0x00000000,
        MOVEMENTFLAG_FORWARD = 0x00000001,
        MOVEMENTFLAG_BACKWARD = 0x00000002,
        MOVEMENTFLAG_STRAFE_LEFT = 0x00000004,
        MOVEMENTFLAG_STRAFE_RIGHT = 0x00000008,
        MOVEMENTFLAG_LEFT = 0x00000010,
        MOVEMENTFLAG_RIGHT = 0x00000020,
        MOVEMENTFLAG_PITCH_UP = 0x00000040,
        MOVEMENTFLAG_PITCH_DOWN = 0x00000080,
        MOVEMENTFLAG_WALK = 0x00000100,
        MOVEMENTFLAG_ONTRANSPORT = 0x00000200,
        MOVEMENTFLAG_UNK1 = 0x00000400,
        MOVEMENTFLAG_FLY_UNK1 = 0x00000800,
        MOVEMENTFLAG_JUMPING = 0x00001000,
        MOVEMENTFLAG_UNK4 = 0x00002000,
        MOVEMENTFLAG_FALLING = 0x00004000,
        MOVEMENTFLAG_SWIMMING = 0x00200000,               // appears with fly flag also
        MOVEMENTFLAG_FLY_UP = 0x00400000,
        MOVEMENTFLAG_CAN_FLY = 0x00800000,
        MOVEMENTFLAG_FLYING = 0x01000000,
        MOVEMENTFLAG_UNK5 = 0x02000000,
        MOVEMENTFLAG_SPLINE = 0x04000000,               // probably wrong name
        MOVEMENTFLAG_SPLINE2 = 0x08000000,
        MOVEMENTFLAG_WATERWALKING = 0x10000000,
        MOVEMENTFLAG_SAFE_FALL = 0x20000000,               // active rogue safe fall spell (passive)
        MOVEMENTFLAG_UNK3 = 0x40000000
    };

    public enum SplineFlags
    {
        NONE = 0x00000000,
        POINT = 0x00010000,
        TARGET = 0x00020000,
        ORIENT = 0x00040000
    };

    [Flags]
    public enum UpdateFlag
    {
        None = 0x000,
        Self = 0x001,
        Transport = 0x002,
        AttackingTarget = 0x004,
        LowGuid = 0x008,
        HighGuid = 0x010,
        Living = 0x020,
        StationaryObject = 0x040,
        Vehicle = 0x080,
        GOPosition = 0x100,
        GORotation = 0x200,
        Unknown2 = 0x400,
    }

    [Flags]
    public enum MovementFlagWotLK : uint
    {
        None = 0x00000000,
        Forward = 0x00000001,
        Backward = 0x00000002,
        StrafeLeft = 0x00000004,
        StrafeRight = 0x00000008,
        TurnLeft = 0x00000010,
        TurnRight = 0x00000020,
        PitchUp = 0x00000040,
        PitchDown = 0x00000080,
        WalkMode = 0x00000100,
        OnTransport = 0x00000200,
        DisableGravity = 0x00000400,
        Root = 0x00000800,
        Falling = 0x00001000,
        FallingFar = 0x00002000,
        PendingStop = 0x00004000,
        PendingStrafeStop = 0x00008000,
        PendingForward = 0x00010000,
        PendingBackward = 0x00020000,
        PendingStrafeLeft = 0x00040000,
        PendingStrafeRight = 0x00080000,
        PendingRoot = 0x00100000,
        Swimming = 0x00200000,
        Ascending = 0x00400000,
        Descending = 0x00800000,
        CanFly = 0x01000000,
        Flying = 0x02000000,
        SplineElevation = 0x04000000,
        SplineEnabled = 0x08000000,
        Waterwalking = 0x10000000,
        CanSafeFall = 0x20000000,
        Hover = 0x40000000,
        LocalDirty = 0x80000000
    }

    [Flags]
    public enum MovementFlagExtra : ushort
    {
        None = 0x0000,
        PreventStrafe = 0x0001, // 4.x
        PreventJumping = 0x0002, // 4.x
        DisableCollision = 0x0004, // 4.x
        FullSpeedTurning = 0x0008,
        FullSpeedPitching = 0x0010,
        AlwaysAllowPitching = 0x0020,
        IsVehicleExitVoluntary = 0x0040, // 4.x
        IsJumpSplineInAir = 0x0080, // 4.x
        IsAnimTierInTrans = 0x0100, // 4.x
        PreventChangePitch = 0x0200, // 4.x
        InterpolateMove = 0x0400, // 4.x (Interpolation is player only)
        InterpolateTurning = 0x0800,
        InterpolatePitching = 0x1000,
        VehiclePassengerIsTransitionAllowed = 0x2000, // 4.x
        CanTransitionBetweenSwimAndFly = 0x4000, // 4.x
        Unknown10 = 0x8000
    }

    [Flags]
    public enum SplineFlagWotLK : uint
    {
        None = 0x00000000,
        AnimTierSwim = 0x00000001,
        AnimTierHover = 0x00000002,
        AnimTierFly = 0x00000003,
        AnimTierSubmerged = 0x00000004,
        Done = 0x00000100,
        Falling = 0x00000200,
        NoSpline = 0x00000400,
        Trajectory = 0x00000800,
        WalkMode = 0x00001000,
        Flying = 0x00002000,
        Knockback = 0x00004000,
        FinalPoint = 0x00008000,
        FinalTarget = 0x00010000,
        FinalOrientation = 0x00020000,
        CatmullRom = 0x00040000,
        Cyclic = 0x00080000,
        EnterCycle = 0x00100000,
        AnimationTier = 0x00200000,
        Frozen = 0x00400000,
        Transport = 0x00800000,
        TransportExit = 0x01000000,
        Unknown7 = 0x02000000,
        Unknown8 = 0x04000000,
        OrientationInverted = 0x08000000,
        UsePathSmoothing = 0x10000000,
        Animation = 0x20000000,
        UncompressedPath = 0x40000000,
        Unknown10 = 0x80000000
    }

    public static class Extensions
    {
        public static bool HasAnyFlag(this IConvertible value, IConvertible flag)
        {
            var uFlag = flag.ToUInt64(null);
            var uThis = value.ToUInt64(null);

            return (uThis & uFlag) != 0;
        }
    }
}
