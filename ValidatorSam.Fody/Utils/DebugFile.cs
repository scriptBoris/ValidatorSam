using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValidatorSam.Fody.Utils
{
    public static class DebugFile
    {
        private const string fileName = "post_fody.log";

        public static void WriteLine(string line)
        {
            string text = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}: {line}";
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
