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
        

        public void CastSpell(UInt64 guid, UInt32 spellId)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_CAST_SPELL);
            packet.Write((byte)1); // count
            packet.Write((UInt32)spellId);
            packet.Write((byte)0); // flags
            packet.Write((UInt32)2); // TARGET_FLAG_UNIT
            AppendPackedGuid(guid, packet);
            Send(packet);
        }

        /*
        public void CastSpell(Object target, UInt32 SpellId)
        {
            SpellTargetFlags flags = 0;

            if (target == objectMgr.getPlayerObject())
                flags = SpellTargetFlags.Self;
            else
            {
                flags = SpellTargetFlags.Unit;
                //Target(target as Unit);
            }

            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_CAST_SPELL);
            packet.Write(SpellId);
            packet.Write((byte)0); // unk flags in WCell

            packet.Write((UInt32)flags);

            // 0x18A02
            if (flags.Has(SpellTargetFlags.SpellTargetFlag_Dynamic_0x10000 | SpellTargetFlags.Corpse | SpellTargetFlags.Object |
                SpellTargetFlags.PvPCorpse | SpellTargetFlags.Unit))
            {
                packet.Write(target.Guid.GetNewGuid());
            }

            // 0x1010
            if (flags.Has(SpellTargetFlags.TradeItem | SpellTargetFlags.Item))
            {
                packet.Write(target.Guid.GetNewGuid());
            }

            // 0x20
            if (flags.Has(SpellTargetFlags.SourceLocation))
            {
                packet.Write(objectMgr.getPlayerObject().Position.X);
                packet.Write(objectMgr.getPlayerObject().Position.Y);
                packet.Write(objectMgr.getPlayerObject().Position.Z);
            }

            // 0x40
            if (flags.Has(SpellTargetFlags.DestinationLocation))
            {
                packet.Write(target.Guid.GetNewGuid());

                packet.Write(target.Position.X);
                packet.Write(target.Position.Y);
                packet.Write(target.Position.Z);

            }

            Send(packet);
        }
        */
     }
}
