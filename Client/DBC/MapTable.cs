using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WotlkClient.Shared
{
    class MapTable : DBCFile
    {
        public MapTable(string prefix)
            : base(@"Map.dbc", prefix)
        {
        }

        public string getMapName(uint mapId)
        {
            for (uint x = 0; x < wdbc_header.nRecords; x++)
            {
                uint id = getFieldAsUint32(x, 0);

                if (id == mapId)
                    return getStringForField(x, 1);
            }
            return null;
        }
    }
}
