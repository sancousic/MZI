using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace MZI_1
{
    class Program
    {
        static string input = "input.txt";
        static string d1_enc = "d1_enc.txt";
        static string d2_enc = "d2_enc.txt";
        static string d3_enc = "d3_enc.txt";
        static string d1_dec = "d1_dec.txt";
        static string d2_dec = "d2_dec.txt";
        static string d3_dec = "d3_dec.txt";
        static string gost_enc = "gost_enc.txt";
        static string gost_dec = "gost_dec.txt";

        static byte[] DesKey1 = Helper.ExtendKey("123456");
        static byte[] DesKey2 = Helper.ExtendKey("12345678");
        static byte[] GostKey = Helper.ExtendGostKey("112345478998765");

        static void Main(string[] args)
        {
            var data = Helper.ExtendData(Read(input));
            var d1 = DES.Encrypt(data, DesKey1);
            var d2 = DES2.Encrypt(data, DesKey1, DesKey2);
            var d3 = DES3.Encrypt(data, DesKey1, DesKey2);
            var g = GOST_28147_89.Encrypt(data, GostKey);

            Write(d1, d1_enc);
            Write(d2, d2_enc);
            Write(d3, d3_enc);
            Write(g, gost_enc);

            var dd1 = Helper.RecoverData(DES.Decrypt(d1, DesKey1).ToList());
            var dd2 = Helper.RecoverData(DES2.Decrypt(d2, DesKey1, DesKey2).ToList());
            var dd3 = Helper.RecoverData(DES3.Decrypt(d3, DesKey1, DesKey2).ToList());
            var dg = Helper.RecoverData(GOST_28147_89.Decrypt(g, GostKey).ToList());

            Write(dd1, d1_dec);
            Write(dd2, d2_dec);
            Write(dd3, d3_dec);
            Write(dg, gost_dec);
        }
        static byte[] Read(string path)
        {
            FileInfo f = new FileInfo(path);
            byte[] data = new byte[f.Length];
            using (FileStream fs = new FileStream(input, FileMode.Open))
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
