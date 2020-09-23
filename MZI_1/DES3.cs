using System;
using System.Collections.Generic;
using System.Text;

namespace MZI_1
{
    class DES3
    {
        public static byte[] Encrypt(byte[] data, byte[] key1, byte[] key2) =>
            DES.Encrypt(DES.Decrypt(DES.Encrypt(data, key1), key2), key1);
        public static byte[] Decrypt(byte[] data, byte[] key1, byte[] key2) =>
            DES.Decrypt(DES.Encrypt(DES.Decrypt(data, key1), key2), key1);
    }
}
