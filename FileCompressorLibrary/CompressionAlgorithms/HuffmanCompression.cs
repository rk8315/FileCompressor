using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCompressorLibrary.Models;

namespace FileCompressorLibrary.CompressionAlgorithms
{
    public static class HuffmanCompression
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
