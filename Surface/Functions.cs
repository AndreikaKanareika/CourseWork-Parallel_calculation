using System;
using System.Collections.Generic;
using System.Text;

namespace SurfaceLibrary
{
    static class Functions
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}
