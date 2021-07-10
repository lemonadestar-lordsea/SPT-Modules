using System;

namespace Aki.Common.Utils
{
    public static class Log
    {
        private static string _filepath;

        static Log()
        {
            _filepath = VFS.Combine(VFS.Cwd, "./user/logs/modules.log");

            if (VFS.Exists(_filepath))
            {
                VFS.DeleteFile(_filepath);
            }
        }

        private static void Write(string type, string text)
        {
            VFS.WriteFile(_filepath, $"[{DateTime.Now}] {type} | {text}{Environment.NewLine}", true);
        }

        public static void Data(string text)
        {
            VFS.WriteFile(_filepath, $"[{DateTime.Now}] {text}{Environment.NewLine}", true);
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
