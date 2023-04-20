using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WotlkClient.Network;
using WotlkClient.Constants;

namespace WotlkClient.Shared
{
    public enum HighId : ushort
    {
        Unit = 0xF130,
        Unit2 = 0xF430,
        Unit3 = 0xF730,

        Pet = 0xF140,

        MoTransport = 0x1FC0,
        Transport = 0xF120,                       // blizz F120 (for GAMEOBJECT_TYPE_TRANSPORT)

        GameObject = 0xF110,
        GameObject2 = 0xF410,
        GameObject3 = 0xF710,
        GameObject4 = 0xF009,

        Item = 0x4000,                       // blizz 4000
        Player = 0x0000,                       // blizz 0000
        DynamicObject = 0xF100,                       // blizz F100

        Corpse = 0xF101,                       // blizz F100
        Corpse2 = 0xF400,
        Corpse3 = 0xF700,
    }

    // pre 2.4:
    //public enum OldHighId : uint
    //{
    //    Player = 0x00000000,
    //    Corpse = 0x00000001,
    //    Unit = 0x00000002,
    //    GameObject = 0x00000003,
    //    DynamicObject = 0x00000004,
    //    Item = 0x00000005,
    //    Container = 0x00000006,
    //    Transport = 0x00000007,
    //}

    public enum HighGuid8 : byte
    {
        Item = 0x40,
        Flag_F1 = 0xF1,
        Flag_F4 = 0xF4,
        Flag_F7 = 0xF7
    }

    public enum HighGuidType : byte
    {
        /// <summary>
        /// Also Player
        /// </summary>
        NoEntry = 0x00,
        GameObject = 0x10,
        Transport = 0x20,
        Unit = 0x30,
        Pet = 0x40,

        MapObjectTransport = 0xC0,

    }

    public struct EntityId
    {
        const uint LowMask = 0xFFFFFF;
        const uint EntryMask = 0xFFFFFF;
        const uint HighMask = 0xFFFF0000;
        const uint High7Mask = 0x00FF0000;
        const uint High8Mask = 0xFF000000;


        public static readonly EntityId Zero = new EntityId(0);
        public static readonly byte[] ZeroRaw = new byte[8];

 
        public ulong Full;
 
        private uint m_low;
 
        private uint m_entry;
 
        private uint m_high;

        public EntityId(byte[] fullRaw)
        {
            m_low = 0;
            m_high = 0;
            m_entry = 0;
            Full = BitConverter.ToUInt64(fullRaw, 0);
        }

        public EntityId(ulong full)
        {
            m_low = 0;
            m_high = 0;
            m_entry = 0;
            Full = full;
        }

        public EntityId(uint low, uint high)
        {
            Full = 0;
            m_entry = 0;
            m_low = low;
            m_high = high;
        }

        public EntityId(uint low, uint entry, HighId high)
        {
            Full = 0;
            m_high = 0;
            m_low = low;
            m_entry = entry;
            High = high;
        }

        public uint Low
        {
            get
            {
                return m_low & LowMask;
            }

            private set
            {
                m_low &= ~LowMask;
                m_low |= (value & LowMask);
            }
        }

        public uint Entry
        {
            get
            {
                return m_entry & EntryMask;
            }
            private set
            {
                m_entry &= ~EntryMask;
                m_entry |= (value & EntryMask);
            }
        }

        public HighId High
        {
            get
            {
                return (HighId)(m_high >> 16);
            }
            private set
            {
                m_high &= ~HighMask;
                m_high |= ((uint)value) << 16;
            }
        }

        public bool HasEntry
        {
            get
            {
                //return ((m_high >> 16) & 0xFF) != 0;
                return SeventhByte != HighGuidType.NoEntry;
            }
        }

        public uint LowRaw
        {
            get { return m_low; }
        }

        public uint HighRaw
        {
            get { return m_high; }
        }

        public HighGuidType SeventhByte
        {
            get
            {
                return (HighGuidType)((m_high & High7Mask) >> 16);
            }
        }

        public HighGuid8 EighthByte
        {
            get
            {
                return (HighGuid8)((m_high & High8Mask) >> 24);
            }
        }

        public ObjectType objecType
        {
            get
            {
                switch (SeventhByte)
                {
                    case HighGuidType.NoEntry:
                        {
                            if (EighthByte == HighGuid8.Item)
                                return ObjectType.Item;
                            return ObjectType.Player;
                        }
                    case HighGuidType.GameObject: return ObjectType.GameObject;
                    case HighGuidType.MapObjectTransport: return ObjectType.GameObject;
                    case HighGuidType.Pet: return ObjectType.Unit;
                    case HighGuidType.Transport: return ObjectType.GameObject;
                    case HighGuidType.Unit: return ObjectType.Unit;
                }

                return ObjectType.Object;
            }
        }


        public static EntityId ReadPacked(ref PacketIn packet)
        {
            var mask = GetSetIndices(packet.ReadByte());
            
            byte[] rawId = new byte[8];
            foreach (var i in mask)
            {
                rawId[i] = packet.ReadByte();
            }
            return new EntityId(rawId);
        }

        public static uint[] GetSetIndices(uint flags)
        {
            List<uint> indices = new List<uint>();
            GetSetIndices(indices, flags);
            return indices.ToArray();
        }

        public static T[] GetSetIndicesEnum<T>(T flags)
        {
            List<uint> indices = new List<uint>();
            var uintFlags = (uint)Convert.ChangeType(flags, typeof(uint));
            GetSetIndices(indices, uintFlags);
            if (indices.Count == 0)
            {
                object box = (uint)0;
                return new T[] { (T)box };
            }
            else
            {
                var arr = new T[indices.Count];
                for (var i = 0; i < indices.Count; i++)
                {
                    var index = indices[i];
                    object box = (uint)(1 << (int)(index));
                    arr[i] = (T)box;
                }
                return arr;
            }
        }

        public static void GetSetIndices(List<uint> indices, uint flags)
        {
            for (uint i = 0; i < 32; i++)
            {
                if ((flags & 1 << (int)i) != 0)
                {
                    indices.Add(i);
                }
            }
        }

        public static implicit operator ulong(EntityId id)
        {
            return id.Full;
        }

        #region GetCorpseId

        public static EntityId GetCorpseId(uint id)
        {
            return new EntityId(id, 0, HighId.Corpse);
        }

        #endregion

        #region GetUnitId
        public static EntityId GetUnitId(uint id, uint entry)
        {
            return new EntityId(id, entry, HighId.Unit);
        }

        #endregion

        public static EntityId GetPlayerId(uint low)
        {
            return new EntityId(low, 0, HighId.Player);
        }

        public static EntityId GetItemId(uint low)
        {
            return new EntityId(low, 0, HighId.Item);
        }

        public static EntityId GetDynamicObjectId(uint low)
        {
            return new EntityId(low, 0, HighId.DynamicObject);
        }

        public static EntityId GetGameObjectId(uint low)
        {
            return new EntityId(low, 0, HighId.GameObject);
        }
    }

}
