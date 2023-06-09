﻿using System;
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
        MovementMgr movementMgr;
        Boolean isFighting = false;
        string prefix;
        Object player = null;

        public CombatMgr(WorldServerClient Client, string _prefix)
        {
            movementMgr = Client.movementMgr;
            client = Client;
            prefix = _prefix;
        }

        public void SetPlayer(Object obj)
        {
            player = obj;
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
                Log.WriteLine(LogType.Error, "Exception Occured", prefix);
                Log.WriteLine(LogType.Error, "Message: {0}", prefix, ex.Message);
                Log.WriteLine(LogType.Error, "Stacktrace: {0}", prefix, ex.StackTrace);
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
                        float dist = TerrainMgr.CalculateDistance(player.Position, target.Position);
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
                    Log.WriteLine(LogType.Error, "Exception Occured", prefix);
                    Log.WriteLine(LogType.Error, "Message: {0}", prefix, ex.Message);
                    Log.WriteLine(LogType.Error, "Stacktrace: {0}", prefix, ex.StackTrace);
                }
                Thread.Sleep(50);
            }
        }
    }
}
