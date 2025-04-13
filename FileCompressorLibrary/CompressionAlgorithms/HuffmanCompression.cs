using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCompressorLibrary.Interfaces;
using FileCompressorLibrary.Models;

namespace FileCompressorLibrary.CompressionAlgorithms
{
    public class HuffmanCompression : IHuffmanCompressor
    {
        private class Node : IComparable<Node>
        {
            public char? Symbol;
            public int Frequency;
            public Node Left, Right;

            public int CompareTo(Node other) => Frequency - other.Frequency;

            public bool IsLeaf => Left == null && Right == null;
        }

        public void Compress(string inputPath, string outputPath)
        {
            string text = File.ReadAllText(inputPath);
            Dictionary<char, int> frequencies = text.GroupBy(c => c)
                                                    .ToDictionary(g => g.Key, g => g.Count());

            // Build Huffman tree
            Node root = BuildTree(frequencies);

            // Build code table
            Dictionary<char, string> codes = new();
            BuildCodes(root, "", codes);

            //Encode the text
            var encoded = new StringBuilder();
            foreach (char c in text)
            {
                encoded.Append(codes[c]);
            }

            // Write encoded data and frequency table
            using var writer = new BinaryWriter(File.Open(outputPath, FileMode.Create));
            writer.Write(frequencies.Count);
            foreach( var kvp in frequencies)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

            byte[] data = BitStringToBytes(encoded.ToString());
            writer.Write(data.Length);
            writer.Write(data);
        }

        public void Decompress(string inputPath, string outputPath) 
        {
            using var reader = new BinaryReader(File.OpenRead(inputPath));
            int tableSize = reader.ReadInt32();
            Dictionary<char, int> frequencies = new();

            for (int i = 0; i < tableSize; i++)
            {
                char symbol = reader.ReadChar();
                int freq = reader.ReadInt32();
                frequencies[symbol] = freq;
            }

            int dataLength = reader.ReadInt32();
            byte[] data = reader.ReadBytes(dataLength);

            string bitString = BytesToBitString(data);

            Node root = BuildTree(frequencies);
            var decoded = new StringBuilder();

            Node current = root;
            foreach (char bit in bitString)
            {
                current = bit == '0' ? current.Left : current.Right;

                if (current.IsLeaf)
                {
                    decoded.Append(current.Symbol.Value);
                    current = root;
                }
            }

            File.WriteAllText(outputPath, decoded.ToString());
        }

        private Node BuildTree(Dictionary<char, int> frequencies)
        {
            var queue = new PriorityQueue<Node, int>();
            foreach (var kvp in frequencies)
            {
                queue.Enqueue(new Node { Symbol = kvp.Key, Frequency = kvp.Value }, kvp.Value);
            }

            while (queue.Count > 1)
            {
                var left = queue.Dequeue();
                var right = queue.Dequeue();
                var parent = new Node
                {
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right,
                };
                queue.Enqueue(parent, parent.Frequency);
            }

            return queue.Dequeue();
        }

        private void BuildCodes(Node node, string code, Dictionary<char, string> codes)
        {
            if (node.IsLeaf)
            {
                codes[node.Symbol!.Value] = code;
                return;
            }

            BuildCodes(node.Left, code + "0", codes);
            BuildCodes(node.Right, code + "1", codes);
        }

        private byte[] BitStringToBytes(string bits)
        {
            int paddedLength = ((bits.Length + 7) / 8) * 8;
            bits = bits.PadRight(paddedLength, '0');
            byte[] result = new byte[bits.Length / 8];

            for (int i = 0; i < result.Length; i++)
            {
                string byteStr = bits.Substring(i * 8, 8);
                result[i] = Convert.ToByte(byteStr, 2);
            }

            return result;
        }

        private string BytesToBitString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }

            return sb.ToString();
        }
    }

    public static class HuffmanCompression_old
    {
        public static Dictionary<char, string> BuildHuffmanTable(string input)
        {
            // Count how many times each character appears
            Dictionary<char, int> characterFrequency = input
                .GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count());

            PriorityQueue<HuffmanNode, int> pq = new();

            foreach (var item in characterFrequency)
            {
                pq.Enqueue(new HuffmanNode(item.Key, item.Value), item.Value);
            }

            while (pq.Count > 1)
            {
                var left = pq.Dequeue();
                var right = pq.Dequeue();
                var parent = new HuffmanNode('\0', left.Frequency +  right.Frequency, left, right);
                pq.Enqueue(parent, parent.Frequency);
            }

            // Build the encoding table
            Dictionary<char, string> huffmanTable = new();
            if (pq.Count > 0)
            {
                BuildTableRecursive(pq.Dequeue(), "", huffmanTable);
            }

            return huffmanTable;
        }

        private static void BuildTableRecursive(HuffmanNode node, string code, Dictionary<char, string> table)
        {
            if (node == null) return;

            if (node.Left == null && node.Right == null)
                table[node.Character] = code;

            BuildTableRecursive(node.Left, code + "0", table);
            BuildTableRecursive(node.Right, code + "1", table);
        }

        public static string Encode(string input, Dictionary<char, string> huffmanTable)
        {
            StringBuilder encoded = new();
            foreach (char c in input)
            {
                encoded.Append(huffmanTable[c]);
            }
            return encoded.ToString();
        }

        public static string Decode(string encoded, HuffmanNode root)
        {
            StringBuilder decoded = new();
            HuffmanNode node = root;

            foreach(char bit in encoded)
            {
                node = bit == '0' ? node.Left : node.Right; 

                if (node.Left == null && node.Right == null)
                {
                    decoded.Append(node.Character);
                    node = root;
                }
            }

            return decoded.ToString();
        }
    }
}
