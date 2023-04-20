using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

using System.Runtime.InteropServices;
using System.Resources;
using WotlkClient.Network;
using WotlkClient.Shared;
using WotlkClient.Constants;
using WotlkClient.Terrain;

namespace WotlkClient.Clients
{
    public class MovementMgr
    {
        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        public static extern uint MM_GetTime();

        private System.Timers.Timer aTimer = new System.Timers.Timer();
        Thread loop = null;
        public MovementFlag Flag = new MovementFlag();
        public List<Coordinate> Waypoints = new List<Coordinate>();
        Coordinate oldLocation;
        UInt32 lastUpdateTime;
        ObjectMgr objectMgr;
        TerrainMgr terrainMgr;


        public MovementMgr(WorldServerClient Client)
        {
            objectMgr = Client.objectMgr;
            terrainMgr = Client.terrainMgr;
        }

        public void Start()
        {
            try
            {
                Flag.SetMoveFlag(MovementFlags.MOVEMENTFLAG_NONE);
                lastUpdateTime = MM_GetTime();

                loop = new Thread(Loop);
                loop.IsBackground = true;
                loop.Start();
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogType.Error, "Exception Occured");
                Log.WriteLine(LogType.Error, "Message: {0}", ex.Message);
                Log.WriteLine(LogType.Error, "Stacktrace: {0}", ex.StackTrace);
            }
        }

        public void Stop()
        {   
            if (loop != null)
                loop.Abort();
        }

        void Loop()
        {
            while (true)
            {
                try
                {
                    Coordinate Waypoint;
                    float angle, dist;

                    UInt32 timeNow = MM_GetTime();
                    UInt32 diff = (timeNow - lastUpdateTime);
                    lastUpdateTime = timeNow;
                    if (Waypoints.Count != 0)
                    {
                        Waypoint = Waypoints.First();

                        if (Waypoint != null)
                        {
                            angle = TerrainMgr.CalculateAngle(objectMgr.getPlayerObject().Position, Waypoint);
                            dist = TerrainMgr.CalculateDistance(objectMgr.getPlayerObject().Position, Waypoint);
                            if (angle == objectMgr.getPlayerObject().Position.O)
                            {
                                if (dist > 1)
                                {
                                    Flag.SetMoveFlag(MovementFlags.MOVEMENTFLAG_FORWARD);
                                    UpdatePosition(diff);
                                    lastUpdateTime = timeNow;
                                }
                                else
                                {
                                    Waypoints.Remove(Waypoint);
                                }
                            }
                            else
                            {
                                Flag.SetMoveFlag(MovementFlags.MOVEMENTFLAG_NONE);
                                objectMgr.getPlayerObject().Position.O = angle;
                            }
                        }
                    }
                    else
                    {
                        Flag.Clear();
                        Flag.SetMoveFlag(MovementFlags.MOVEMENTFLAG_NONE);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogType.Error, "Exception Occured");
                    Log.WriteLine(LogType.Error, "Message: {0}", ex.Message);
                    Log.WriteLine(LogType.Error, "Stacktrace: {0}", ex.StackTrace);
                }
            }
        }


        public void UpdatePosition(UInt32 diff)
        {
            double h; double speed;

            if (objectMgr.getPlayerObject() == null)
                return;

            if (Flag.IsMoveFlagSet(MovementFlags.MOVEMENTFLAG_FORWARD))
            {
                speed = 7.0;
            }
            else
                return;

            float predictedDX = 0;
            float predictedDY = 0;

            if (oldLocation == null)
                oldLocation = objectMgr.getPlayerObject().Position;


            h = objectMgr.getPlayerObject().Position.O;

            float dt = (float)diff / 1000f;
            float dx = (float)Math.Cos(h) * (float)speed * dt;
            float dy = (float)Math.Sin(h) * (float)speed * dt;

            predictedDX = dx;
            predictedDY = dy;

            Coordinate loc = objectMgr.getPlayerObject().Position;
            float realDX = loc.X - oldLocation.X;
            float realDY = loc.Y - oldLocation.Y;

            float predictDist = (float)Math.Sqrt(predictedDX * predictedDX + predictedDY * predictedDY);
            float realDist = (float)Math.Sqrt(realDX * realDX + realDY * realDY);

            if (predictDist > 0.0)
            {

                Coordinate expected = new Coordinate(loc.X + predictedDX, loc.Y + predictedDY, objectMgr.getPlayerObject().Position.Z, objectMgr.getPlayerObject().Position.O);
                expected = terrainMgr.getZ(expected);
                objectMgr.getPlayerObject().Position = expected;

            }

            oldLocation = loc;
        }

        public float CalculateDistance(Coordinate c1)
        {
            return TerrainMgr.CalculateDistance(objectMgr.getPlayerObject().Position, c1);
        }
    }


    public class MovementFlag
    {
        public uint MoveFlags;

        public void Clear()
        {
            MoveFlags = new uint();
        }

        public void SetMoveFlag(MovementFlags flag)
        {
            MoveFlags |= (uint)flag;
        }
        public void UnSetMoveFlag(MovementFlags flag)
        {
            MoveFlags &= ~(uint)flag;
        }
        public bool IsMoveFlagSet(MovementFlags flag)
        {
            return ((MoveFlags & (uint)flag) >= 1) ? true : false;
        }
    }
}
