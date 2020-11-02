using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZI_3
{
    class Program
    {
        static string _input = "input.txt";
        static string _enc = "enc.txt";
        static string _dec = "dec.txt";
        static void Main(string[] args)
        {
            RSA rSA = new RSA();
            var input = Read(_input);
            var enc = rSA.Encrypt(input);
            Write(enc, _enc);
            rSA.print(rSA.ByteToLong(enc), "encrypt: ");
            var dec = rSA.Decrypt(enc);
            Write(dec, _dec);
            rSA.print(dec, "decrypt: ");
            rSA.printAll();
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
    }
}
