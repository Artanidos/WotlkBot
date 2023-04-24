using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

using System.Runtime.InteropServices;
using System.Resources;

using WotlkClient.Shared;
using WotlkClient.Network;
using WotlkClient.Constants;

namespace WotlkClient.Clients
{
    public partial class WorldServerClient
    {
        // forward flag   flag2    time            pos.x       pos.y           pos.z       pos.o
        // 01 00 00 00 - 00 00 - 48 36 5f 17 - 13 82 0b c6 - 1c a3 fe c2 - d1 c8 a3 42 - 3d d8 93 40 - 00 00 00 00
        // stop
        // 00 00 00 00 - 00 00 - 14 38 5f 17 - 43 83 0b c6 - 5f 86 02 c3 - 80 25 a4 42 - 3d d8 93 40 - 00 00 00 00
        // 01 = MOVEMENTFLAG_FORWARD
        // 00 = MOVEMENTFLAG_NONE
        public void MoveForward()
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.MSG_MOVE_START_FORWARD);
            AppendPackedGuid(objectMgr.getPlayerObject().Guid.GetOldGuid(), packet);
            // add new position
            Send(packet);
        }

        public void MoveStop()
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.MSG_MOVE_STOP);
            AppendPackedGuid(objectMgr.getPlayerObject().Guid.GetOldGuid(), packet);
            // add new position
            Send(packet);
        }


        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_FORWARD)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_BACKWARD)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_STOP)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_STRAFE_LEFT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_STRAFE_RIGHT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_STOP_STRAFE)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_JUMP)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_TURN_LEFT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_TURN_RIGHT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_STOP_TURN)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_PITCH_UP)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_PITCH_DOWN)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_STOP_PITCH)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_SET_RUN_MODE)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_SET_WALK_MODE)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_TOGGLE_LOGGING)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_TELEPORT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_TELEPORT_CHEAT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_TELEPORT_ACK)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_TOGGLE_FALL_LOGGING)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_FALL_LAND)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_SWIM)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_STOP_SWIM)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_HEARTBEAT)]
        public void HandleAnyMove(PacketIn packet)
        {
            byte mask = packet.ReadByte();

            WoWGuid guid = new WoWGuid(mask, packet.ReadBytes(WoWGuid.BitCount8(mask)));

            Object obj = objectMgr.getObject(guid);
            if (obj != null)
            {
                    packet.ReadBytes(9);
                    obj.Position= new Coordinate(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
            }
        }

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_MONSTER_MOVE)]
        public void HandleMonsterMove(PacketIn packet)
        {
            byte mask = packet.ReadByte();

            WoWGuid guid = new WoWGuid(mask, packet.ReadBytes(WoWGuid.BitCount8(mask)));

            Object obj = objectMgr.getObject(guid);
            if (obj != null)
            {
                System.Console.WriteLine("MONSTER_MOVE " + obj.Name);
                obj.Position = new Coordinate(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
            }
            else
            {
                QueryName(guid);
            }
        }

        void Heartbeat(object source, ElapsedEventArgs e)
        {
            if (objectMgr.getPlayerObject().Position == null)
                return;

            PacketOut packet = new PacketOut(WorldServerOpCode.MSG_MOVE_HEARTBEAT);
            packet.Write(movementMgr.Flag.MoveFlags);
            packet.Write((byte)0);
            packet.Write((UInt32)MM_GetTime());
            packet.Write((float)objectMgr.getPlayerObject().Position.X);
            packet.Write((float)objectMgr.getPlayerObject().Position.Y);
            packet.Write((float)objectMgr.getPlayerObject().Position.Z);
            packet.Write((float)objectMgr.getPlayerObject().Position.O);
            packet.Write((UInt32)0);
            Send(packet);
        }
    }
}

