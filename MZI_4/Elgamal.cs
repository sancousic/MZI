using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MZI_4
{
    class Elgamal
    {
        public Elgamal(long start, long stop)
        {
            GenerateKeys(start, stop);
        }
        public long X { get; set; }
        private long Y { get; set; }
        private long P { get; set; }
        public long G { get; set; }
        private long PowMod(long a, long b, long m)
        {
            long res = 1;
            while(b != 0)
            {
                res = (res * a) % m;
                b--;
            }
            return res;
        }
        public long GCD(long a, long b)
        {
            while (b != 0)
            {
                b = a % (a = b);
            }
            return Math.Abs(a);
        }

        internal void Test(byte v)
        {
            P = 11;
            G = 2;
            X = 8;
            Y = 3;
            var a = Encrypt(new byte[] { v }, 9);
            var t = Decrypt(a.Item1, a.Item2);
        }

        private long GetPrimitiveRoor(long p)
        {
            List<long> fact = new List<long>();
            long phi = p - 1, n = phi;
            for (long i = 2; i * i <= n; i++)
            { 
                if(n % i == 0)
                {
                    fact.Add(i);
                    while(n % i == 0)
                    {
                        n /= i;
                    }
                }
            }
            if(n > 1)
            {
                fact.Add(n);
            }
            for (long res = 2; res <= p; res++)
            {
                bool ok = true;
                for(int i = 0; i < fact.Count() && ok; i++)
                {
                    ok &= PowMod(res, phi / fact[i], p) != 1;
                }
                if (ok)
                    return res;
            }
            throw new Exception("Cannot find Primitive Root");
        }
        private List<long> GetPrimes(long start, long stop)
        {
            var is_prime = new bool[stop + 1];
            for (long i = 2; i <= stop; i++)
            {
                is_prime[i] = true;
            }

            for (long i = 2; i <= stop; i++)
            {
                if (is_prime[i])
                {
                    if (i * i <= stop)
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
        private long LongRand(long min, long max, Random rand)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long res = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(res % (max - min)) + min);
        }
        private void GenerateKeys(long start, long stop)
        {
            Random rand = new Random();
            var p_var = GetPrimes(start, stop);
            P = p_var[rand.Next(p_var.Count)];

            G = GetPrimitiveRoor(P);

            X = GenerateK();
            Y = PowMod(G, X, P);
        }
        private long GenerateK()
        {
            Random rand = new Random();
            long k = 0;
            while(k == 0)
            {
                var a = rand.Next((int)P - 1);
                if(GCD(a, P-1) == 1)
                {
                    k = a;
                }
            }
            return k;
        }
        public (long[], long[]) Encrypt(byte[] data, long k = 0)
        {
            if(k == 0)
                k = GenerateK();

            var A = new long[data.Length];
            var B = new long[data.Length];

            for(long i = 0; i < data.Length; i++)
            {
                A[i] = PowMod(G, k, P);
                B[i] = (PowMod(Y, k, P) * data[i] % P) % P;
            }
            return (A, B);
        }
        public byte[] Decrypt(long[] A, long[] B)
        {
            byte[] res = new byte[A.Length];

            for(int i = 0; i < res.Length; i++)
            {
                var r = PowMod(A[i], P - 1 - X, P);
                res[i] = (byte)((B[i] * r) % P);
            }

            return res;
        }
        public void printAll()
        {
            Console.WriteLine($"X = {X}");
            Console.WriteLine($"Y = {Y}");
            Console.WriteLine($"P = {P}");
            Console.WriteLine($"G = {G}");
        }
    }
}
