using System;
using System.Collections.Generic;
using System.Text;

namespace MZI_1
{
    class DES2
    {
        public static byte[] Encrypt(byte[] data, byte[] key1, byte[] key2) =>
            DES.Encrypt(DES.Encrypt(data, key1), key2);
        public static byte[] Decrypt(byte[] data, byte[] key1, byte[] key2) =>
            DES.Decrypt(DES.Decrypt(data, key2), key1);
    }
}
