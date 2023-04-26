using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WotlkClient.Constants
{
    public enum UpdateTypeModern
    {
        Values = 0,
        CreateObject1 = 1,
        CreateObject2 = 2,
    }

    public enum HighGuid
    {
        Item           = 0x4000,
        Container      = 0x4000,
        Player         = 0x0000,
        GameObject     = 0xF110,
        Transport      = 0xF120,
        Unit           = 0xF130,
        Pet            = 0xF140,
        Vehicle        = 0xF150,
        DynamicObject  = 0xF100,
        Corpse         = 0xF101,
        Mo_Transport   = 0x1FC0,
        Instance       = 0x1F40,
        Group          = 0x1F50,
    }

    public enum Classname : uint
    {
        Warrior = 1,
        Paladin = 2,
        Hunter = 3,
        Rogue = 4,
        Priest = 5,
        DeathKnight = 6,
        Shaman = 7,
        Mage = 8,
        Warlock = 9,
        //??	= 10
        Druid = 11
    }

    public enum Race
    {
        Human = 1,
        Orc = 2,
        Dwarf = 3,
        NightElf = 4,
        Undead = 5,
        Tauren = 6,
        Gnome = 7,
        Troll = 8,
        Goblin = 9,
        BloodElf = 10,
        Draenei = 11,
        FelOrc = 12,
        Naga = 13,
        Broken = 14,
        Skeleton = 15
    }

    public enum Gender : int
    {
        Male = 0,
        Female = 1,
        Neutral = 2
    }

    public enum UpdateType
    {
        /// <summary>
        /// Update type that update only object field values.
        /// </summary>
        Values = 0,
        /// <summary>
        /// Update type that update only object movement.
        /// </summary>
        Movement = 1,
        /// <summary>
        /// Update type that create an object (full update).
        /// </summary>
        Create = 2,
        /// <summary>
        /// Update type that create an object (gull update, self use).
        /// </summary>
        CreateSelf = 3,
        /// <summary>
        /// Update type that update only objects out of range.
        /// </summary>
        OutOfRange = 4,
        /// <summary>
        /// Update type that update only near objects.
        /// </summary>
        NearObjects = 5,
    }

    public struct Entry
    {
        public UInt32 Type;
        public UInt32 DisplayID;
        public UInt32 entry;
        public string name;
        public byte[] blarg;
        public string subname;
        public UInt32 flags;
        public UInt32 subtype;
        public UInt32 family;
        public UInt32 rank;
    }

    public enum ObjectType
    {
        Object = 0,
        Item = 1,
        Container = 2,
        Unit = 3,
        Player = 4,
        GameObject = 5,
        DynamicObject = 6,
        Corpse = 7,
        AIGroup = 8,
        AreaTrigger = 9,
        Count,
        None = 0xFF
    }

}
