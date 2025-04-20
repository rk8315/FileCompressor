using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressorLibrary
{
    public static class FileManager
    {
        public static byte[] ReadBytes(string filePath) => File.ReadAllBytes(filePath);

        public static void WriteBytes(string filePath, byte[] content) => File.WriteAllBytes(filePath, content);

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
