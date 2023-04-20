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
    public class CombatMgr
    {
        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        public static extern uint MM_GetTime();

        Thread loop;
        public List<Object> Targets = new List<Object>();
        UInt32 lastUpdateTime;

        WorldServerClient client;
        ObjectMgr objectMgr;
        MovementMgr movementMgr;
        Boolean isFighting = false;


        public CombatMgr(WorldServerClient Client)
        {
            objectMgr = Client.objectMgr;
            movementMgr = Client.movementMgr;
            client = Client;
        }

        public void Start()
        {
            try
            {
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
                    if (Targets.Count > 0)
                    {
                        Object target = Targets.First();
                        float dist = TerrainMgr.CalculateDistance(objectMgr.getPlayerObject().Position, target.Position);
                        if (dist > 1)
                        {
                            movementMgr.Waypoints.Add(target.Position);
                        }
                        else if (dist < 1 && !isFighting)
                        {
                            client.Attack(target);
                            isFighting = true;
                        }
                        else if (isFighting && target.Health < 0)
                        {
                            isFighting = false;
                            Targets.Remove(target);
                        }
                        else if (isFighting && target.Health > 0)
                        {
                            Console.WriteLine(target.Health);
                        }
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
    }
}
