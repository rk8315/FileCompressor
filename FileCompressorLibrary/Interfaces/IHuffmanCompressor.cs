using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressorLibrary.Interfaces
{
    public interface IHuffmanCompressor
    {
        void Compress(string inputFilePath, string outputFilePath);
        void Decompress(string inputFilePath, string outputFilePath);
    }
}
