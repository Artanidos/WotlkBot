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
        [PacketHandlerAtribute(WorldServerOpCode.SMSG_GROUP_INVITE)]
        public void HandleGroupInvite(PacketIn inpacket)
        {
            // 01 41 72 74 00 00 00 00 00 00 00 00 00 00
            /*
            System.Console.WriteLine("Group invite");
            byte[] bytes = inpacket.ReadBytes((int)inpacket.BaseStream.Length);
            foreach (byte b in bytes)
            {
                Console.Write(b.ToString("X2") + " ");
            }
            */
            inpacket.ReadByte();
            string inviter = inpacket.ReadString();
            System.Console.WriteLine("Invite from " + inviter);

            if (inviter == "Art")
            {
                PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_GROUP_ACCEPT);
                packet.Write((UInt32)0);
                Send(packet);
            }
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
                Log.WriteLine(LogType.Debug, "Got nice pong. We love server;)", mUsername);
            }
            else
                Log.WriteLine(LogType.Error, "Server pong'd bad sequence! Ours: {0} Theirs: {1}", mUsername, Ping_Seq, Server_Seq);
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
