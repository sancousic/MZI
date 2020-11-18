using System;
using System.IO;
using System.Numerics;

namespace MZI_6
{
    class Program
    {
        static void Main(string[] args)
        {
            GOST_3410 a = new GOST_3410();
            var h = Read("test.txt");
            var s = a.Sign(h);
            Console.WriteLine(a.Verify(h, s.Item1, s.Item2));
            Console.ReadLine();
        }
        static byte[] Read(string path)
        {
            FileInfo f = new FileInfo(path);
            byte[] data = new byte[f.Length];
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                fs.Read(data, 0, (int)f.Length);
            }
            return data;
        }
        static void Write(byte[] data, string path)
        {
            using FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            fs.Write(data, 0, data.Length);
        }
        public static void print<T>(T[] ts, string txt = "")
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
