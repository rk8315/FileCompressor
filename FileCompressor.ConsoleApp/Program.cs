using FileCompressorLibrary;
using FileCompressorLibrary.CompressionAlgorithms;
using FileCompressorLibrary.Models;

namespace FileCompressor.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("+++ File Compression Test +++");
            List<string> options = ["1", "2"];

            while (true)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1. Run-Length Compression");
                Console.WriteLine("2. Huffman Compression");
                Console.WriteLine("0. Exit");
                Console.Write("Option Selected: ");
                var option = Console.ReadLine();

                if (option == "0") break;

                Console.Write("Enter full path of the input file: ");
                string inputPath = Console.ReadLine();

                if (!File.Exists(inputPath))
                {
                    Console.WriteLine("File does not exist.");
                    continue;
                }

                //TODO: Add check for .txt filetype only

                string input = FileManager.ReadFile(inputPath);

                switch (option)
                {
                    case "1":
                        ExecuteRunLength(input, inputPath);
                        break;
                    case "2":
                        ExecuteHuffman(input, inputPath);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }

            Console.WriteLine("Exiting...");
        }

        static void ExecuteRunLength(string input, string inputPath)
        {
            Console.WriteLine("Compressing using Run-Length Encoding...");

            string encoded = RunLengthEncoding.Encode(input);
            string outputPath = inputPath + ".rle.txt";

            FileManager.WriteFile(outputPath, encoded);

            Console.WriteLine($"Output saved to {outputPath}");

            CompressionBenchmarker.MeasureCompression(inputPath, RunLengthEncoding.Encode);
        }

        static void ExecuteHuffman(string input, string inputPath)
        {
            Console.WriteLine("Compressing using Huffman Encoding...");

            var table = HuffmanCompression.BuildHuffmanTable(input);
            var encoded = HuffmanCompression.Encode(input, table);
            string outputPath = inputPath + ".huffman.txt";

            FileManager.WriteFile(outputPath, encoded);

            Console.WriteLine($"Output saved to {outputPath}");

            CompressionBenchmarker.MeasureCompression(inputPath, s => HuffmanCompression.Encode(s, HuffmanCompression.BuildHuffmanTable(s)));
        }
    }
}
