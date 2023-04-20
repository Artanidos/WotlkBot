/*
 * Copyright ¸ 2005 Kele (fooleau@gmail.com)
 * This library is free software; you can redistribute it and/or 
 * modify it under the terms of the GNU Lesser General public 
 * License version 2.1 as published by the Free Software Foundation
 * (the "LGPL").
 * This software is distributed on an "AS IS" basis, WITHOUT WARRANTY
 * OF ANY KIND, either express or implied.
 */

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

namespace WotlkClient.Crypt
{
	/// <summary>
	/// This class handles the encryption that is done on the headers of world server wws.
	/// The Key is 40 bytes long
	/// </summary>
	public class WoWCrypt
	{
		private bool mInitialised = false;
		
		// Encryption state
		private byte mEncPrev;
		public int mEncIndex;
		// Decryption state
		public byte mDecPrev; 
		public int mDecIndex;

		public byte[] mKey;

		public void Init(byte[] Key)
		{
			mKey = (byte[]) Key.Clone();
			mInitialised = true;
		}
        public byte GetDecPrev()
        {
            return mDecPrev;
        }
        public void SetDecPrev(byte SetTo)
        {
            mDecPrev = SetTo;
        }
        public int GetDecIndex()
        {
            return mDecIndex;
        }
        public void SetDecIndex(int SetTo)
        {
            mDecIndex = SetTo;
        }
		public void Decrypt(byte[] Data, int Length)
		{
            if (mInitialised == false) return;

			for(int i = 0; i < Length; ++i)
			{
				byte x = (byte)((Data[i] - mDecPrev) ^ mKey[mDecIndex]);
				++mDecIndex;
				mDecIndex %= mKey.Length;
				mDecPrev = Data[i];
				Data[i] = x;
			}
		}

		public void Encrypt(byte[] Data, int Length)
		{
			if (mInitialised == false) return;
			
			for(int i = 0; i < Length; ++i)
			{
				byte x = (byte)((Data[i] ^ mKey[mEncIndex]) + mEncPrev);
				++mEncIndex;
				mEncIndex %= mKey.Length;
				mEncPrev = x;
				Data[i] = x;
			}			
		}
	}
}
