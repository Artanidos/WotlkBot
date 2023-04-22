using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

using WotlkClient.Shared;
using WotlkClient.Network;
using WotlkClient.Constants;
using System.Runtime.Remoting.Messaging;


namespace WotlkClient.Clients
{
    partial class WorldServerClient
    {
        [PacketHandlerAtribute(WorldServerOpCode.SMSG_GROUP_LIST)]
        public void HandleGroupList(PacketIn inpacket)
        { 
            byte type = inpacket.ReadByte();
            byte slot = inpacket.ReadByte();
            byte slot_flags = inpacket.ReadByte();
            byte slot_roles = inpacket.ReadByte();
            if(type == 8) // GROUPTYPE_LFG
            {
                byte finished = inpacket.ReadByte();
                UInt32 dungeon = inpacket.ReadUInt32();
            }      
            UInt64 guid = inpacket.ReadUInt64();
            UInt32 counter = inpacket.ReadUInt32();
            UInt32 count = inpacket.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                string name = inpacket.ReadString();
                WoWGuid pguid = new WoWGuid(inpacket.ReadUInt64());
                byte onlineState = inpacket.ReadByte();
                byte group = inpacket.ReadByte();
                byte flags = inpacket.ReadByte();
                byte role = inpacket.ReadByte();
                
                Object obj = new Object(pguid);
                obj.Name = name;
                objectMgr.addObject(obj);
                Console.WriteLine("Added player: " + name);
            }
        }

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_GROUP_INVITE)]
        public void HandleGroupInvite(PacketIn inpacket)
        {
            inpacket.ReadByte();
            inviteCallBack(inpacket.ReadString());
        }

        public void AcceptInviteRequest()
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_GROUP_ACCEPT);
            packet.Write((UInt32)0);
            Send(packet);
        }


        [PacketHandlerAtribute(WorldServerOpCode.SMSG_UPDATE_ACCOUNT_DATA)]
        void HandleAccountDataUpdate(PacketIn inpacket)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_UPDATE_ACCOUNT_DATA);
            packet.Write(0x00000002);
            packet.Write((UInt32)0);
            Send(packet);
        }

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_WARDEN_DATA)]
        void GotWarden(PacketIn packet)
        {
            Console.WriteLine(packet.ToHex());

        }

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_PONG)]
        void HandlePong(PacketIn packet)
        {
            UInt32 Server_Seq = packet.ReadUInt32();
            if (Server_Seq == Ping_Seq)
            {
                Ping_Res_Time = MM_GetTime();
                Latency = Ping_Res_Time - Ping_Req_Time;
                Ping_Seq += 1;
                Log.WriteLine(LogType.Debug, "Got nice pong. We love server;)", prefix);
            }
            else
                Log.WriteLine(LogType.Error, "Server pong'd bad sequence! Ours: {0} Theirs: {1}", prefix, Ping_Seq, Server_Seq);
        }

        public void SendEmote(EmoteType EmoteType)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_TEXT_EMOTE);
            packet.Write((UInt32)EmoteType);
            packet.Write((UInt32)0x00);
            packet.Write((UInt64)0x00);
            Send(packet);
        }

    }
}
