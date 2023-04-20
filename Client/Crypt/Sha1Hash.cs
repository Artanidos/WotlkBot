/*
 * Copyright ¸ 2005 Kele (fooleau@gmail.com)
 * This library is free software; you can redistribute it and/or 
 * modify it under the terms of the GNU Lesser General Public 
 * License version 2.1 as published by the Free Software Foundation
 * (the "LGPL").
 * This software is distributed on an "AS IS" basis, WITHOUT WARRANTY
 * OF ANY KIND, either express or implied.
 */
using System;
using System.Text;
using System.Security.Cryptography;

namespace WotlkClient.Crypt
{
	/// <summary>
	/// A wrapper for .NET's SHA1 class
	/// This is designed to be compatable with my initial implementation
	/// </summary>
	public class Sha1Hash
	{
		private SHA1 mSha;
		private static byte[] ZeroArray = new byte[0];

		public Sha1Hash()
		{
			mSha = SHA1.Create();
		}

        public void Initialize()
        {
            mSha.Initialize();
        }

		public void Update(byte[] Data)
		{
			mSha.TransformBlock(Data, 0, Data.Length, Data, 0);
		}
		
		public void Update(string s)
		{
			Update(Encoding.Default.GetBytes(s));
		}

		public void Update(Int32 data)
		{
			Update(BitConverter.GetBytes(data));
		}

		public void Update(UInt32 data)
		{
			Update(BitConverter.GetBytes(data));
		}
		
		public byte[] Final()
		{
			mSha.TransformFinalBlock(ZeroArray, 0, 0);
			return mSha.Hash;
		}

		public byte[] Final(byte[] Data)
		{
			mSha.TransformFinalBlock(Data, 0, Data.Length);
			return mSha.Hash;
		}
	}
}
