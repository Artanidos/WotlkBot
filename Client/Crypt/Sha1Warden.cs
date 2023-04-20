using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WotlkClient.Shared;

namespace WotlkClient.Crypt
{
    // This class generates warden crappy RC4 hash :)
    // I don't think that i will ever need that but I was a bit bored this evening and decided to take a closer look into BClient sources. 
    // This is way how burlex calculates RC4 hash for warden packets. 
    // If only I will make that freaking CRC working I will try to do something more with this shit :)
    public class Sha1Warden
    {
        Sha1Hash sh;
        UInt32 taken;
        byte[] o0 = new byte[20];
        byte[] o1 = new byte[20];
        byte[] o2 = new byte[20];

        public Sha1Warden(byte[] buff, UInt32 size)
        {
            sh = new Sha1Hash();
            taken = size / 2;
            
            byte[] buff2 = new byte[taken];
            memcopy(buff2, buff);

            sh.Initialize();
            sh.Update(buff2);
            memcopy(o1, sh.Final());
            
            byte[] buff3 = new byte[20];
            memcopy(buff3, buff, 20);

            sh.Initialize();
            sh.Update(buff3);
            memcopy(o2, sh.Final());

            memset(o0, 0x00);

            FillUp();
        }

        public void FillUp()
        {
            sh.Initialize();
            sh.Update(o1);
            sh.Update(o0);
            sh.Update(o2);
            
            memcopy(o0, sh.Final());
            taken = 0;
        }

        public byte[] Generate(UInt32 size)
        {
            byte[] result = new byte[size];
            for (UInt32 i = 0; i < size; i++)
            {
                if (taken == 20)
                {
                    FillUp();
                }

                result[i] = o0[taken];
                taken++;
            }
            return result;
        }

        private static void memset(byte[] buffer, byte value)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = value;
            }
        }

        private static void memcopy(byte[] output, byte[] input, int x)
        {
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = input[i+x];
            }
        }

        private static void memcopy(byte[] buffer, byte[] buffer2)
        {
            memcopy(buffer, buffer2, 0);
        }

    }
}
