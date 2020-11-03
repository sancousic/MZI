using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZI_4
{
    class Program
    {
        static string _input = "input.txt";
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
        public static void print<T>(T[] ts, string txt = "")
        {
            Console.Write(txt);
            for (long i = 0; i < ts.Length; i++)
            {
                Console.Write(ts[i].ToString() + " ");
            }
            Console.WriteLine();
        }
        static void Main(string[] args)
        {
            Elgamal elgamal = new Elgamal(1000, 10000);
            //elgamal.Test(5);
            var input = Read(_input);
            elgamal.printAll();
            print(input, "input: ");
            var enc = elgamal.Encrypt(input);
            print(enc.Item1, "A: ");
            print(enc.Item2, "B: ");

            var dec = elgamal.Decrypt(enc.Item1, enc.Item2);
            print(dec, "decrypt: ");
            Console.ReadLine();
        }
    }
}
