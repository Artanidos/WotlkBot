using System;
using System.Collections.Generic;
using System.Text;

namespace WotlkClient.Terrain
{
    public class MapTile : ADT
    {
        private int TileX;
        private int TileZ;
        

        public MapTile(String mapname, int x, int z, string prefix) : base(mapname, x, z, prefix)
        {
            // Keep a note of what tile we are
            TileX = x;
            TileZ = z;
        }

        //public void getZ(Coordinate c)
        //{
        //    c.Z = getZ(c.X, c.Y);
        //}

        public float getZ(float x, float y)
        {
            float diff = 500.0f / 15.0f;
            float vdiff = diff / 8.0f;

            // x/y base coordinates for the top left most subtile (tile[0][0])
            float Xb = mapChunkTable[0][0].zpos;
            float Yb = mapChunkTable[0][0].xpos;

            int i = (int)Math.Abs((Xb - x) / diff);
            int j = (int)Math.Abs((Yb - y) / diff);

            if (i < 0 || i > 15 || j < 0 || j > 15)
                throw new Exception("The coordinates are NOT on this Tile.");

            float X = mapChunkTable[i][j].zpos; // X coordinate of SubTile
            float Y = mapChunkTable[i][j].xpos; // Y coordinate of SubTile
            float Z = mapChunkTable[i][j].ypos; // Base Height of SubTile

            // Get Vertex Coordinates
            int iv = (int)Math.Round((double)Math.Abs((X - x) / vdiff));
            int jv = (int)Math.Round((double)Math.Abs((Y - y) / vdiff));

            // Add the vertex height difference to the base height of the maptile, and return it!
            float ActualZ = Z + mapChunkTable[i][j].VerticesOuter[iv][jv];

            return ActualZ;
        }

        public float getWaterHeight(float x, float y)
        {
            float diff = 500.0f / 15.0f;
            float vdiff = diff / 8.0f;

            // x/y base coordinates for the top left most subtile (tile[0][0])
            float Xb = mapChunkTable[0][0].zpos;
            float Yb = mapChunkTable[0][0].xpos;

            int i = (int)Math.Abs((Xb - x) / diff);
            int j = (int)Math.Abs((Yb - y) / diff);

            if (i < 0 || i > 15 || j < 0 || j > 15)
                throw new Exception("The coordinates are NOT on this Tile.");

            return mapChunkTable[i][j].Liquid.waterLevel;
        }

        public int X
        {
            get { return TileX; }
        }

        public int Z
        {
            get { return TileZ; }
        }
    }
}
