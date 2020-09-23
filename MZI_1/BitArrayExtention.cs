using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZI_1
{
    public static class BitArrayExtention
    {
        public static BitArray LeftCycleShift(this BitArray data, int count)
        {
            BitArray temp = new BitArray(count);

            for (int i = 0; i < count; i++)
            {
                temp[i] = data[data.Length - count + i];
            }

            var res = data.LeftShift(count);
            
            for (int i = 0; i < count; i++)
            {
                res[i] = temp[i];
            }
            return res;
        }
        public static BitArray RightCycleShift(this BitArray data, int count)
        {
            BitArray temp = new BitArray(count);

            for (int i = 0; i < count; i++)
            {
                temp[i] = data[i];
            }

            BitArray res = ((BitArray)data.Clone()).RightShift(count);
            for (int i = 0; i < count; i++)
            {
                res[data.Length - count + i] = temp[i];
            }
            return res;
        }
    }
}
