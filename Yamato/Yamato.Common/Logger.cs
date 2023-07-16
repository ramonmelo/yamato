using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Yamato.Common
{
    internal static class Logger
    {
        [Conditional("DEBUG")]
        public static void Debug(string msg)
        {
            Console.WriteLine(msg);
        }


        [Conditional("DEBUG")]
        public static void Error(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
