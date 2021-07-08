using UnityEngine;

namespace Aki.Common.Utils
{
    public class Log
    {
        private static void Write(string type, string text)
        {
            Debug.LogError(string.Format("{0} | {1}", type, text));
        }

        public static void Data(string text)
        {
            Debug.LogError(string.Format("{0}", text));
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
