using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Resources;

using WotlkClient.Shared;
using WotlkClient.Network;
using WotlkClient.Crypt;
using WotlkClient.Constants;
using WotlkClient.Terrain;

namespace WotlkClient.Clients
{
    public partial class WorldServerClient
    {

        private UInt32 ServerSeed;
        private UInt32 ClientSeed;
        private Random random = new Random();

        public Socket mSocket = null;

        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        public static extern uint MM_GetTime();

        
        private System.Timers.Timer aTimer = new System.Timers.Timer();
        private System.Timers.Timer uTimer = new System.Timers.Timer();
        private UInt32 Ping_Seq;
        private UInt32 Ping_Req_Time;
        private UInt32 Ping_Res_Time;
        public UInt32 Latency;

        // Connection Info
        readonly string mUsername;
        readonly string mCharname;
        private byte[] mKey;
        public bool Connected;

        //Packet Handling
        private PacketHandler pHandler;
        private PacketLoop pLoop = null;
        public PacketCrypt mCrypt;
        
        //Managers
        public ObjectMgr objectMgr = null;
        public MovementMgr movementMgr = null;
        public CombatMgr combatMgr = null;
        public TerrainMgr terrainMgr = null;
        
        //
        public Realm realm;
        public Character[] Charlist = new Character[0];
        

        public WorldServerClient(string user, Realm rl, byte[] key, string charName, AuthCompletedCallBack _callback)
        {
            mUsername = user.ToUpper();
            mCharname = charName;
            objectMgr = new ObjectMgr();
            movementMgr = new MovementMgr(this);
            combatMgr = new CombatMgr(this);
            terrainMgr = new TerrainMgr();
            realm = rl;
            mKey = key;
            authCompletedCallBack = _callback;
        }

        public WorldServerClient(Realm rl, byte[] key)
        {
            mUsername = Config.Login.ToUpper();
            objectMgr = new ObjectMgr();
            movementMgr = new MovementMgr(this);
            combatMgr = new CombatMgr(this);
            terrainMgr = new TerrainMgr();
            realm = rl;
            mKey = key;
        }


        public void Connect()
        {
            string[] address = realm.Address.Split(':');
            byte[] test = new byte[1];
            test[0] = 10;
            mCrypt = new PacketCrypt(test);
            IPAddress WSAddr = Dns.GetHostAddresses(address[0])[0];
            int WSPort = Int32.Parse(address[1]);
            IPEndPoint ep = new IPEndPoint(WSAddr, WSPort);
            
            try
            {
                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                mSocket.Connect(ep);
                Log.WriteLine(LogType.Success, "Successfully connected to WorldServer at: {0}!", realm.Address);

            }
            catch (SocketException ex)
            {
                Log.WriteLine(LogType.Error, "Failed to connect to realm: {0}", ex.Message);
                Disconnect();
                if (charLoginCompletedCallBack != null)
                    charLoginCompletedCallBack(1);
                return;
            }

            byte[] nullA = new byte[24];
            mCrypt = new PacketCrypt(nullA);
            Connected = true;
            pHandler = new PacketHandler(this);
            pLoop = new PacketLoop(this, mSocket);
            pLoop.Start();
            pHandler.Initialize();
        }

        void PingLoop()
        {
            aTimer.Elapsed += new ElapsedEventHandler(Ping);
            aTimer.Interval = 1000000;
            aTimer.Enabled = true;

            Ping_Seq = 1;
            Latency = 1;
        }

        void Ping(object source, ElapsedEventArgs e)
        {
            while(!mSocket.Connected)
            {
                aTimer.Enabled = false;
                aTimer.Stop();
                return;
            }

            Ping_Req_Time = MM_GetTime();

            PacketOut ping = new PacketOut(WorldServerOpCode.CMSG_PING);
            ping.Write(Ping_Seq);
            ping.Write(Latency);
            Send(ping);
        }

        public void Send(PacketOut packet)
        {
            try
            {
                if (!Connected)
                    return;
                Log.WriteLine(LogType.Network, "Sending packet: {0}", packet.packetId);
                if (!Connected)
                    return;
                Byte[] Data = packet.ToArray();

                int Length = Data.Length;
                byte[] Packet = new byte[2 + Length];
                Packet[0] = (byte)(Length >> 8);
                Packet[1] = (byte)(Length & 0xff);
                Data.CopyTo(Packet, 2);
                mCrypt.Encrypt(Packet, 0, 6);
                //While writing this part of code I had a strange feeling of Deja-Vu or whatever it's called :>

                Log.WriteLine(LogType.Packet,"{0}", packet.ToHex());
                mSocket.Send(Packet);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogType.Error, "Exception Occured");
                Log.WriteLine(LogType.Error, "Message: {0}", ex.Message);
                Log.WriteLine(LogType.Error, "Stacktrace: {0}", ex.StackTrace);
            }
        }

        public void StartHeartbeat()
        {
            aTimer.Elapsed += new ElapsedEventHandler(Heartbeat);
            aTimer.Interval = 3000;
            aTimer.Enabled = true;
        }

        public void HandlePacket(PacketIn packet)
        {
            Log.WriteLine(LogType.Packet, "{0}", packet.ToHex());
            pHandler.HandlePacket(packet);
        }

        public void Disconnect()
        {
            
        }

        public void HardDisconnect()
        {
            if (mSocket != null && mSocket.Connected)
                mSocket.Close();
            
            if (movementMgr != null)
                movementMgr.Stop();
            if (combatMgr != null)
                combatMgr.Stop();
            if (pLoop != null)
                pLoop.Stop();
            Connected = false;
        }

        ~WorldServerClient()
        {
            HardDisconnect();
        }
    }
}
