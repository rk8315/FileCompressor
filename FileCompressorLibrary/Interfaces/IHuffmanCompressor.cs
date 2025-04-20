using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressorLibrary.Interfaces
{
    public interface IHuffmanCompressor
    {
        byte[] Compress(byte[] input);
        byte[] Decompress(byte[] compressedData);
    }
}
