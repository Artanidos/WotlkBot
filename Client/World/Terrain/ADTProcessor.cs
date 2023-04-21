using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

using WotlkClient.Shared; 

namespace WotlkClient.Terrain
{
    public class ADT
    {
        // Data read in from ADT file
        private MVER mver;                  // ADT File Version
        private MHDR mhdr;                  // ADT File Header
        private MCIN[] mcin_array;          // Array of indices to MapChunks
        private List<String> wmoFiles;      // List of WMO Files located on this MapTile
        private MDDF[] doodadLocations;     // List of Doodad Locations on this MapTile
        private MODF[] wmoLocations;        // List of WMO Locations on this MapTile
        protected MCNK[][] mapChunkTable;   // 16x16 MapChunk Table
        private FileStream adtStream;
        string prefix;
 
        // ** Removed because this constructor isn't neccessary.
        public ADT(String mapname, int x, int z, string _prefix)
        {
            string filename = String.Format(@"ADT\{0}\{0}_{1}_{2}.adt", mapname, x, z);
            adtStream = new FileStream(filename, FileMode.Open);
            parseFile();
            prefix = _prefix;
        }

        private void parseFile()
        {
            FileStream ms = adtStream;
            if (ms == null)
            {
                throw new Exception("Stream null!");
            }
            BinaryReader bin = new BinaryReader(ms);

            BlizChunkHeader tempHeader;
            long pos = 0;

            // Read bytes from the stream until we run out
            while (pos < ms.Length)
            {
                // Advance to the next Chunk
                ms.Position = pos;

                // Read in Chunk Header Name
                tempHeader = new BlizChunkHeader(bin.ReadChars(4), bin.ReadUInt32());
                tempHeader.Flip();

                // Set pos to the location of the next Chunk
                pos = ms.Position + tempHeader.Size;

                if (tempHeader.Is("MVER"))   // ADT File Version
                {
                    mver = new MVER();
                    mver.version = bin.ReadUInt32();

                    continue;
                }

                if (tempHeader.Is("MHDR"))  // ADT File Header
                {
                    mhdr = new MHDR();
                    mhdr.pad = bin.ReadUInt32();
                    mhdr.offsInfo = bin.ReadUInt32();
                    mhdr.offsTex = bin.ReadUInt32();
                    mhdr.offsModels = bin.ReadUInt32();
                    mhdr.offsModelsIds = bin.ReadUInt32();
                    mhdr.offsMapObejcts = bin.ReadUInt32();
                    mhdr.offsMapObejctsIds = bin.ReadUInt32();
                    mhdr.offsDoodsDef = bin.ReadUInt32();
                    mhdr.offsObjectsDef = bin.ReadUInt32();
                    mhdr.pad1 = bin.ReadUInt32();
                    mhdr.pad2 = bin.ReadUInt32();
                    mhdr.pad3 = bin.ReadUInt32();
                    mhdr.pad4 = bin.ReadUInt32();
                    mhdr.pad5 = bin.ReadUInt32();
                    mhdr.pad6 = bin.ReadUInt32();
                    mhdr.pad7 = bin.ReadUInt32();

                    continue;
                }

                if (tempHeader.Is("MCIN"))  // Index for MCNK chunks.
                {
                    if (tempHeader.Size != 256 * 16)
                        throw new Exception("MCIN Chunk is short??");

                    mcin_array = new MCIN[256];

                    // Read in the 256 records
                    for (int i = 0; i < 256; i++)
                    {
                        mcin_array[i].MCNK_offset = bin.ReadUInt32();
                        mcin_array[i].MCNK_size = bin.ReadUInt32();
                        mcin_array[i].flags = bin.ReadUInt32();
                        mcin_array[i].asyncID = bin.ReadUInt32();
                    }

                    continue;
                }

                if (tempHeader.Is("MTEX"))  // List of texture filenames used by the terrain in this map tile.
                {
                    // Not needed.
                    continue;
                }

                if (tempHeader.Is("MMDX")) // List of filenames for M2 models that appear in this map tile.
                {
                    // Not needed.
                    continue;
                }

                if (tempHeader.Is("MMID"))  // Lists the relative offsets of string beginnings in the above MMDX chunk. 
                {
                    // Not needed.
                    continue;
                }

                if (tempHeader.Is("MWMO"))  // List of filenames for WMOs (world map objects) that appear in this map tile.
                {
                    byte[] wmoFilesChunk = bin.ReadBytes((int)tempHeader.Size);

                    wmoFiles = new List<String>();

                    StringBuilder str = new StringBuilder();

                    // Convert szString's to a List<String>.
                    for (int i = 0; i < wmoFilesChunk.Length; i++)
                    {
                        if (wmoFilesChunk[i] == '\0')
                        {
                            if (str.Length > 1)
                                wmoFiles.Add(str.ToString());
                            str = new StringBuilder();
                        }
                        else
                            str.Append((char)wmoFilesChunk[i]);
                    }

                    continue;
                }

                if (tempHeader.Is("MWID"))  // Lists the relative offsets of string beginnings in the above MWWO chunk.
                {
                    // Not needed.
                    continue;
                }

                if (tempHeader.Is("MDDF"))  // Placement information for doodads (M2 models). 
                {
                    uint num = tempHeader.Size / 32;

                    doodadLocations = new MDDF[num];

                    for (int i = 0; i < num; i++)
                    {
                        doodadLocations[i].nameId = bin.ReadUInt32();
                        doodadLocations[i].uniqueId = bin.ReadUInt32();
                        doodadLocations[i].coord = new Coordinate(bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle());
                    }

                    continue;
                }

                if (tempHeader.Is("MODF"))  // Placement information for WMOs.
                {
                    uint num = tempHeader.Size / 64;

                    wmoLocations = new MODF[num];

                    for (int i = 0; i < num; i++)
                    {
                        wmoLocations[i].nameId = bin.ReadUInt32();
                        wmoLocations[i].uniqueId = bin.ReadUInt32();
                        wmoLocations[i].coord = new Coordinate(bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle());
                        wmoLocations[i].orientation = new Vect3D(bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle());
                        wmoLocations[i].coord2 = new Coordinate(bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle());
                        wmoLocations[i].coord3 = new Coordinate(bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle());
                        wmoLocations[i].flags = bin.ReadUInt32();
                        wmoLocations[i].doodadSet = bin.ReadUInt16();
                        wmoLocations[i].nameSet = bin.ReadUInt16();
                    }

                    continue;
                }

                if (tempHeader.Is("MCNK")) // || tempHeader.Is("MCVT") || tempHeader.Is("MCNR") || tempHeader.Is("MCLY") || tempHeader.Is("MCRF") || tempHeader.Is("MCSH") || tempHeader.Is("MCAL") || tempHeader.Is("MCLQ") || tempHeader.Is("MCSE"))
                {
                    // Skip these. They are read in afterwards.
                    continue;
                }

                // If we're still down here, we got a problem
                throw new Exception(String.Format("ADTFile: Woah. Got a header of {0}. Don't know how to deal with this, bailing out.", tempHeader.ToString()));
            }

            // Read in Map Chunks
            mapChunkTable = new MCNK[16][];

            for (int i = 0; i < 16; i++)
            {
                mapChunkTable[i] = new MCNK[16];

                for (int j = 0; j < 16; j++)
                {
                    int index = i * 16 + j;
                    Log.WriteLine(LogType.Terrain,  "Parsing MCNK Chunk #{0} [{1}, {2}]", prefix, index, i, j);
                    mapChunkTable[i][j] = parseMapChunk(ms, mcin_array[index].MCNK_offset, mcin_array[index].MCNK_size);
                }
            }

            
        }

        // Read in an MCNK chunk at supplied offset.
        private MCNK parseMapChunk(FileStream ms, uint offset, uint size)
        {
            BinaryReader bin = new BinaryReader(ms);
            ms.Position = offset;

            BlizChunkHeader tempHeader = new BlizChunkHeader(bin.ReadChars(4), bin.ReadUInt32());
            tempHeader.Flip();

            if (!tempHeader.Is("MCNK"))
                throw new Exception("This was supposed to be an MCNK chunk. Wtf?");

            long lastpos = ms.Position + tempHeader.Size;

            MCNK mapChunk = new MCNK();

            mapChunk.flags = bin.ReadUInt32();
            mapChunk.ix = bin.ReadUInt32();
            mapChunk.iy = bin.ReadUInt32();
            mapChunk.nLayers = bin.ReadUInt32();
            mapChunk.nDoodadRefs = bin.ReadUInt32();
            mapChunk.ofsHeight = bin.ReadUInt32();
            mapChunk.ofsNormal = bin.ReadUInt32();
            mapChunk.ofsLayer = bin.ReadUInt32();
            mapChunk.ofsRefs = bin.ReadUInt32();
            mapChunk.ofsAlpha = bin.ReadUInt32();
            mapChunk.sizeAlpha = bin.ReadUInt32();
            mapChunk.ofsShadow = bin.ReadUInt32();
            mapChunk.sizeShadow = bin.ReadUInt32();
            mapChunk.areaid = bin.ReadUInt32();
            mapChunk.nMapObjRefs = bin.ReadUInt32();
            mapChunk.holes = bin.ReadUInt32();
            mapChunk.s1 = bin.ReadUInt16();
            mapChunk.s2 = bin.ReadUInt16();
            mapChunk.d1 = bin.ReadUInt32();
            mapChunk.d2 = bin.ReadUInt32();
            mapChunk.d3 = bin.ReadUInt32();
            mapChunk.predTex = bin.ReadUInt32();
            mapChunk.nEffectDoodad = bin.ReadUInt32();
            mapChunk.ofsSndEmitters = bin.ReadUInt32();
            mapChunk.nSndEmitters = bin.ReadUInt32();
            mapChunk.ofsLiquid = bin.ReadUInt32();
            mapChunk.sizeLiquid = bin.ReadUInt32();
            mapChunk.zpos = bin.ReadSingle();
            mapChunk.xpos = bin.ReadSingle();
            mapChunk.ypos = bin.ReadSingle();
            mapChunk.textureId = bin.ReadUInt32();
            mapChunk.props = bin.ReadUInt32();
            mapChunk.effectId = bin.ReadUInt32();

            // Parse this MapChunk's SubChunks!
            parseMapChunkSubChunks(ref mapChunk, ms, lastpos);

            return mapChunk;
        }

        private void parseMapChunkSubChunks(ref MCNK mapChunk, FileStream ms, long lastpos)
        {
            BinaryReader bin = new BinaryReader(ms);

            BlizChunkHeader tempHeader;
            long pos = ms.Position;

            // Read bytes from the stream until we run out
            while (pos < lastpos)
            {
                // Advance to the next Chunk
                ms.Position = pos;

                // Read in Chunk Header Name
                tempHeader = new BlizChunkHeader(bin.ReadChars(4), bin.ReadUInt32());
                tempHeader.Flip();

                // Set pos to the location of the next Chunk
                pos = ms.Position + tempHeader.Size;

                if (tempHeader.Is("MCVT"))  // These are the actual height values for the 9x9+8x8 vertices.
                {
                    mapChunk.VerticesOuter = new float[9][];
                    mapChunk.VerticesInner = new float[8][];

                    for (int i = 0; i < 9; i++)
                    {
                        mapChunk.VerticesOuter[i] = new float[9];

                        for (int j = 0; j < 9; j++)
                        {
                            mapChunk.VerticesOuter[i][j] = bin.ReadSingle();
                        }

                        if (i == 8) continue;

                        mapChunk.VerticesInner[i] = new float[8];

                        for (int j = 0; j < 8; j++)
                        {
                            mapChunk.VerticesInner[i][j] = bin.ReadSingle();
                        }
                    }

                    continue;
                }

                if (tempHeader.Is("MCNR"))  // Normal vectors for each vertex.
                {
                    pos = ms.Position + 0x1C0; // sizefix?
                    // Not needed.
                    continue;
                }

                if (tempHeader.Is("MCLY"))  // Texture layer definitions for this map chunk.
                {
                    // Not needed.
                    continue;
                }

                if (tempHeader.Is("MCRF"))  // Unknown. List of integers.
                {
                    // Not needed.
                    continue;
                }

                if (tempHeader.Is("MCSH"))  // Shadow map for static shadows on the terrain.
                {
                    // Not needed.
                    continue;
                }

                if (tempHeader.Is("MCAL"))  // Alpha maps for additional texture layers.
                {
                    // Not needed.
                    continue;
                }

                if (tempHeader.Is("MCLQ"))  // Water levels for this map chunk.
                {
                    mapChunk.Liquid = new MCLQ();

                    // I dunno. MCLQ header size lies. If MCSE is immidiately following, there's no water.
                    tempHeader = new BlizChunkHeader(bin.ReadChars(4), 0);
                    tempHeader.Flip();

                    if (tempHeader.Is("MCSE"))
                    {
                        mapChunk.Liquid.waterLevel = float.NaN;
                    }
                    else
                    {
                        // After reading water, we stop. I do NOT like this solution, but
                        // 1) We don't know much about MCLQ chunks.
                        // 2) The size field lies, saying its always 0. (no idea why)
                        // 3) I can't kludge this with a size fix, BECAUSE THE LENGTH VARIES?! Its in the area of 0x320, or 0x31F or 0x31E.
                        // 4) So stopping after this chunk is the best we can do; this is what wowmapview does in any case.
                        pos = lastpos;
                        ms.Seek(-4, SeekOrigin.Current); // Go back! Re-read the last 4 bytes as its actually a float not char[4].
                        mapChunk.Liquid.waterLevel = bin.ReadSingle();
                    }

                    continue;
                }

                if (tempHeader.Is("MCSE"))  // Sound emitters.
                {
                    // Not needed.
                    continue;
                }

                // If we're still down here, we got a problem
                throw new Exception(String.Format("ADTFile: Woah. Got a header of Sub-{0}. Don't know how to deal with this, bailing out.", tempHeader.ToString()));
            }
        }

        // MVER Chunk (ADT Version)
        private struct MVER
        {
            public UInt32 version;
        }

        // MHDR Chunk (ADT Header)
        private struct MHDR
        {
            public UInt32 pad;
            public UInt32 offsInfo;
            public UInt32 offsTex;
            public UInt32 offsModels;
            public UInt32 offsModelsIds;
            public UInt32 offsMapObejcts;
            public UInt32 offsMapObejctsIds;
            public UInt32 offsDoodsDef;
            public UInt32 offsObjectsDef;
            public UInt32 pad1;
            public UInt32 pad2;
            public UInt32 pad3;
            public UInt32 pad4;
            public UInt32 pad5;
            public UInt32 pad6;
            public UInt32 pad7;
        }

        // MCIN Chunk (Index Table for MCNK Chunks) (x256 of these)
        private struct MCIN
        {
            public UInt32 MCNK_offset;
            public UInt32 MCNK_size;
            public UInt32 flags;
            public UInt32 asyncID;
        }

        // MDDF Chunk (Doodad Placement)
        private struct MDDF
        {
            public UInt32 nameId;
            public UInt32 uniqueId;
            public Coordinate coord;
        }

        // MODF Chunk (WMO Placement)
        private struct MODF
        {
            public UInt32 nameId;
            public UInt32 uniqueId;
            public Coordinate coord;
            public Vect3D orientation;
            public Coordinate coord2;
            public Coordinate coord3;
            public UInt32 flags;
            public UInt16 doodadSet;
            public UInt16 nameSet;
        }

        // MCNK (MapChunk)
        protected struct MCNK
        {
            public UInt32 flags;
            public UInt32 ix;
            public UInt32 iy;
            public UInt32 nLayers;
            public UInt32 nDoodadRefs;
            public UInt32 ofsHeight;
            public UInt32 ofsNormal;
            public UInt32 ofsLayer;
            public UInt32 ofsRefs;
            public UInt32 ofsAlpha;
            public UInt32 sizeAlpha;
            public UInt32 ofsShadow;
            public UInt32 sizeShadow;
            public UInt32 areaid;
            public UInt32 nMapObjRefs;
            public UInt32 holes;
            public UInt16 s1;
            public UInt16 s2;
            public UInt32 d1;
            public UInt32 d2;
            public UInt32 d3;
            public UInt32 predTex;
            public UInt32 nEffectDoodad;
            public UInt32 ofsSndEmitters;
            public UInt32 nSndEmitters;
            public UInt32 ofsLiquid;
            public UInt32 sizeLiquid;
            public float zpos;
            public float xpos;
            public float ypos;
            public UInt32 textureId;
            public UInt32 props;
            public UInt32 effectId;

            public float[][] VerticesOuter;     // 9x9 Vertices (outer)
            public float[][] VerticesInner;     // 8x8 Vertices (inner)

            public MCLQ Liquid;
        }

        // MCLQ (Liquids)
        protected struct MCLQ
        {
            public float waterLevel;
            // We aren't reading the rest of the data in :(
        }
    }


}
