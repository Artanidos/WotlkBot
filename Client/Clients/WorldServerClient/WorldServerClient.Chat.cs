using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

using WotlkClient.Shared;
using WotlkClient.Network;
using WotlkClient.Constants;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace WotlkClient.Clients
{
    partial class WorldServerClient
    {
        private ArrayList ChatQueued = new ArrayList();

        private void HandleBotCommand(string message, UInt64 user)
        {
            
            string cmd = message.Substring(4, message.Length - 5);
            Console.WriteLine("bot " + cmd);
            if (cmd == "move forward")
            {
                MoveForward();
            }
            else if(cmd == "move stop")
            {
                MoveStop();
            }
            else if(cmd == "heal me")
            {
                CastSpell(user, 2050); // LESSER_HEAL
            }
            else if (cmd == "buff me")
            {
                CastSpell(user, 1243); // PW_FORTITUDE
            }
            else if(cmd == "follow me")
            {
                // not working yet, because player is not known
                WoWGuid fguid = new WoWGuid(user);
                if (objectMgr.objectExists(fguid))
                {
                    Console.WriteLine("found player in objectMgr " + objectMgr.getObject(fguid).Position + ", " + objectMgr.getPlayerObject().Position);
                    if (objectMgr.getObject(fguid).Position != null && objectMgr.getPlayerObject().Position != null)
                    {

                        movementMgr.Waypoints.Add(objectMgr.getObject(fguid).Position);
                    }

                }
                
                QueryName(fguid);
                QueryName(objectMgr.getPlayerObject().Guid);
                
            }
        }

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_CHANNEL_NOTIFY)]
        public void HandleChannelNotify(PacketIn packet)
        {
            
        }

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_MESSAGECHAT)]
        public void HandleChat(PacketIn packet)
        {
            try
            {
                string channel = null;
                UInt64 guid = 0;
                WoWGuid fguid = null, fguid2 = null;
                string username = null;

                byte Type = packet.ReadByte();
                UInt32 Language = packet.ReadUInt32();

                guid = packet.ReadUInt64();
                fguid = new WoWGuid(guid);
                packet.ReadInt32();

                if ((ChatMsg)Type == ChatMsg.Channel)
                {
                    channel = packet.ReadString();
                }

                if (Type == 47)
                    return;
                fguid2 = new WoWGuid(packet.ReadUInt64());

                UInt32 Length = packet.ReadUInt32();
                string Message = Encoding.Default.GetString(packet.ReadBytes((int)Length));

                byte afk = 0;
           
                if (fguid.GetOldGuid() == 0)
                {
                    username = "System";
                }
                else
                {
                    if (objectMgr.objectExists(fguid))
                        username = objectMgr.getObject(fguid).Name;
                }

                if (Message.StartsWith("bot ") && (ChatMsg)Type == ChatMsg.Whisper)
                {
                    Console.WriteLine("guid " + guid.ToString());
                    HandleBotCommand(Message, guid);
                    return;
                }

                if (username == null)
                {
                    ChatQueue que = new ChatQueue();
                    que.GUID = fguid;
                    que.Type = Type;
                    que.Language = Language;
                    if ((ChatMsg)Type == ChatMsg.Channel)
                        que.Channel = channel;
                    que.Length = Length;
                    que.Message = Message;
                    que.AFK = afk;
                    ChatQueued.Add(que);
                    QueryName(guid);
                    return;
                }
                
                Log.WriteLine(LogType.Chat, "[{1}] {0}", prefix, Message);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogType.Error, "Exception Occured", prefix);
                Log.WriteLine(LogType.Error, "Message: {0}", prefix, ex.Message);
                Log.WriteLine(LogType.Error, "Stacktrace: {0}", prefix, ex.StackTrace);
            }
        }

        public void SendChatMsg(ChatMsg Type, Languages Language, string Message)
        {
            if (Type != ChatMsg.Whisper || Type != ChatMsg.Channel)
                SendChatMsg(Type, Language, Message, "");
        }

        public void SendChatMsg(ChatMsg Type, Languages Language, string Message, string To)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_MESSAGECHAT);
            packet.Write((UInt32)Type);
            packet.Write((UInt32)Language);
            if ((Type == ChatMsg.Whisper || Type == ChatMsg.Channel) && To != "")
                packet.Write(To);
            packet.Write(Message);
            Send(packet);
        }

        public void SendEmoteMsg(ChatMsg Type, Languages Language, string Message, string To)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_TEXT_EMOTE);
            packet.Write((UInt32)Type);
            packet.Write((UInt32)Language);
            packet.Write(Message);
            Send(packet);
        }

        public void JoinChannel(string channel, string password)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_JOIN_CHANNEL);
            packet.Write((UInt32)0);
            packet.Write((UInt16)0);
            packet.Write(channel);
            packet.Write("");
            Send(packet);
        }
    }

    
   

    public struct ChatQueue
    {
        public WoWGuid GUID;
        public byte Type;
        public UInt32 Language;
        public string Channel;
        public UInt32 Length;
        public string Message;
        public byte AFK;

    };
}
