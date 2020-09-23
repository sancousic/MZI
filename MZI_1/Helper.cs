using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZI_1
{
    class Helper
    {
        public static byte[] ExtendData(byte[] data)
        {
            List<byte> l = new List<byte>(data);
            int diff = 16 - (l.Count % 16);
            for (int i = 0; i < diff; i++)
            {
                l.Add(0);
            }
            return l.ToArray();
        }
        public static byte[] RecoverData(List<byte> data)
        {
            int i = data.Count - 1;
            while (data[i] == 0)
            {
                data.RemoveRange(i, data.Count - i);
                i--;
            }
            return data.ToArray();
        }

        public static byte[] ExtendGostKey(string data)
        {
            StringBuilder stringBuilder = new StringBuilder(data);
            if (data.Length > 32)
            {
                return StrToByteArray(stringBuilder.Remove(7, data.Length - 7).ToString());
            }
            int diff = 32 - data.Length;
            for (int i = 0; i < diff; i++)
            {
                stringBuilder.Append(data[i % data.Length]);
            }
            return StrToByteArray(stringBuilder.ToString());
        }

        public static byte[] ExtendKey(string data)
        {
            StringBuilder stringBuilder = new StringBuilder(data);
            if (data.Length > 7)
            {
                return StrToByteArray(stringBuilder.Remove(7, data.Length - 7).ToString());
            }
            int diff = 7 - data.Length;
            for (int i = 0; i < diff; i++)
            {
                stringBuilder.Append(data[i % data.Length]);
            }
            return StrToByteArray(stringBuilder.ToString());
        }
        public static string ToString(byte[] source)
        {
            return Encoding.UTF8.GetString(source);
        }
        public static byte[] StrToByteArray(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
