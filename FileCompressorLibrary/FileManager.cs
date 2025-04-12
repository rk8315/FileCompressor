using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressorLibrary
{
    public static class FileManager
    {
        public static string ReadFile(string filePath)
        {
            return File.Exists(filePath) ? File.ReadAllText(filePath) : "";
        }

        public static void WriteFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }
    }
}
