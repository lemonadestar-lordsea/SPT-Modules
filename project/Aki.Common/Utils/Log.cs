using System;
using System.IO;

namespace Aki.Common.Utils
{
    public static class Log
    {
        private const string fn = "C:\\temp\\akitestlog.log";

        private static void Write(string type, string text)
        {
            File.AppendAllText(fn, $"[{DateTime.Now}] {type} | {text}{Environment.NewLine}");
        }

        public static void Data(string text)
        {
            File.AppendAllText(fn, $"[{DateTime.Now}] {text}{Environment.NewLine}");
        }

        public static void Info(string text)
        {
            Write("INFO", text);
        }

        public static void Warning(string text)
        {
            Write("WARNING", text);
        }

        public static void Error(string text)
        {
            Write("ERROR", text);
        }
    }
}
