using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using WotlkClient.Network;
using WotlkClient.Constants;


namespace WotlkClient.Shared
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PacketHandlerAtribute : Attribute
    {
        public PacketHandlerAtribute(LogonServerOpCode opcode)
        {
            this.PacketID = new PacketId(opcode);
        }

        public PacketHandlerAtribute(WorldServerOpCode opcode)
        {
            this.PacketID = new PacketId(opcode);
        }

        protected PacketId Packetid;
        public PacketId PacketID
        {
            get
            {
                return this.Packetid;
            }
            set
            {
                this.Packetid = value;
            }

        }
    }

    



}
