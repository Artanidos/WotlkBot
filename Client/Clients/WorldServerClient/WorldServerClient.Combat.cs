using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WotlkClient.Shared;
using WotlkClient.Network;
using WotlkClient.Crypt;
using WotlkClient.Constants;

namespace WotlkClient.Clients
{
    public partial class WorldServerClient
    {
        public void Attack(Object target)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_SET_SELECTION);
            if (player != null)
            {
                packet.Write(target.Guid.GetNewGuid());
            }
            Send(packet);

            packet = new PacketOut(WorldServerOpCode.CMSG_ATTACKSWING);
            if (player != null)
            {
                packet.Write(target.Guid.GetNewGuid());
            }
            Send(packet);
        }


    }
}
