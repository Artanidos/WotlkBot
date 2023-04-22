using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Collections;
using System;

using WotlkClient.Network;
using WotlkClient.Shared;
using WotlkClient.Constants;



namespace WotlkClient.Clients
{
    public partial class WorldServerClient
    {
        /*
		[PacketHandlerAtribute(WorldServerOpCode.SMSG_COMPRESSED_UPDATE_OBJECT)]
		public void HandleCompressedObjectUpdate(PacketIn packet)
		{
			try
			{
				Int32 size = packet.ReadInt32();
				byte[] decomped = WotlkClient.Shared.Compression.Decompress(size, packet.ReadRemaining());
				packet = new PacketIn(decomped, 1);
				HandleObjectUpdate(packet);
			}
			catch(Exception ex)
			{
				Log.WriteLine(LogType.Error, "{1} \n {0}", prefix, ex.StackTrace, ex.Message);
			}
		}
        */
        [PacketHandlerAtribute(WorldServerOpCode.SMSG_UPDATE_OBJECT)]
        public void HandleObjectUpdate(PacketIn packet)
        {
            UInt32 UpdateBlocks = packet.ReadUInt32();

            for (int allBlocks = 0; allBlocks < UpdateBlocks; allBlocks++)
            {
                UpdateType type = (UpdateType)packet.ReadByte();

                WoWGuid updateGuid;
                uint updateId;
                uint fCount;


                switch (type)
                {
                    case UpdateType.Values:
                        Object getObject;
                        updateGuid = new WoWGuid(packet.ReadUInt64());
                        Console.WriteLine("HandleObjectUpdate values");

                        if (objectMgr.objectExists(updateGuid))
                        {
                            getObject = objectMgr.getObject(updateGuid);
                        }
                        else
                        {
                            getObject = new Object(updateGuid);
                            objectMgr.addObject(getObject);
                        }
                        Log.WriteLine(LogType.Normal, "Handling Fields Update for object: {0}", prefix, getObject.Guid.ToString());
                        HandleUpdateObjectFieldBlock(packet, getObject);
                        objectMgr.updateObject(getObject);

                        break;

                    case UpdateType.Create:
                    case UpdateType.CreateSelf:
                        updateGuid = new WoWGuid(packet.ReadUInt64());
                        Console.WriteLine("HandleObjectUpdate create " + updateGuid.ToString());
                        /*
                        updateId = packet.ReadByte();
                            fCount = GeUpdateFieldsCount(updateId);

                            if (objectMgr.objectExists(updateGuid))
                                objectMgr.delObject(updateGuid);

                            Object newObject = new Object(updateGuid);
                            newObject.Fields = new UInt32[2000];
                            objectMgr.addObject(newObject);
                            HandleUpdateMovementBlock(packet, newObject);
                            HandleUpdateObjectFieldBlock(packet, newObject);
                            objectMgr.updateObject(newObject);
                            Log.WriteLine(LogType.Normal, "Handling Creation of object: {0}", prefix, newObject.Guid.ToString());
                        */
                        break;

                    case UpdateType.OutOfRange:
                        Console.WriteLine("HandleObjectUpdate oor");
                        /*
                            fCount = packet.ReadByte();
                            for (int j = 0; j < fCount; j++)
                            {
                                if(packet.Remaining < 8)
                                    return;
                                WoWGuid guid = new WoWGuid(packet.ReadUInt64());
                                Log.WriteLine(LogType.Normal, "Handling delete for object: {0}", prefix, guid.ToString());
                                if (objectMgr.objectExists(guid))
                                    objectMgr.delObject(guid);
                            }
                        */
                        break;
                }
            }

        }


        public void HandleUpdateMovementBlock(PacketIn packet, Object newObject)
        {
            UInt16 flags = packet.ReadUInt16();


            if ((flags & 0x20) >= 1)
            {
                UInt32 flags2 = packet.ReadUInt32();
                UInt16 unk1 = packet.ReadUInt16();
                UInt32 unk2 = packet.ReadUInt32();
                newObject.Position = new Coordinate(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());

                if ((flags2 & 0x200) >= 1)
                {
                    packet.ReadBytes(21); //transporter
                }

                if (((flags2 & 0x2200000) >= 1) || ((unk1 & 0x20) >= 1))
                {
                    packet.ReadBytes(4); // pitch
                }

                packet.ReadBytes(4); //lastfalltime

                if ((flags2 & 0x1000) >= 1)
                {
                    packet.ReadBytes(16); // skip 4 floats
                }

                if ((flags2 & 0x4000000) >= 1)
                {
                    packet.ReadBytes(4); // skip 1 float
                }

                packet.ReadBytes(32); // all of speeds

                if ((flags2 & 0x8000000) >= 1) //spline ;/
                {
                    UInt32 splineFlags = packet.ReadUInt32();

                    if ((splineFlags & 0x00020000) >= 1)
                    {
                        packet.ReadBytes(4); // skip 1 float
                    }
                    else
                    {
                        if ((splineFlags & 0x00010000) >= 1)
                        {
                            packet.ReadBytes(4); // skip 1 float
                        }
                        else if ((splineFlags & 0x00008000) >= 1)
                        {
                            packet.ReadBytes(12); // skip 3 float
                        }
                    }

                    packet.ReadBytes(28); // skip 8 float

                    UInt32 splineCount = packet.ReadUInt32();

                    for (UInt32 j = 0; j < splineCount; j++)
                    {
                        packet.ReadBytes(12); // skip 3 float
                    }

                    packet.ReadBytes(13);

                }
            }

            else if ((flags & 0x100) >= 1)
            {
                packet.ReadBytes(40);
            }
            else if ((flags & 0x40) >= 1)
            {
                newObject.Position = new Coordinate(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
            }

            if ((flags & 0x8) >= 1)
            {
                packet.ReadBytes(4);
            }

            if ((flags & 0x10) >= 1)
            {
                packet.ReadBytes(4);
            }

            if ((flags & 0x04) >= 1)
            {
                packet.ReadBytes(8);
            }

            if ((flags & 0x2) >= 1)
            {
                packet.ReadBytes(4);
            }

            if ((flags & 0x80) >= 1)
            {
                packet.ReadBytes(8);
            }

            if ((flags & 0x200) >= 1)
            {
                packet.ReadBytes(8);
            }
        }

        public void HandleUpdateObjectFieldBlock(PacketIn packet, Object newObject)
        {
            uint lenght = packet.ReadByte();

            UpdateMask UpdateMask = new UpdateMask();
            UpdateMask.SetCount((ushort)(lenght));
            UpdateMask.SetMask(packet.ReadBytes((int)lenght * 4), (ushort)lenght);
            UInt32[] Fields = new UInt32[UpdateMask.GetCount()];

            for (int i = 0; i < UpdateMask.GetCount(); i++)
            {
                if (!UpdateMask.GetBit((ushort)i))
                {
                    if (packet.Remaining < 4)
                        return;
                    UInt32 val = packet.ReadUInt32();
                    newObject.SetField(i, val);
                    Log.WriteLine(LogType.Normal, "Update Field: {0} = {1}", prefix, (UpdateFields)i, val);
                }
            }
        }

        /*
        [PacketHandlerAtribute(WorldServerOpCode.SMSG_DESTROY_OBJECT)]
        public void DestroyObject(PacketIn packet)
        {
            WoWGuid guid = new WoWGuid(packet.ReadUInt64());
            objectMgr.delObject(guid);

        }
         * */

        public uint GeUpdateFieldsCount(uint updateId)
        {
            switch ((ObjectType)updateId)
            {
                case ObjectType.Object:
                    return (uint)UpdateFields.GAMEOBJECT_END;

                case ObjectType.Unit:
                    return (uint)UpdateFields.UNIT_END;

                case ObjectType.Player:
                    return (uint)UpdateFields.PLAYER_END;

                case ObjectType.Item:
                    return (uint)UpdateFields.ITEM_END;

                case ObjectType.Container:
                    return (uint)UpdateFields.CONTAINER_END;

                case ObjectType.DynamicObject:
                    return (uint)UpdateFields.DYNAMICOBJECT_END;

                case ObjectType.Corpse:
                    return (uint)UpdateFields.CORPSE_END;
                default:
                    return 0;
            }
        }

        public void CreatureQuery(WoWGuid guid, UInt32 entry)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_CREATURE_QUERY);
            packet.Write(entry);
            packet.Write(guid.GetNewGuid());
            Send(packet);
        }

        public void ObjectQuery(WoWGuid guid, UInt32 entry)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_Object_QUERY);
            packet.Write(entry);
            packet.Write(guid.GetNewGuid());
            Send(packet);
        }

        public void QueryName(WoWGuid guid)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_NAME_QUERY);
            packet.Write(guid.GetNewGuid());
            Send(packet);
        }

        public void QueryName(UInt64 guid)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_NAME_QUERY);
            packet.Write(guid);
            Send(packet);
        }

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_CREATURE_QUERY_RESPONSE)]
        public void Handle_CreatureQuery(PacketIn packet)
        {
            Entry entry = new Entry();
            entry.entry = packet.ReadUInt32();
            entry.name = packet.ReadString();
            entry.blarg = packet.ReadBytes(3);
            entry.subname = packet.ReadString();
            entry.flags = packet.ReadUInt32();
            entry.subtype = packet.ReadUInt32();
            entry.family = packet.ReadUInt32();
            entry.rank = packet.ReadUInt32();

            foreach (Object obj in objectMgr.getObjectArray())
            {
                if (obj.Fields != null)
                {
                    if (obj.Fields[(int)UpdateFields.OBJECT_FIELD_ENTRY] == entry.entry)
                    {
                        obj.Name = entry.name;
                        objectMgr.updateObject(obj);
                    }
                }
            }
        }


        [PacketHandlerAtribute(WorldServerOpCode.SMSG_NAME_QUERY_RESPONSE)]
        public void Handle_NameQuery(PacketIn packet)
        {
            if (packet.Remaining < 8)
                return;
            WoWGuid guid = new WoWGuid(packet.ReadUInt64());
            if (packet.Remaining == 0)
                return;
            string name = packet.ReadString();
            packet.ReadByte();
            Race Race = (Race)packet.ReadUInt32();
            Gender Gender = (Gender)packet.ReadUInt32();
            Classname Class = (Classname)packet.ReadUInt32();

            System.Console.WriteLine("Handle_NameQuery: " + name);

            if (objectMgr.objectExists(guid))    // Update existing Object
            {
                Object obj = objectMgr.getObject(guid);
                obj.Name = name;
                objectMgr.updateObject(obj);
            }
            else                // Create new Object        -- FIXME: Add to new 'names only' list?
            {
                Object obj = new Object(guid);
                obj.Name = name;
                objectMgr.addObject(obj);

                /* Process chat message if we looked them up now */
                for (int i = 0; i < ChatQueued.Count; i++)
                {
                    ChatQueue message = (ChatQueue)ChatQueued[i];
                    if (message.GUID.GetOldGuid() == guid.GetOldGuid())
                    {
                        Log.WriteLine(LogType.Chat, "[{1}] {0}", mUsername, message.Message, name);
                        ChatQueued.Remove(message);
                    }
                }

            }
        }

    }

}

