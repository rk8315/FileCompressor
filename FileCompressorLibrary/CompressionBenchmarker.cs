using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressorLibrary
{
    public static class CompressionBenchmarker
    {
        public static void MeasureCompression(string inputFile, Func<string, string> compressionFunc)
        {
            string input = FileManager.ReadFile(inputFile); 

            Stopwatch stopwatch = Stopwatch.StartNew();
            string compressed = compressionFunc(input); 
            stopwatch.Stop();

            long originalSize = new FileInfo(inputFile).Length;
            long compressedSize = compressed.Length;

            Console.WriteLine($"Original Size: {originalSize} bytes");
            Console.WriteLine($"Compressed Size: {compressedSize} characters");
            Console.WriteLine($"Compression Ratio: {(1 - (double)compressedSize / originalSize) * 100:F2}%");
            Console.WriteLine($"Compression Time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
