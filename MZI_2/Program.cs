using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZI_2
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new uint[] {5,6,7,8, 9, 11, 23, 33};
            var k = new uint[] { 2, 3, 4, 5, 6, 7, 8, 9 };

            // 1
            var d = STB.SimpleReplace(a, k, STB.Encrypt);
            var e = STB.SimpleReplace(d, k, STB.Decrypt);
            Console.WriteLine(Check(a, e));

            // 2

            var s = new uint[] { 1, 2, uint.MaxValue, uint.MaxValue };
            var d1 = STB.ClutchEncrypt(a, k, s);
            var e1 = STB.ClutchDecrypt(d1, k, s);
            Console.WriteLine(Check(e1, a));

            var a2 = new byte[] {1, 2, 3, 4,
                                 5, 6, 7, 8,
                                 9, 10, 11, 12,
                                13, 14, 15};

            // 3

            var d2 = STB.FeedbackGamma(a2, k, s);
            var e2 = STB.FeedbackGamma(d2, k, s);
            Console.WriteLine(Check(a2, e2));

            // 4

            var d3 = STB.Counter(a2, k, s);
            var e3 = STB.Counter(d3, k, s);
            Console.WriteLine(Check(e3, a2));

            Console.ReadLine();
        }
        static private bool Check<T>(T[] a, T[] b)
        {
            for(int i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i]))
                    return false;
            }
            return true;
        }
    }
}
