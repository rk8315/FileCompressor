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
            public byte? Byte { get; set; }
            public int Frequency { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public bool IsLeaf => Left == null && Right == null;

            public int CompareTo(Node other)
            {
                return Frequency.CompareTo(other.Frequency);
            }
        }

        private Dictionary<byte, string> _encodingTable;

        public byte[] Compress(byte[] input)
        {
            if (input == null || input.Length == 0)
                throw new ArgumentException("Input data cannot be null or empty.");

            // Frequency table
            var frequencies = new Dictionary<byte, int>();
            foreach (var b in input)
            {
                if (!frequencies.ContainsKey(b))
                    frequencies[b] = 0;
                frequencies[b]++;
            }

            // Build Huffman tree
            var priorityQueue = new PriorityQueue<Node, int>();
            foreach (var kvp in frequencies)
            {
                var node = new Node { Byte = kvp.Key, Frequency = kvp.Value };
                priorityQueue.Enqueue(node, node.Frequency);
            }

            while (priorityQueue.Count > 1)
            {
                var left = priorityQueue.Dequeue();
                var right = priorityQueue.Dequeue();
                var parent = new Node
                {
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right,
                };
                priorityQueue.Enqueue(parent, parent.Frequency);
            }

            var root = priorityQueue.Dequeue();

            // Build encode table (byte -> bitstring)
            _encodingTable = new Dictionary<byte, string>();
            BuildEncodingTable(root, "");

            // Encode the data
            var bitString = string.Concat(input.Select(b => _encodingTable[b]));

            // Put bits into bytes
            var outputBytes = new List<byte>();

            // Write original input length (4 bytes)
            outputBytes.AddRange(BitConverter.GetBytes(input.Length));

            // Write number of unique bytes to header
            outputBytes.Add((byte)frequencies.Count);

            // Write byte and its frequency to header (4 bytes per frequency)
            foreach(var kvp in frequencies)
            {
                outputBytes.Add(kvp.Key);
                outputBytes.AddRange(BitConverter.GetBytes(kvp.Value));
            }

            // Data: actual encoded data bits, pad to bytes
            int extraBits = (8 - (bitString.Length % 8)) % 8;
            bitString = bitString.PadRight(bitString.Length + extraBits, '0');

            for (int i = 0; i < bitString.Length; i += 8)
            {
                var byteStr = bitString.Substring(i, 8);
                outputBytes.Add(Convert.ToByte(byteStr, 2));
            }

            // save padding count to footer
            outputBytes.Add((byte)extraBits);

            return outputBytes.ToArray();
        }

        public byte[] Decompress(byte[] compressedData) 
        {
            if (compressedData == null || compressedData.Length == 0)
                throw new ArgumentException("Compressed data cannot be null or empty");

            var pointer = 0;
            int originalLength = BitConverter.ToInt32(compressedData, pointer);
            pointer += 4;

            // read number of unique bytes
            int uniqueCount = compressedData[pointer++];
            var frequencies = new Dictionary<byte, int>();

            // read frequency table
            for (int i = 0; i < uniqueCount; i++)
            {
                byte b = compressedData[pointer++];
                int freq = BitConverter.ToInt32(compressedData, pointer);
                pointer += 4;
                frequencies[b] = freq;
            }

            // rebuild huffman tree
            var priorityQueue = new PriorityQueue<Node, int>();
            foreach (var kvp in frequencies)
            {
                var node = new Node { Byte = kvp.Key, Frequency = kvp.Value };
                priorityQueue.Enqueue(node, node.Frequency);
            }

            while (priorityQueue.Count > 1)
            {
                var left = priorityQueue.Dequeue();
                var right = priorityQueue.Dequeue();
                var parent = new Node
                {
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                };
                priorityQueue.Enqueue(parent, parent.Frequency);
            }

            var root = priorityQueue.Dequeue();

            // extract encoded data without final padding byte
            int paddingBits = compressedData[^1];
            int dataLength = compressedData.Length - pointer - 1;
            var bitList = new List<char>();

            for (int i = 0; i < dataLength; i++)
            {
                string bits = Convert.ToString(compressedData[pointer++], 2).PadLeft(8, '0');
                bitList.AddRange(bits);
            }

            bitList.RemoveRange(bitList.Count - paddingBits, paddingBits); // remove padding

            // Decoode the bits using huffman tree
            var output = new List<byte>();
            var current = root;
            foreach (var bit in bitList)
            {
                current = (bit == '0') ? current.Left : current.Right;
                if (current.IsLeaf)
                {
                    output.Add(current.Byte.Value);
                    current = root;

                    if (output.Count == originalLength)
                        break;
                }
            }

            return output.ToArray();
        }

        private void BuildEncodingTable(Node node, string path)
        {
            if (node == null)
                return;

            if (node.IsLeaf && node.Byte.HasValue)
            {
                _encodingTable[node.Byte.Value] = path;
            }

            BuildEncodingTable(node.Left, path + "0");
            BuildEncodingTable(node.Right, path + "1");
        }
    }
}
