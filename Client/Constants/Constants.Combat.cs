using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WotlkClient.Constants
{
    public enum SpellTargetFlags : uint
    {
        Self = 0,
        SpellTargetFlag_Dynamic_0x1 = 0x1,
        Unit = 0x0002,
        SpellTargetFlag_Dynamic_0x4 = 0x4,
        SpellTargetFlag_Dynamic_0x8 = 0x8,
        Item = 0x10,
        SourceLocation = 0x20,
        DestinationLocation = 0x40,
        UnkObject_0x80 = 0x80,
        UnkUnit_0x100 = 0x100,
        PvPCorpse = 0x200,
        UnitCorpse = 0x400,
        Object = 0x800,
        TradeItem = 0x1000,
        String = 0x2000,
        /// <summary>
        /// For spells that open an object
        /// </summary>
        OpenObject = 0x4000,
        Corpse = 0x8000,
        SpellTargetFlag_Dynamic_0x10000 = 0x10000,
        Glyph = 0x20000,

        Flag_0x200000 = 0x200000,
    }

    public static class SpellEnumExtensions
    {
        public static bool Has(this SpellTargetFlags flags, SpellTargetFlags toCheck)
        {
            return (flags & toCheck) != 0;
        }
    }

}
