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
            catch (Exception ex)
            {
                Log.WriteLine(LogType.Error, "{1} \n {0}", prefix, ex.StackTrace, ex.Message);
            }
        }

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
                        updateGuid = new WoWGuid(UnpackGuid(packet));

                        if (ObjectMgr.GetInstance().objectExists(updateGuid))
                        {
                            getObject = ObjectMgr.GetInstance().getObject(updateGuid);
                        }
                        else
                        {
                            getObject = new Object(updateGuid);
                            ObjectMgr.GetInstance().addObject(getObject);
                        }
                        //HandleUpdateObjectFieldBlock(packet, getObject, updateGuid.GetTypeId());
                        ReadValuesUpdateBlock(packet, updateGuid.GetTypeId());
                        ObjectMgr.GetInstance().updateObject(getObject);
                        break;

                    case UpdateType.Create:
                    case UpdateType.CreateSelf:
                        updateGuid = new WoWGuid(UnpackGuid(packet));
                        Object obj;
                        if (ObjectMgr.GetInstance().objectExists(updateGuid))
                        {
                            obj = ObjectMgr.GetInstance().getObject(updateGuid);
                        }
                        else
                        {
                            obj = new Object(updateGuid);
                            ObjectMgr.GetInstance().addObject(obj);
                        }
                        ReadMovementUpdateBlock(packet, obj);
                        ReadValuesUpdateBlock(packet, updateGuid.GetTypeId());
                        ObjectMgr.GetInstance().updateObject(obj);
                        break;
                        
                    case UpdateType.OutOfRange: // checked
                        fCount = packet.ReadUInt32();
                        for (int j = 0; j < fCount; j++)
                        {
                            if (packet.Remaining < 8)
                                return;
                            WoWGuid guid = new WoWGuid(UnpackGuid(packet));
                            Log.WriteLine(LogType.Normal, "Handling delete for object: {0}", prefix, guid.ToString());
                            if (ObjectMgr.GetInstance().objectExists(guid))
                                ObjectMgr.GetInstance().delObject(guid);
                        }
                        break;
                    
                    case UpdateType.NearObjects:
                        Console.WriteLine("near objects ");
                        break;

                }
            }
        }

        private void ReadValuesUpdateBlock(PacketIn packet, ObjectType type)
        {
            var maskSize = packet.ReadByte();

            var updateMask = new int[maskSize];
            for (var i = 0; i < maskSize; i++)
                updateMask[i] = packet.ReadInt32();

            var mask = new BitArray(updateMask);


            for (var i = 0; i < mask.Count; ++i)
            {
                if (!mask[i])
                    continue;

                packet.ReadUInt32();   // UpdateField blockVal = 
                
                int start = i;
                int size = 1;
                
                for (int k = i - start + 1; k < size; ++k)
                {
                    int currentPosition = ++i;
                    
                    if (mask[currentPosition])
                        packet.ReadUInt32();   // updateField = 

                }
            }
        }

        MovementFlagWotLK ReadMovementInfoLegacy(PacketIn packet, Object obj)
        {
            MovementFlagWotLK flags = (MovementFlagWotLK)packet.ReadUInt32();
            UInt16 flagsExtra = packet.ReadUInt16();
            bool hasPitch = flags.HasAnyFlag(MovementFlagWotLK.Swimming | MovementFlagWotLK.Flying) || flagsExtra.HasAnyFlag(MovementFlagExtra.AlwaysAllowPitching);

            packet.ReadUInt32();    // time
            float x = packet.ReadFloat();
            float y = packet.ReadFloat();
            float z = packet.ReadFloat();
            float o = packet.ReadFloat();
            obj.Position = new Coordinate(x, y, z, o);
            
            

            if (flags.HasAnyFlag(MovementFlagWotLK.OnTransport))
            {
                UnpackGuid(packet);
                packet.ReadFloat();   // transport offset
                packet.ReadFloat();
                packet.ReadFloat();
                packet.ReadFloat();     // transport orientation
                packet.ReadUInt32();    // transport time
                packet.ReadByte();      // transport seat
                if (flagsExtra.HasAnyFlag(MovementFlagExtra.InterpolateMove))
                    packet.ReadUInt32(); // transport time 2
            }
            if (hasPitch)
                packet.ReadFloat(); // swimpitch
            packet.ReadUInt32();    // falltime

            if (flags.HasAnyFlag(MovementFlagWotLK.Falling))
            {
                packet.ReadFloat(); // info.JumpVerticalSpeed
                packet.ReadFloat(); // info.JumpSinAngle
                packet.ReadFloat(); // info.JumpCosAngle
                packet.ReadFloat(); // info.JumpHorizontalSpeed
            }

            if (flags.HasAnyFlag(MovementFlagWotLK.SplineElevation))
                packet.ReadFloat(); // info.SplineElevation
            return flags;
        }

        void ReadMovementUpdateBlock(PacketIn packet, Object obj)
        {
            packet.ReadByte();
            UpdateFlag flags = (UpdateFlag)packet.ReadUInt16();


            if (flags.HasAnyFlag(UpdateFlag.Self))
            {
                player = obj;
                movementMgr.SetPlayer(player);
                combatMgr.SetPlayer(player);
            }

            if (flags.HasAnyFlag(UpdateFlag.Living))
            {
                var moveFlags = ReadMovementInfoLegacy(packet, obj);

                packet.ReadFloat(); // moveInfo.WalkSpeed = 
                packet.ReadFloat(); // moveInfo.RunSpeed = 
                packet.ReadFloat(); // moveInfo.RunBackSpeed = 
                packet.ReadFloat(); // moveInfo.SwimSpeed = 
                packet.ReadFloat(); // moveInfo.SwimBackSpeed = 

                packet.ReadFloat(); // moveInfo.FlightSpeed = 
                packet.ReadFloat(); // moveInfo.FlightBackSpeed = 

                packet.ReadFloat(); // moveInfo.TurnRate = 

                packet.ReadFloat(); // moveInfo.PitchRate = 

                if (moveFlags.HasAnyFlag(MovementFlagWotLK.SplineEnabled))
                {
                    SplineFlagWotLK splineFlags = (SplineFlagWotLK)packet.ReadUInt32();

                    if (splineFlags.HasAnyFlag(SplineFlagWotLK.FinalTarget))
                    {
                        UnpackGuid(packet);
                    }
                    else if (splineFlags.HasAnyFlag(SplineFlagWotLK.FinalOrientation))
                    {
                        packet.ReadFloat(); // monsterMove.FinalOrientation = 
                    }
                    else if (splineFlags.HasAnyFlag(SplineFlagWotLK.FinalPoint))
                    {
                        packet.ReadFloat();   // monsterMove.FinalFacingSpot = 
                        packet.ReadFloat();
                        packet.ReadFloat();
                    }

                    packet.ReadUInt32();    // monsterMove.SplineTime = 
                    packet.ReadUInt32();    // monsterMove.SplineTimeFull = 
                    packet.ReadUInt32();    // monsterMove.SplineId = 


                    packet.ReadFloat(); // Spline Duration Multiplier
                    packet.ReadFloat(); // Spline Duration Multiplier Next
                    packet.ReadInt32(); // Spline Vertical Acceleration
                    packet.ReadInt32(); // Spline Start Time               

                    var splineCount = packet.ReadUInt32();
                    for (var i = 0; i < splineCount; i++)
                    {
                        packet.ReadFloat();
                        packet.ReadFloat();
                        packet.ReadFloat();
                    }


                    packet.ReadByte(); // monsterMove.SplineMode = 

                    packet.ReadFloat();   // monsterMove.EndPosition = 
                    packet.ReadFloat();
                    packet.ReadFloat();
                }
            }
            else // !UpdateFlag.Living
            {
                if (flags.HasAnyFlag(UpdateFlag.GOPosition))
                {
                    UnpackGuid(packet);
                    packet.ReadFloat();   // moveInfo.Position = 
                    packet.ReadFloat();
                    packet.ReadFloat();
                    packet.ReadFloat();   // moveInfo.TransportOffset = 
                    packet.ReadFloat();
                    packet.ReadFloat();
                    packet.ReadFloat(); // moveInfo.Orientation = 
                    packet.ReadFloat(); // moveInfo.CorpseOrientation = 
                }
                else if (flags.HasAnyFlag(UpdateFlag.StationaryObject))
                {
                    packet.ReadFloat();   // moveInfo.Position = 
                    packet.ReadFloat();
                    packet.ReadFloat();
                    packet.ReadFloat();     // moveInfo.Orientation = 
                }
            }

            if (flags.HasAnyFlag(UpdateFlag.LowGuid))
                packet.ReadUInt32();

            if (flags.HasAnyFlag(UpdateFlag.HighGuid))
                packet.ReadUInt32();

            if (flags.HasAnyFlag(UpdateFlag.AttackingTarget))
            {
                UnpackGuid(packet);
            }

            if (flags.HasAnyFlag(UpdateFlag.Transport))
            {
                packet.ReadUInt32();    // uint transportPathTimer = 

            }

            if (flags.HasAnyFlag(UpdateFlag.Vehicle))
            {
                packet.ReadUInt32();    // uint vehicleId = 
                packet.ReadFloat();     // float vehicleOrientation = 
            }

            if (flags.HasAnyFlag(UpdateFlag.GORotation))
            {
                var rotation = packet.ReadUInt64();
            }
        }

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

            foreach (Object obj in ObjectMgr.GetInstance().getObjectArray())
            {
                if (obj.Fields != null)
                {
                    if (obj.Fields[(int)UpdateFields.OBJECT_FIELD_ENTRY] == entry.entry)
                    {
                        obj.Name = entry.name;
                        ObjectMgr.GetInstance().updateObject(obj);
                        Console.WriteLine("Handle_CreatureQuery " + obj.Name);
                    }
                }
            }
        }


        [PacketHandlerAtribute(WorldServerOpCode.SMSG_NAME_QUERY_RESPONSE)]
        public void Handle_NameQuery(PacketIn packet)
        {
            WoWGuid guid = new WoWGuid(UnpackGuid(packet));
            byte nameKnown = packet.ReadByte();
            if (nameKnown == 1)
                return;
            string name = packet.ReadString();
            packet.ReadByte();
            Race Race = (Race)packet.ReadByte();
            Gender Gender = (Gender)packet.ReadByte();
            Classname Class = (Classname)packet.ReadByte();

            if (ObjectMgr.GetInstance().objectExists(guid))    // Update existing Object
            {
                Object obj = ObjectMgr.GetInstance().getObject(guid);
                obj.Name = name;
                ObjectMgr.GetInstance().updateObject(obj);
                Console.WriteLine("Name updated for " + name);
            }
            else                // Create new Object        -- FIXME: Add to new 'names only' list?
            {
                Object obj = new Object(guid);
                obj.Name = name;
                ObjectMgr.GetInstance().addObject(obj);
                Console.WriteLine("Name inserted for " + name);
            }
        }

    }

}

