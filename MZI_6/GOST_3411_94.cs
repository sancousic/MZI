using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace MZI_6
{
    class GOST_3411_94
    {
        private byte[] ExtendData(byte[] data, int bitlen)
        {
            var dif = bitlen - (data.Length * 8) % bitlen;
            var res = new byte[data.Length + dif / 8];

            data.CopyTo(res, 0);
            return res;
        }
        
        private byte[] GetC3() => new byte[32] { 255, 0, 255, 255, 0, 0, 0, 255, 255,
                0, 0, 255, 0, 255, 255, 0, 0, 255, 0, 255, 0, 255, 0, 255, 255,
                0, 255, 0, 255, 0, 255, 0 };
        
        public byte[] GetHashe(byte[] data, byte[] H)
        {
            data = ExtendData(data, 256);
            for(int i = 0; i < data.Length / 32; i++)
            {
                var M = data.Skip(32 * i).Take(32).ToArray();
                var keys = GenerateKeys(H, M);
                H = Ksi(M, H, keys);
            }
            return H;
        }
        private byte[] Ksi(byte[] M, byte[] H, byte[][] K)
        {
            var S = E(H, K);
            var res = Kappa(XOR(H, Kappa(XOR(M, Kappa(S, 12)), 1)), 61);
            return res;
        }
        private byte[] Kappa(byte[] data, int count)
        {
            var a = GetUintArrFromByteArr(data);
            var res = new uint[a.Length];
            res[0] = a[0];
            for(int i = 1; i < a.Length; i++)
            {
                res[0] ^= a[i];
            }

            for(int i = 1; i < a.Length; i++)
            {
                res[i] = a[a.Length - i];
            }

            return GetByteArrFromUIntArr(res);
        }
        private uint[] GetUintArrFromByteArr(byte[] data)
        {
            var res = new uint[data.Length / 4];
            for(int i = 0; i < data.Length / 4; i++)
            {
                res[i] = BitConverter.ToUInt32(data, i * 4);
            }
            return res;
        }
        private byte[] GetByteArrFromUIntArr(uint[] data)
        {
            var res = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
            {
                BitConverter.GetBytes(data[i]).CopyTo(res, i * 4);
            }
            return res;
        }
        public byte[] E(byte[] H, byte[][] K)
        {
            byte[][] S = new byte[4][];
            for(int i = 3, k = 0; i >= 0; i--, k++)
            {
                var data = H.Skip(i * 8).Take(8).ToArray();
                S[i] = GOST_28147_89.Encrypt(data, K[k]);
            }

            byte[] res = new byte[H.Length];
            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < S[i].Length; j++)
                {
                    res[i * 8 + j] = S[i][j];
                }
            }
            return res;
        }
        public byte[][] GenerateKeys(byte[] H, byte[] M)
        {
            byte[][] C = new byte[4][];
            C[1] = new byte[32];
            C[2] = GetC3();
            C[3] = new byte[32];
            byte[][] K = new byte[4][];
            byte[] U = new byte[H.Length], V = new byte[M.Length];

            H.CopyTo(U, 0);
            M.CopyTo(V, 0);
            var W = XOR(U, V);
            K[0] = P(W);
            for(int i = 1; i < 4; i++)
            {
                U = XOR(A(U), C[i]);
                V = A(A(V));
                W = XOR(U, V);
                K[i] = P(W);
            }
            return K;
        }
        private byte[] P(byte[] data)
        {
            var res = new byte[data.Length];
            for(int i = 0; i < 4; i++)
            {
                for(int k = 1; k < 9; k++)
                {
                    res[8 * i + k - 1] = data[i + 4 * (k - 1)];
                }
            }
            return res;
        }
        private byte[] A(byte[] data)
        {
            var x = new List<byte>[4];
            var res = new List<byte>();
            for (int i = 0; i < 4; i++)
            {
                x[i] = data.Skip(i * 8).Take(8).ToList();
            }
            res.AddRange(XOR(x[0], x[1]));
            res.AddRange(x[3]);
            res.AddRange(x[2]);
            res.AddRange(x[1]);
            return res.ToArray();
        }
        public List<byte> XOR(List<byte> a, List<byte> b)
        {
            for (int i = 0; i < a.Count; i++)
            {
                a[i] ^= b[i];
            }
            return a;
        }
        public byte[] XOR(byte[] a, byte[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] ^= b[i];
            }
            return a;
        }
    }
}
