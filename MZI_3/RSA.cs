using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MZI_3
{
    class RSA
    {
        public RSA()
        {
            GenerateKeys(9);
        }
        public long E { get; set; }
        private long D{ get; set; }
        private long PQ { get; set; }
        public long GCD(long a, long b)
        {
            while (b != 0)
            {
                b = a % (a = b);
            }
            return Math.Abs(a);
        }
        private long Mul(long a, long b, long m)
        {
            if (b == 1)
                return a;

            if (b % 2 == 0)
            {
                long t = Mul(a, b / 2, m);
                return (2 * t) % m;
            }

            return (Mul(a, b - 1, m) + a) % m;
        }
        private long Pows(long a, long b, long m)
        {
            if (b == 0)
                return 1;
            if (b % 2 == 0)
            {
                long t = Pows(a, b / 2, m);
                return Mul(t, t, m) % m;
            }
            return (Mul(Pows(a, b - 1, m), a, m)) % m;
        }
        private bool IsPrime(long n)
        {
            for(long i = 2; i < Math.Sqrt(n); i++)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }
        private bool IsPrimeFerma(long n)
        {
            if (n == 2)
                return true;
            var rand = new Random();
            for(int i = 0; i < 100; i++)
            {
                long a = (rand.Next() % (n - 2)) + 2;
                if (GCD(a, n) != 1)
                    return false;
            }
            return true;
        }
        private List<long> GetPrimes(long start, long stop)
        {
            var is_prime = new bool[stop + 1];
            for(long i = 2; i <= stop; i++)
            {
                is_prime[i] = true;
            }

            for (long i = 2; i <= stop; i++)
            {
                if (is_prime[i])
                {
                    if (i * 1 * i <= stop)
                    {
                        for (long j = i * i; j <= stop; j+=i)
                        {
                            is_prime[j] = false;
                        }
                    }
                }
            }
            List<long> primes = new List<long>();
            for(long i = start; i <= stop; i++)
            {
                if (is_prime[i])
                    primes.Add(i);
            }
            return primes;
        }

        public void GenerateKeys(int length) 
        {
            if (length < 4)
                throw new ArgumentException("Cannot generate key pair with length less than 4");

            var min = 1 << (length - 1);
            var max = (1 << length) - 1;

            var start = 1 << ((int)(length / 2) - 1);
            var stop = 1 << ((int)(length / 2) + 1);

            var primes = GetPrimes(start, stop);
            var rand = new Random();
            while(primes.Count > 0)
            {
                var p = primes[rand.Next(primes.Count)];
                primes.Remove(p);
                var q_variants = new List<long>();
                foreach(var q in primes)
                {
                    var pq = p * q;
                    if (pq < min || pq > max)
                        continue;
                    q_variants.Add(q);
                }
                while(q_variants.Count > 0)
                {
                    var q = q_variants[rand.Next(q_variants.Count)];
                   
                    q_variants.Remove(q);

                    var F = (p - 1) * (q - 1);

                    var e_candidate = GetPrimes(1, F);
                    
                    while(e_candidate.Count > 0)
                    {
                        var e = e_candidate[rand.Next(e_candidate.Count)];
                        e_candidate.Remove(e);
                        if(GCD(F, e) == 1)
                        {
                            for(long d = 3; d < F; d+=2)
                            {
                                if(Mul(d, e, F) == 1)
                                {
                                    PQ = p * q;
                                    E = e;
                                    D = d;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            throw new Exception("Connot find pair with curent length");
        }
        public long[] ByteToLong(byte[] vs)
        {
            var res = new long[vs.Length / 8];
            for(int i = 0; i < res.Length; i++)
            {
                res[i] = BitConverter.ToInt64(vs, i * 8);
            }
            return res;
        }
        public byte[] LongToByte(long[] vs)
        {
            var res = new byte[vs.Length * 8];
            for(int i = 0; i < vs.Length; i++)
            {
                BitConverter.GetBytes(vs[i]).CopyTo(res, i * 8);
            }
            return res;
        }
        public byte[] Encrypt(byte[] data)
        {
            var res = new long[data.Length];

            for(long i = 0; i < data.Length; i++)
            {
                res[i] = Pows(data[i], E, PQ);
            }
            return LongToByte(res);
        }
        public byte[] Decrypt(byte[] data)
        {
            var bd = ByteToLong(data);
            var res = new byte[bd.Length];

            for (long i = 0; i < bd.Length; i++)
            {
                res[i] = (byte)Pows(bd[i], D, PQ);
            }

            return res;
        }
        public void print<T>(T[] ts, string txt = "")
        {
            Console.Write(txt);
            for(long i = 0; i < ts.Length; i++)
            {
                Console.Write(ts[i].ToString() + " ");
            }
            Console.WriteLine();
        }
        public void printAll()
        {
            Console.WriteLine($"PQ = {PQ}");
            Console.WriteLine($"E = {E}");
            Console.WriteLine($"D = {D}");
        }
    }
}
