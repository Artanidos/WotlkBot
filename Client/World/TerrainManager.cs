using System;
using System.Collections.Generic;
using System.Text;

using WotlkClient.Shared;

namespace WotlkClient.Terrain
{
    /// <summary>Manages Terrain Data. Provides numerous useful methods to query terrain data, and does so by looking up (and if nessessary, loading in) the appropriate maptile.</summary>
    public class TerrainMgr
    {
        private List<MapTile> mapTiles;


        static float TILESIZE = 533.33333f;
        static float ZEROPOINT = 32.0f * TILESIZE;
        private UInt32 MapId;
        string prefix;
        public TerrainMgr(string _prefix)
        {
            mapTiles = new List<MapTile>();
            prefix = _prefix;
        }

        public void ChangeMap(UInt32 mapId)
        {
            MapId = mapId;
        }

        public Coordinate getZ(Coordinate c)
        {
            // Make a new coordinate object so we don't modify the original
            Coordinate h = new Coordinate(c.X, c.Y, c.Z, c.O);
            h.Z = getZ(c.X, c.Y);
            return h;
        }

        public float getZ(float x, float y)
        {
            doMaintenance(false);

            int TileX = (int)(((0f - y) + ZEROPOINT) / TILESIZE);
            int TileZ = (int)(((0f - x) + ZEROPOINT) / TILESIZE);

            // Find the maptile on the list of loaded tiles.
            MapTile tile = findTile(TileX, TileZ);

            // Ask the maptile to get z for x,y
            return tile.getZ(x, y);
        }

        public float getWaterHeight(float x, float y)
        {
            doMaintenance(false);

            int TileX = (int)(((0f - y) + ZEROPOINT) / TILESIZE);
            int TileZ = (int)(((0f - x) + ZEROPOINT) / TILESIZE);

            // Find the maptile on the list of loaded tiles.
            MapTile tile = findTile(TileX, TileZ);

            // Ask the maptile to get z for x,y
            return tile.getWaterHeight(x, y);
        }

        static public float CalculateDistance(Coordinate c1, Coordinate c2)
        {
            float dX = c2.X - c1.X;
            float dY = c2.Y - c1.Y;

            return (float)Math.Sqrt(dX * dX + dY * dY);

        }

        // Credit to ascent - I'm lazy :P
        static public float CalculateAngle(Coordinate c1, Coordinate c2)
        {
            float dx = c2.X - c1.X;
            float dy = c2.Y - c1.Y;
            double angle = 0.0f;

            // Calculate angle
            if (dx == 0.0)
            {
                if (dy == 0.0)
                    angle = 0.0;
                else if (dy > 0.0)
                    angle = Math.PI * 0.5;
                else
                    angle = Math.PI * 3.0 * 0.5;
            }
            else if (dy == 0.0)
            {
                if (dx > 0.0)
                    angle = 0.0;
                else
                    angle = Math.PI;
            }
            else
            {
                if (dx < 0.0)
                    angle = Math.Atan(dy / dx) + Math.PI;
                else if (dy < 0.0)
                    angle = Math.Atan(dy / dx) + (2 * Math.PI);
                else
                    angle = Math.Atan(dy / dx);
            }

            return (float)angle;
        }


        // Notify the terrain manager that we have just zoned.
        public void zoned()
        {
            // If we just zoned to a different map, do maintenance and flush the current tile list
            doMaintenance(true);
        }

        // Finds Maptile x,z on the list
        private MapTile findTile(int x, int z)
        {
            foreach (MapTile mapTile in mapTiles)
            {
                if (mapTile.X == x && mapTile.Z == z)
                    return mapTile;
            }

            // Wasn't a tile we have currently Loaded? Load it in!!
            return loadTile(x, z);
        }

        // Loads a maptile in
        private MapTile loadTile(int x, int z)
        {
            //String mapname = BoogieCore.mapTable.getMapName(BoogieCore.world.getMapID());
            MapTable map = new MapTable(prefix);

            string mapname = map.getMapName(MapId);
            MapTile tile = new MapTile(mapname, x, z, prefix);
            mapTiles.Add(tile);
            return tile;
        }

        // Do maintenance
        private void doMaintenance(Boolean flush)
        {
            // Delete all maptiles off the list
            if (flush)
            {
                mapTiles = new List<MapTile>();
            }

            // If the list is getting long
            if (mapTiles.Count > 100)
            {
                // Prune it.
                mapTiles = new List<MapTile>();
            }
        }

        // DEBUG METHODS /////////////////////////////////////////////////////////////////
        public int DEBUG_TileCount()
        {
            return mapTiles.Count;
        }
    }
}
