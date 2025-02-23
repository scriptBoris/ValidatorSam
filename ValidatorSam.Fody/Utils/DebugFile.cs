using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ValidatorSam.Fody.Extensions;

namespace ValidatorSam.Fody.Utils
{
    public static class DebugFile
    {
        public static bool UseDebugFile = false;

        private const string fileName = "post_fody.log";

        public static void ClearFile()
        {
            if (!UseDebugFile)
                return;

            string dir = Environment.CurrentDirectory;
            string path = Path.Combine(dir, fileName);
            if (File.Exists(path))
                System.IO.File.WriteAllText(path, string.Empty);
        }

        public static void WriteLine(string line, int consoleSpacing = 0)
        {
            if (!UseDebugFile)
                return;

            string spacing = "  ".Multiple(consoleSpacing);

            string text = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}: {spacing}{line}";
            var sw = TryCreateStream();
            sw.WriteLine(text);
            sw.Dispose();
        }

        private static StreamWriter TryCreateStream()
        {
            string dir = Environment.CurrentDirectory;
            string path = Path.Combine(dir, fileName);
            StreamWriter sw;

            //if (File.Exists(path))
            //{
                sw = File.AppendText(path);
            //}
            //else
            //{
            //    sw = File.CreateText(path);
            //}

            return sw;
        }
    }
}
