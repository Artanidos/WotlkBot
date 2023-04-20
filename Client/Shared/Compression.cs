/*
 * Copyright ¸ 2005 Kele (fooleau@gmail.com)
 * This library is free software; you can redistribute it and/or 
 * modify it under the terms of the GNU Lesser General Public 
 * License version 2.1 as published by the Free Software Foundation
 * (the "LGPL").
 * This software is distributed on an "AS IS" basis, WITHOUT WARRANTY
 * OF ANY KIND, either express or implied.
 */
// created on 05/07/2004 at 12:18
// Version 1.01 - Decompress now keeps reading until it gets all expected data
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace WotlkClient.Shared
{
	public class Compression
	{
		// Length = decompressed length
		public static byte[] Decompress(int Length, byte[] Data)
		{
			byte[] Output = new byte[Length];
			Stream s = new InflaterInputStream(new MemoryStream(Data));
			int Offset = 0;
			while(true)
			{
				int size = s.Read(Output, Offset, Length);
				if (size == Length) break;
				Offset += size;
				Length -= size;
			}
			return Output;
		}

		public static void Decompress(int Length, byte[] Data, string Filename)
		{
			byte[] Output = Decompress(Length, Data);
			FileStream fs = new FileStream(Filename, FileMode.Create, FileAccess.Write);
			fs.Write(Output, 0, Length);
 			fs.Close();
		}

		public static byte[] Compress(byte[] Data)
		{
			MemoryStream ms = new MemoryStream();
			Stream s = new DeflaterOutputStream(ms);
			s.Write(Data, 0, Data.Length);
			s.Close();
			return ms.ToArray();
		}
	}
}
