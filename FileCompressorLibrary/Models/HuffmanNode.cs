using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressorLibrary.Models
{
    // Node in the Huffman binary tree
    public class HuffmanNode
    {
        public char Character { get; }
        public int Frequency { get; }
        public HuffmanNode Left { get; }
        public HuffmanNode Right { get; }

        public HuffmanNode(char character, int frequency, HuffmanNode left = null, HuffmanNode right = null)
        {
            Character = character;
            Frequency = frequency;
            Left = left;
            Right = right;
        }
    }
}
