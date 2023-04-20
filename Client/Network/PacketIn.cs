using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

using WotlkClient.Constants;

namespace WotlkClient.Network
{
    public class PacketIn : BinaryReader
    {

        public PacketId PacketId;
        private byte[] packetData;

        public PacketIn(byte[] data) : base(new MemoryStream(data))
        {
            packetData = data;
            PacketId = new PacketId((WorldServerOpCode)base.ReadUInt16());
            
        }

        public PacketIn(byte[] data, int i)
            : base(new MemoryStream(data))
        {
            packetData = data;
        }

        public PacketIn(byte[] data, bool logon)
            : base(new MemoryStream(data))
        {
            packetData = data;
            PacketId = new PacketId((LogonServerOpCode)ReadByte());
        }

		public override string ReadString()
		{
			StringBuilder sb = new StringBuilder();
			while (true)
			{
                byte b;
                //if (Remaining > 0)
                    b = ReadByte();
                //else
                //   b = 0;

				if (b == 0) break;
				sb.Append((char)b);
			}
			return sb.ToString();
		}

		public byte[] ReadRemaining()
		{
			MemoryStream ms = (MemoryStream)BaseStream;
			int Remaining = (int)(ms.Length - ms.Position);
			return ReadBytes(Remaining);
		}

		public int Remaining
		{
			get
			{
				MemoryStream ms = (MemoryStream)BaseStream;
				return (int)(ms.Length - ms.Position);
			}
            set
            {
                MemoryStream ms = (MemoryStream)BaseStream;
                if (value <= (ms.Length - ms.Position))
                    ms.Position = value;
            }
		}
        public float ReadFloat()
        {
            return System.BitConverter.ToSingle(ReadBytes(4), 0);
        }


		public byte[] ToArray()
		{
			return ((MemoryStream)BaseStream).ToArray();
		}

        public string ToHex()
        {
            StringBuilder hexDump = new StringBuilder();
            byte[] packetData = ToArray();

            hexDump.Append('\n');
            hexDump.Append("{Client<-Server} " + string.Format("Packet: ({0}) {1} PacketSize = {2}\n" +
                                                       "|------------------------------------------------|----------------|\n" +
                                                       "|00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F |0123456789ABCDEF|\n" +
                                                       "|------------------------------------------------|----------------|\n",
                                                       "0x" + ((short)PacketId.RawId).ToString("X4"), PacketId, packetData.Length));

            int end = 0 + packetData.Length;
            for (int i = 0; i < end; i += 16)
            {
                StringBuilder text = new StringBuilder();
                StringBuilder hex = new StringBuilder();
                hex.Append("|");

                for (int j = 0; j < 16; j++)
                {
                    if (j + i < end)
                    {
                        byte val = packetData[j + i];
                        hex.Append(packetData[j + i].ToString("X2"));
                        hex.Append(" ");
                        if (val >= 32 && val <= 127)
                        {
                            text.Append((char)val);
                        }
                        else
                        {
                            text.Append(".");
                        }
                    }
                    else
                    {
                        hex.Append("   ");
                        text.Append(" ");
                    }
                }
                hex.Append("|");
                hex.Append(text + "|");
                hex.Append('\n');
                hexDump.Append(hex.ToString());
            }

            hexDump.Append("-------------------------------------------------------------------");

            return hexDump.ToString();
        }
    }
}
