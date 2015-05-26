using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandGesture
{
    static class Debug
    {
        static public void Log(string str)
        {
#if DEBUG
            Console.WriteLine(str);
#endif
        }
    }
}
