using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WotlkClient.Terrain
{
    public static class Extensions
    {

        public static T ReadStruct<T>(this BinaryReader binReader) where T : struct
        {
            byte[] rawData = binReader.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            T returnObject = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return returnObject;
        }

        public static string ReadCString(this BinaryReader binReader)
        {
            StringBuilder str = new StringBuilder();
            char c;
            while ((c = binReader.ReadChar()) != '\0')
            {
                str.Append(c);
            }

            return str.ToString();
        }

        public static byte PeekByte(this BinaryReader binReader)
        {
            byte b = binReader.ReadByte();
            binReader.BaseStream.Position -= 1;
            return b;
        }


        public static string ReadFixedReversedAsciiString(this BinaryReader binReader, int size)
        {
            char[] chars = new char[size];//binReader.ReadChars(size);
            for (int i = 0; i < size; i++)
            {
                chars[i] = (char)binReader.ReadByte();
            }
            Reverse(chars);
            return new string(chars);
        }

        public static void Reverse<T>(this T[] array)
        {
            int length = array.Length;
            for (int i = 0; i < length / 2; i++)
            {
                T temp = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = temp;
            }
        }
    }
}
