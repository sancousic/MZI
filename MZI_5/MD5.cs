using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZI_5
{
    class MD5
    {
        public MD5()
        {
            T = GetT();
        }
        private uint[] T { get; set; }
        public uint[] GetT()
        {
            var res = new uint[64];
            for (int i = 1; i <= 64; i++)
            {
                res[i - 1] = (uint)Math.Truncate(((ulong)1 << 32) * Math.Abs(Math.Sin(i)));
            }
            return res;
        }
        public byte[] ExtendData(byte[] data)
        {
            var dif = (512 - ((data.Length * 8 - 448) % 512)) % 512;
            if (dif == 0) dif = 512;
            var res = new byte[data.Length + dif / 8];

            data.CopyTo(res, 0);
            byte one = 1 << 7;
            res[data.Length] = one;

            return res;
        }
        public byte[] ExtendLength(byte[] data, long length)
        {
            var res = new byte[data.Length + sizeof(ulong)];
            data.CopyTo(res, 0);
            BitConverter.GetBytes((uint)length).CopyTo(res, data.Length + 1);
            return res;
        }
        public byte[] InitABCD()
        {
            byte[] ABCD = new byte[16];
            uint A = 0x01234567;
            uint B = 0x89ABCDEF;
            uint C = 0xFEDCBA98;
            uint D = 0x76543210;
            BitConverter.GetBytes(A).CopyTo(ABCD, 0);
            BitConverter.GetBytes(B).CopyTo(ABCD, sizeof(uint));
            BitConverter.GetBytes(C).CopyTo(ABCD, sizeof(uint) * 2);
            BitConverter.GetBytes(D).CopyTo(ABCD, sizeof(uint) * 3);
            return ABCD;
        }
        public static byte[] XOR(byte[] a, byte[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] ^= b[i];
            }
            return a;
        }
        public byte[] GetS(byte[] arr, int a)
        {
            return arr.Skip(a * sizeof(uint)).Take(sizeof(uint)).ToArray();
        }
        public byte[] Glue(byte[] A, byte[] B, byte[] C, byte[] D)
        {
            byte[] res = new byte[A.Length * 4];
            A.CopyTo(res, 0);
            B.CopyTo(res, sizeof(uint));
            C.CopyTo(res, sizeof(uint)*2);
            D.CopyTo(res, sizeof(uint)*3);
            return res;
        }
        public byte[] CLS(byte[] data, int s)
        {
            var val = BitConverter.ToUInt32(data, 0);
            return BitConverter.GetBytes(CLS(val, s));
        }
        public uint CLS(uint data, int s)
        {
            return data << s | data >> (32 - s);
        }
        public byte[] AddMod32(byte[] a, byte[] b)
        {
            var A = (ulong)BitConverter.ToUInt32(a, 0);
            var B = (ulong)BitConverter.ToUInt32(b, 0);

            var res = (uint)((A + B) % (((ulong)(1) << 32)));

            return BitConverter.GetBytes(res);
        }
        public byte[] Left32WordShift(byte[] data)
        {
            uint[] res = new uint[4];

            for(int i = 0; i < res.Length; i++)
            {
                res[i] = BitConverter.ToUInt32(data, i * sizeof(uint));
            }
            res = Left32WordShift(res);

            for(int i = 0; i < 4; i++)
            {
                BitConverter.GetBytes(res[i]).CopyTo(data, i * sizeof(uint));
            }

            return data;
        }
        public uint[] Left32WordShift(uint[] data)
        {
            var tmp = data[0];
            for(int i = 0; i < data.Length - 1; i++)
            {
                data[i] = data[i + 1];
            }
            data[data.Length - 1] = tmp;
            return data;
        }
        public byte[] BigCycle(byte[] M)
        {
            var ABCD = InitABCD();

            for (int i = 0; i < M.Length / 64; i++)
            {
                var Yq = M.Skip(i * 64).Take(64).ToArray();
                var MDq = new byte[ABCD.Length];
                ABCD.CopyTo(MDq, 0);

                Cycle(ref ABCD, Yq, T.Take(16).ToArray(), fF);
                Cycle(ref ABCD, Yq, T.Skip(16).Take(16).ToArray(), fG);
                Cycle(ref ABCD, Yq, T.Skip(16 * 2).Take(16).ToArray(), fH);
                Cycle(ref ABCD, Yq, T.Skip(16 * 3).Take(16).ToArray(), fI);

                var A = XOR(GetS(ABCD, 0), MDq);
                var B = XOR(GetS(ABCD, 0), MDq);
                var C = XOR(GetS(ABCD, 0), MDq);
                var D = XOR(GetS(ABCD, 0), MDq);

                ABCD = Glue(A, B, C, D);
            }
            return ABCD;
        }
        public (uint, uint, uint) GetBCD(byte[] ABCD)
        {
            var B = BitConverter.ToUInt32(GetS(ABCD, 1), 0);
            var C = BitConverter.ToUInt32(GetS(ABCD, 2), 0);
            var D = BitConverter.ToUInt32(GetS(ABCD, 3), 0);

            return (B, C, D);
        }
        public byte[] fF(byte[] ABCD)
        {
            (uint B, uint C, uint D) = GetBCD(ABCD);
            return BitConverter.GetBytes((B&C)|(~B&D));
        }
        public byte[] fG(byte[] ABCD)
        {
            (uint B, uint C, uint D) = GetBCD(ABCD);
            return BitConverter.GetBytes((B & D) | (C & ~D));
        }
        public byte[] fH(byte[] ABCD)
        {
            (uint B, uint C, uint D) = GetBCD(ABCD);
            return BitConverter.GetBytes(B^C^D);
        }
        public byte[] fI(byte[] ABCD)
        {
            (uint B, uint C, uint D) = GetBCD(ABCD);
            return BitConverter.GetBytes(C^(B&~D));
        }
        public void Cycle(ref byte[] ABCD, byte[] Yq, uint[] T, Func<byte[], byte[]> f)
        {
            var s = 5; // ??????????
            for (int i = 0; i < 16; i++)
            {
                var a = f(ABCD);
                a = AddMod32(a, GetS(ABCD, 0));
                a = AddMod32(a, GetS(Yq, i));
                a = AddMod32(a, BitConverter.GetBytes(T[i]));
                a = CLS(a, s);
                a = AddMod32(a, GetS(ABCD, 2));
                a.CopyTo(ABCD, 0);
                ABCD = Left32WordShift(ABCD);
            }
        }
        public byte[] GetHashe(byte[] data)
        {
            var len = data.Length;
            data = ExtendData(data);
            data = ExtendLength(data, len);
            var res = BigCycle(data);
            print(res);
            return res;
        }
        public void print<T>(T[] ts, string txt = "")
        {
            Console.Write(txt);
            for (long i = 0; i < ts.Length; i++)
            {
                Console.Write(ts[i].ToString() + " ");
            }
            Console.WriteLine();
        }
    }
}
