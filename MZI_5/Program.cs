using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZI_5
{
    class Program
    {
        static string _input = "input.txt";
        static void Main(string[] args)
        {
            MD5 md = new MD5();
            var input = Read(_input);
            md.GetHashe(input);
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
    }
}
