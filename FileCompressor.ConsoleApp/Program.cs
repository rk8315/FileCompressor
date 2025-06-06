﻿using FileCompressorLibrary;
using FileCompressorLibrary.CompressionAlgorithms;
using FileCompressorLibrary.Interfaces;
using FileCompressorLibrary.Models;

namespace FileCompressor.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IHuffmanCompressor huffman = new HuffmanCompression();

            Console.WriteLine("+++ File Compression Test +++");

            while (true)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1. Run-Length Compression");
                Console.WriteLine("2. Run-Length Decompression");
                Console.WriteLine("3. Huffman Compress");
                Console.WriteLine("4. Huffman Decompress");
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

                string input = FileManager.ReadFile(inputPath);
                string outputPath;

                switch (option)
                {
                    case "1": // Run-Length Compression
                        ExecuteRunLengthEncode(input, inputPath);
                        break;
                    case "2": // Run-Length Decompression
                        ExecuteRunLengthDecode(input, inputPath);
                        break;
                    case "3": // Huffman Compress
                        byte[] originalData = FileManager.ReadBytes(inputPath);
                        byte[] compressed = huffman.Compress(originalData);
                        outputPath = Path.ChangeExtension(inputPath, ".bkhuff");
                        FileManager.WriteBytes(outputPath, compressed);

                        CompressionBenchmarker.MeasureCompression(
                            () => huffman.Compress(originalData),
                            inputPath,
                            outputPath
                         );

                        Console.WriteLine($"Huffman compression complete. Output: {outputPath}");
                        break;
                    case "4": // Huffman Decompress
                        byte[] compressedData = FileManager.ReadBytes(inputPath);
                        byte[] decompressed = huffman.Decompress(compressedData);
                        outputPath = Path.ChangeExtension(inputPath, ".original");
                        FileManager.WriteBytes(outputPath, decompressed);
                        Console.WriteLine($"Compressed bytes: {compressedData.Length}");
                        Console.WriteLine($"Decompressed bytes: {decompressed.Length}");
                        Console.WriteLine($"Huffman decompression complete. Output: {outputPath}");
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }

            Console.WriteLine("Exiting...");
        }

        static void ExecuteRunLengthEncode(string input, string inputPath)
        {
            Console.WriteLine("Compressing using Run-Length Encoding...");

            string encoded = RunLengthEncoding.Encode(input);
            string outputPath = inputPath + ".rle.bin";

            FileManager.WriteFile(outputPath, encoded);

            Console.WriteLine($"Output saved to {outputPath}");

            CompressionBenchmarker.MeasureCompression(inputPath, RunLengthEncoding.Encode);
        }

        static void ExecuteRunLengthDecode(string input, string inputPath)
        {
            Console.WriteLine("Decompressing using Run-Length Encoding...");

            string encoded = RunLengthEncoding.Decode(input);
            string outputPath = inputPath + ".rle.txt";

            FileManager.WriteFile(outputPath, encoded);

            Console.WriteLine($"Output saved to {outputPath}");

            CompressionBenchmarker.MeasureCompression(inputPath, RunLengthEncoding.Decode);
        }
    }
}
