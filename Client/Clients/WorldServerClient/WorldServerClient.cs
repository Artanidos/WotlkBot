using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Timers;
using WotlkClient.Constants;
using WotlkClient.Crypt;
using WotlkClient.Network;
using WotlkClient.Shared;
using WotlkClient.Terrain;

namespace WotlkClient.Clients
{
    public delegate void AuthCompletedCallBack(uint taskResult);
    public delegate void CharLoginCompletedCallBack(uint taskResult);
    public delegate void CharEnumCompletedCallBack(uint taskResult);
    public delegate void InviteCallBack(string inviter);

    public partial class WorldServerClient
    {
        private static object _lockObj = new object();
        AuthCompletedCallBack authCompletedCallBack;
        CharEnumCompletedCallBack charEnumCompletedCallBack;
        CharLoginCompletedCallBack charLoginCompletedCallBack;
        InviteCallBack inviteCallBack;
        private UInt32 packetNumber = 0;
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
        string prefix;

        //Packet Handling
        private PacketHandler pHandler;
        private PacketLoop pLoop = null;
        public PacketCrypt mCrypt;
        
        //Managers
        
        public MovementMgr movementMgr = null;
        public CombatMgr combatMgr = null;
        public TerrainMgr terrainMgr = null;
        
        //
        public Realm realm;
        public Character[] Charlist = new Character[0];

        public Object player = null;

        public WorldServerClient(string user, Realm rl, byte[] key, string charName, AuthCompletedCallBack callback)
        {
            prefix = user;
            mUsername = user.ToUpper();
            mCharname = charName;
            terrainMgr = new TerrainMgr(prefix);
            movementMgr = new MovementMgr(this, prefix);
            combatMgr = new CombatMgr(this, prefix);
            realm = rl;
            mKey = key;
            authCompletedCallBack = callback;
        }

        public WorldServerClient()
        {

        }

        public void SetInviteCallback(InviteCallBack callback)
        {
            inviteCallBack = callback;
        }

        public void Logout()
        {
            PacketOut ping = new PacketOut(WorldServerOpCode.CMSG_LOGOUT_REQUEST);
            Send(ping);
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
                Log.WriteLine(LogType.Success, "Successfully connected to WorldServer at: {0}!", prefix, realm.Address);

            }
            catch (SocketException ex)
            {
                Log.WriteLine(LogType.Error, "Failed to connect to realm: {0}", prefix, ex.Message);
                Disconnect();
                if (charLoginCompletedCallBack != null)
                    charLoginCompletedCallBack(1);
                return;
            }

            byte[] nullA = new byte[24];
            mCrypt = new PacketCrypt(nullA);
            Connected = true;
            pHandler = new PacketHandler(this, prefix);
            pLoop = new PacketLoop(this, mSocket, prefix);
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
            Console.WriteLine("pinging");
        }

        public void Send(PacketOut packet)
        {
            lock (_lockObj)
            {
                try
                {
                    if (!Connected)
                        return;
                    Log.WriteLine(LogType.Network, "Sending packet: {0}", prefix, packet.packetId);

                    Byte[] Data = packet.ToArray();

                    int Length = Data.Length;
                    byte[] Packet = new byte[2 + Length];
                    Packet[0] = (byte)(Length >> 8);
                    Packet[1] = (byte)(Length & 0xff);
                    Data.CopyTo(Packet, 2);
                    mCrypt.Encrypt(Packet, 0, 6);
                    packetNumber++;
                    Log.WriteLine(LogType.Packet, "{0}", prefix, packet.ToHex(packetNumber));
                    mSocket.Send(Packet);
                }
                catch (SocketException se)
                {
                    Log.WriteLine(LogType.Error, "Exception Occured in packet {0}", prefix, packetNumber);
                    Log.WriteLine(LogType.Error, "Message: {0}", prefix, se.Message);
                    Log.WriteLine(LogType.Error, "Stacktrace: {0}", prefix, se.StackTrace);
                    HardDisconnect();
                    System.Console.WriteLine("Disconnected from server with " + mUsername);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogType.Error, "Exception Occured", prefix);
                    Log.WriteLine(LogType.Error, "Message: {0}", prefix, ex.Message);
                    Log.WriteLine(LogType.Error, "Stacktrace: {0}", prefix, ex.StackTrace);
                }
            }
        }

        public void StartHeartbeat()
        {
            uTimer.Elapsed += new ElapsedEventHandler(Heartbeat);
            uTimer.Interval = 3000;
            uTimer.Enabled = true;
        }

        public void HandlePacket(PacketIn packet)
        {
            Log.WriteLine(LogType.Packet, "{0}", mUsername, packet.ToHex());
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

        void AppendPackedGuid(UInt64 guid, PacketOut stream)
        {
            byte[] packGuid = new byte[9];
            packGuid[0] = 0;
            int size = 1;

            for (byte i = 0; guid != 0; i++)
            {
                if ((guid & 0xFF) != 0)
                {
                    packGuid[0] |= (byte)(1 << i);
                    packGuid[size] = (byte)(guid & 0xFF);
                    size++;
                }
                guid >>= 8;
            }
            stream.Write(packGuid, 0, size);
        }


        UInt64 UnpackGuid(PacketIn stream)
        {
            UInt64 guid = 0;

            byte guidmark = stream.ReadByte();
            byte shift = 0;

            for (int i = 0; i < 8 && stream.Remaining > 0; i++)
            {
                if ((guidmark & (1 << i)) != 0)
                {
                    guid |= ((UInt64)stream.ReadByte()) << shift;
                    shift += 8;
                }
            }
            return guid;
        }
    }
}
