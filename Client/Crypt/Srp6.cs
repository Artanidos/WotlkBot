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

namespace WotlkClient.Crypt
{
	/// <summary>
	/// Description of SRP6.
	/// </summary>
	public class Srp6
	{
		BigInteger N;
		BigInteger g;
		BigInteger k = new BigInteger(3);

		public Srp6(BigInteger N, BigInteger g)
		{
			this.N = N;
			this.g = g;
		}

		public static byte[] GetLogonHash(string Username, string Password)
		{
			Sha1Hash h = new Sha1Hash();
			string sI = String.Format("{0}:{1}", Username, Password.ToUpper());
			h.Update(sI);
			return h.Final();
		}

		public static BigInteger Getx(BigInteger Salt, byte[] LogonHash)
		{
			Sha1Hash h = new Sha1Hash();
			h.Update(Salt);
            h.Update(LogonHash);
			return new BigInteger(h.Final());
		}
		
		public BigInteger GetA(BigInteger a)
		{
			return g.modPow(a, N);
		}

		public BigInteger GetB(BigInteger b, BigInteger v)
		{
			BigInteger B = ((v * k) + g.modPow(b, N)) % N;
			return B;
		}

		public BigInteger Getv(BigInteger x)
		{
			return g.modPow(x, N);
		}

		// HandleLogonProof stuff
		public static BigInteger Getu(BigInteger A, BigInteger B)
		{
			Sha1Hash h = new Sha1Hash();
			h.Update(A);
			return new BigInteger(h.Final(B));
		}

		// Server version
		// S = (Av^u) ^ b
		public BigInteger ServerGetS(BigInteger A, BigInteger b, BigInteger v, BigInteger u)
		{
			return (A * (v.modPow(u, N))).modPow(b, N);
		}

		// Client version
		// S = (B - kg^x) ^ (a + ux)
		public BigInteger ClientGetS(BigInteger a, BigInteger B, BigInteger x, BigInteger u)
		{
			BigInteger S;
			S = (B - (k * g.modPow(x, N))).modPow(a + (u * x), N);
			return S;
		}

		public BigInteger GetM(string Username, BigInteger s, BigInteger A, BigInteger B, BigInteger K)
		{
			Sha1Hash sha;
			
			sha = new Sha1Hash();
			byte[] hash = sha.Final(N);

			sha = new Sha1Hash();
			byte[] ghash = sha.Final(g);

			for (int i = 0; i < 20; ++i)
				hash[i] ^= ghash[i];

			// TODO: do t2 and t4 need to be BigInts?  Could we just use the byte[]?
			BigInteger t3 = new BigInteger(hash);

			sha = new Sha1Hash();
			sha.Update(Username);
			BigInteger t4 = new BigInteger(sha.Final());

			sha = new Sha1Hash();
			sha.Update(t3);
			sha.Update(t4);
			sha.Update(s);
			sha.Update(A);
			sha.Update(B);
			return new BigInteger(sha.Final(K));
		}

		public static byte[] GetM2(BigInteger A, BigInteger M, BigInteger K)
		{
			Sha1Hash h = new Sha1Hash();
			h.Update(A);
			h.Update(M);
			return h.Final(K);
		}

		// Converts S to K
		// K is the Key which is passed to the Crypt class
		public static byte[] ShaInterleave(BigInteger S)
		{
			byte[] t = S;
			int HalfSize = t.Length / 2; // Untested.  I previously hard coded this as 16
			byte[] t1 = new byte[HalfSize];
			
			for (int i = 0; i < HalfSize; i++)
				t1[i] = t[i*2];
				
			Sha1Hash sha = new Sha1Hash();
			byte[] t1hash = sha.Final(t1);

			byte[] vK = new byte[40];
			for (int i = 0; i < 20; i++)
				vK[i*2] = t1hash[i];
				
			for (int i = 0; i < HalfSize; i++)
				t1[i] = t[i*2+1];

			sha = new Sha1Hash();
			t1hash = sha.Final(t1);

			for (int i = 0; i < 20; i++)
				vK[i*2+1] = t1hash[i];

			return vK;
		}
	}
}
