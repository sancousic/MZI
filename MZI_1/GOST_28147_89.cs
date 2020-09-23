using System;
using System.Collections;
using System.Drawing;
using System.Linq;


namespace MZI_1
{
    class GOST_28147_89
    {
        private static readonly byte Rounds = 32;
        private static readonly byte[,] SubstitutioTable =
        {
            { 4,10,9,2,13,8,0,14,6,11,1,12,7,15,5,3},
            { 14,11,4,12,6,13,15,10,2,3,8,1,0,7,5,9},
            { 5,8,1,13,10,3,4,2,14,15,12,7,6,0,9,11},
            { 7,13,10,1,0,8,9,15,14,4,6,12,11,2,5,3},
            { 6,12,7,1,5,15,13,8,4,10,9,14,0,3,11,2},
            { 4,11,10,0,7,2,1,13,3,6,8,5,9,12,15,14},
            { 13,11,4,1,3,15,5,9,0,10,14,7,6,8,2,12},
            { 1,15,13,0,5,7,10,4,9,2,3,14,6,11,8,12},
        };
        public static readonly byte[] KeyTable =
        {
            0,1,2,3,4,5,6, 7,
            0,1,2,3,4,5,6, 7,
            0,1,2,3,4,5,6, 7,
            7,6,5,4,3,2,1,0
        };
        public static uint[] GetKeys(byte[] key)
        {
            var res = new uint[8];
            for(int i = 0; i < 8; i++)
            {
                var val = key.Skip(i * 4).Take(4).ToArray();
                val = val.ToArray(); //TODO need it?
                res[i] = BitConverter.ToUInt32(val);
            }
            return res;
        }
        public static byte[] getS(uint S32)
        {
            byte[] res = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                res[i] = (byte)((S32 /  Math.Pow(2, 4 * (7 - i ))) % Math.Pow(2, 4));
            }
            return res;
        }
        public static uint Replace(byte[] S)
        {
            var t = new byte[8];
            var res = new byte[4];
            for(int i = 0; i < S.Length; i++)
            {
                t[i] = SubstitutioTable[7 - i, S[i]];
            }
            for(int i = 0; i < res.Length; i++)
            {
                res[i] += (byte)(t[i * 2] * Math.Pow(2, 4));
                res[i] += t[i * 2 + 1];
            }
            return BitConverter.ToUInt32(res.ToArray());
        }
        static uint shift(uint a, int n)
        {
            n %= (sizeof(uint) * 8);
            var t1 = a << n;
            var t2 = a >> (sizeof(uint) * 8 - n);
            return t1 | t2;
        }
        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            var keys = GetKeys(key);
            var res = new byte[data.Length];
            for(int i = 0; i < data.Length / 8; i++)
            {
                var L = BitConverter.ToUInt32(data.Skip(i * 8).Take(4).ToArray());
                var R = BitConverter.ToUInt32(data.Skip(i * 8 + 4).Take(4).ToArray());
                for (int round = 0; round < Rounds; round++)
                {     
                    var temp = R;
                    var S32 = R + keys[KeyTable[round]];
                    var S = getS(S32);
                    var P = Replace(S);
                    P = shift(P, 11);
                    R = P ^ L;
                    L = temp;
                }
                var R1 = BitConverter.GetBytes(R);
                var L1 = BitConverter.GetBytes(L); 
                for(int k = i * 8; k < i*8 + 4; k++)
                {
                    res[k] = R1[k - i * 8];
                }
                for(int k = i*8+4; k < i * 8 + 8; k++)
                {
                    res[k] = L1[k - i * 8 - 4];
                }
            }
            return res;
        }
        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            var keys = GetKeys(key);
            var res = new byte[data.Length];
            for (int i = 0; i < data.Length / 8; i++)
            {
                var L = BitConverter.ToUInt32(data.Skip(i * 8).Take(4).ToArray());
                var R = BitConverter.ToUInt32(data.Skip(i * 8 + 4).Take(4).ToArray());
                for (int round = 0; round < Rounds; round++)
                {
                    var temp = R;
                    var S32 = R + keys[KeyTable[KeyTable.Length - round - 1]];
                    var S = getS(S32);
                    var P = Replace(S);
                    P = shift(P, 11);
                    R = P ^ L;
                    L = temp;
                }
                var R1 = BitConverter.GetBytes(R);
                var L1 = BitConverter.GetBytes(L);
                for (int k = i * 8; k < i * 8 + 4; k++)
                {
                    res[k] = R1[k - i * 8];
                }
                for (int k = i * 8 + 4; k < i * 8 + 8; k++)
                {
                    res[k] = L1[k - i * 8 - 4];
                }
            }
            return res;
        }
    }
}
