using System;
using System.Collections.Generic;
using System.Text;

namespace WotlkClient.Shared
{
    public class Coordinate
    {
        public Coordinate(float cx, float cy, float cz)
        {
            x = cx; y = cy; z = cz;
        }

        public Coordinate(float cx, float cy, float cz, float co)
        {
            x = cx; y = cy; z = cz; o = co;
        }

        public Coordinate(String s)
        {
            String[] t = s.Split(new char[] {' '});

            if (t.Rank >= 0)
                float.TryParse(t[0], out x);
            if (t.Rank >= 1)
                float.TryParse(t[1], out y);
            if (t.Rank >= 2)
                float.TryParse(t[2], out z);
            if (t.Rank >= 3)
                float.TryParse(t[3], out o);
        }

        public override String ToString()
        {
            return String.Format("[{0}, {1}, {2}]", x, y, z);
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public float O
        {
            get { return o; }
            set { o = value; }
        }

        public float GetWoWX()
        {
            return (x + Coordinate.ZEROPOINT);  // No idea if this method is correct.
        }

        public float GetWoWY()
        {
            return (y + Coordinate.ZEROPOINT);  // No idea if this method is correct.
        }

        public int GetTileX()
        {
            return (int)(((0f - y) + Coordinate.ZEROPOINT) / Coordinate.TILESIZE);
        }

        public int GetTileZ()
        {
            return (int)(((0f - x) + Coordinate.ZEROPOINT) / Coordinate.TILESIZE);
        }

        private float x, y, z, o;

        private static float TILESIZE = 533.33333f;
        private static float ZEROPOINT = 32.0f * TILESIZE;
    } 
}
