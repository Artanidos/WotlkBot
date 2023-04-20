using System;
using System.Collections.Generic;
using System.Text;

using WotlkClient.Shared;

namespace WotlkClient.Terrain
{
    public class Vect3D
    {
        private float x, y, z;

        public Vect3D(float cx, float cy, float cz)
        {
            x = cx; y = cy; z = cz;
        }

        public Vect3D(Coordinate from, Coordinate to)
        {
            x = to.X - from.X;
            y = to.Y - from.Y;
            z = to.Z - from.Z;
        }

        public float Length
        {
            get { return (float)Math.Sqrt(x * x + y * y + z * z); }
        }

        // Returns length of Vector, without performing square root. Useful if you just want
        // to compare vector sizes and don't want to take the performance hit.
        public float LengthFast
        {
            get { return (x * x + y * y + z * z); }
        }

        public override String ToString()
        {
            return String.Format("xyz = [{0}, {1}, {2}]", x, y, z);
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
    }
}
