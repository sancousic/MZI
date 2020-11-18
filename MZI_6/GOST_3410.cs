using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MZI_6
{
    class GOST_3410
    {
        public GOST_3410()
        {
            GenerateKeys();
        }
        public BigInteger _2_161 { get; set; } = BigInteger.Pow(new BigInteger(2), 161);
        private BigInteger P { get; set; }
        private BigInteger Q { get; set; }
        private BigInteger X { get; set; }
        public BigInteger Y { get; set; }
        public BigInteger A { get; set; }
        // t = length of p, length of q = [t/2]
        public void GenerateKeys()
        {
            P = BigInteger.Parse("00000000" + "EE8172AE" + "8996608F" + "B69359B8" + "9EB82A69" +
                "854510E2" + "977A4D63" + "BC97322C" + "E5DC3386" +
                "EA0A12B3" + "43E9190F" + "23177539" + "84583978" +
                "6BB0C345" + "D165976E" + "F2195EC9" + "B1C379E3",
                System.Globalization.NumberStyles.HexNumber);
            Q = BigInteger.Parse("00000000" + "98915E7E" + "C8265EDF" + "CDA31E88" + "F24809DD" +
                "B064BDC7" + "285DD50D" + "7289F0AC" + "6F49DD2D",
                System.Globalization.NumberStyles.HexNumber);
            X = RandBigInt(Q);
            A = GetG();
            Y = BigInteger.ModPow(A, X, P);
        }
        public (byte[],byte[]) Sign(byte[] data)
        {
            var gost3411 = new GOST_3411_94();
            var h = gost3411.GetHashe(data, new byte[32]);
            var k = RandBigInt(Q);
            var r = BigInteger.ModPow(A, k, P) % Q;
            var s = (k * new BigInteger(h) + X * r) % Q;
            return (r.ToByteArray(), s.ToByteArray());
        }
        public bool Verify(byte[] data, byte[] r, byte[] s)
        {
            var gost3411 = new GOST_3411_94();
            var h = gost3411.GetHashe(data, new byte[32]);
            var w = new BigInteger(h) % Q;
            var u1 = (new BigInteger(s) / w) % Q;
            var u2 = ((Q - new BigInteger(r)) / w) % Q;
            return ((BigInteger.ModPow(A, u1, P) * BigInteger.ModPow(Y, u2, P) % P) % Q == new BigInteger(r));
        }
        private BigInteger GetG()
        {
            BigInteger a = BigInteger.One;
            while (true)
            {
                
                if (BigInteger.ModPow(a, Q, P) == 1)
                    return a;
                a += 2;
            }

        }
        private BigInteger RandBigInt(BigInteger stop)
        {
            byte[] bytes = stop.ToByteArray();
            Random random = new Random();
            BigInteger R;
            do
            {
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F;
                R = new BigInteger(bytes);
            } while (R >= stop);

            return R;
        }
        private BigInteger FloorBigInt(BigInteger val, BigInteger rem)
        {
            if(rem > 0)
            {
                return val + 1;
            }
            return val;
        }
        public ushort Generator(BigInteger x, ushort c)
        {
            BigInteger a;
            BigInteger.DivRem((19381 * x + c), BigInteger.Pow(2, 16), out a);
            return (ushort)(a);
        }
        public uint GeneratePrimeWithBitLength(int l)
        {
            var min = 1 << l;
            var list = GetPrimes(min);
            if (list.Count == 0)
                throw new Exception($"Cannot find prime with bitlen = {l}");
            return (uint)list[0];
        }
        private List<long> GetPrimes(long start, long stop = 0)
        {
            if(stop == 0)
            {
                stop = start << 1;
            }
            var is_prime = new bool[stop + 1];
            for (long i = 2; i <= stop; i++)
            {
                is_prime[i] = true;
            }

            for (long i = 2; i <= stop; i++)
            {
                if (is_prime[i])
                {
                    if (i * 1 * i <= stop)
                    {
                        for (long j = i * i; j <= stop; j += i)
                        {
                            is_prime[j] = false;
                        }
                    }
                }
            }
            List<long> primes = new List<long>();
            
            for (long i = start; i <= stop; i++)
            {
                if (is_prime[i])
                    primes.Add(i);
            }
            return primes;
        }
    }
}
