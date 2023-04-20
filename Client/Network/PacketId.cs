using System;
using WotlkClient.Constants;

namespace WotlkClient.Network
{

    public struct PacketId
    {
        public ServiceType Service;
        public uint RawId;

        public PacketId(LogonServerOpCode id)
        {
            Service = ServiceType.Logon;
            RawId = (uint)id;
        }

        public PacketId(WorldServerOpCode id)
        {
            Service = ServiceType.World;
            RawId = (uint)id;
        }

        public static bool operator ==(PacketId orig, PacketId copy)
        {
            return (orig.RawId == copy.RawId) && (orig.Service == copy.Service);
        }

        public static bool operator !=(PacketId orig, PacketId copy)
        {
            return !(orig == copy);
        }

        public override string ToString()
        {
            string temp;
            if (Service == ServiceType.Logon)
            {
                temp = string.Format("Logon Server Packet: {0}", (LogonServerOpCode)RawId);
                return temp;
            }
            else if (Service == ServiceType.World)
            {
                temp = string.Format("World Server Packet: {0}", (WorldServerOpCode)RawId);
                return temp;
            }
            return null;   
        }
    }
}