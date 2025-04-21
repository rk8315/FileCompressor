# FileCompressor

A hands-on project that explores file compression such as the **Huffman encoding algorithm** ([Wikipedia](https://en.wikipedia.org/wiki/Huffman_coding)). This project provides a basic yet functional compression and decompression tool that works with **any file type**, showcasing the inner workings of one of the foundational lossless compression techniques.

Huffman encoding works best on redundant or text-based data. Modern image formats like `.jpg` are already compressed, so gains are minimal as seen here:

#### Huffman Compression Testing

| File Type | Original Size | Compressed Size | Compression | 
|-----------|---------------|-----------------|-------------|
| .txt      | 2.3 MB        | 1.2 MB          | ~48% saved  | 
| .jpg      | 8.487 MB      | 8.485 MB        | <1% saved   | 


---

## Features

- Compress and decompress any file type (`.txt`, `.jpg`, etc.)
- Custom binary output format with compression metadata
- Automatic preservation of original file content
- Pure C# implementation — no external compression libraries used
- Educational and extensible code structure for future enhancements

---

## Huffman Compression

Huffman coding is a **lossless data compression algorithm**. It works by assigning **shorter binary codes to more frequent bytes** and longer codes to less frequent ones, reducing the overall number of bits needed.

Imagine the string `"ABABABAAABABAB"`. Since "A" and "B" repeat often, Huffman coding can represent them with fewer bits than characters like "Z" that appear only once.

### How It Works (Simplified):

1. **Count Frequencies** – Analyze how often each byte appears in the file.
2. **Build Tree** – Construct a binary tree where frequent bytes are closer to the root.
3. **Generate Codes** – Assign binary strings based on tree paths (left = 0, right = 1).
4. **Compress Data** – Replace each byte with its binary code.
5. **Store Metadata** – Save the encoding map and padding info for decompression.

### Huffman Tree Example:

Suppose we want to compress the string: `"ABBCCCDDDDEEEEE"`

#### Step 1: Character Frequencies

| Char | Frequency |
|------|-----------|
| A    | 1         |
| B    | 2         |
| C    | 3         |
| D    | 4         |
| E    | 5         |


#### Step 2: Build the Huffman Tree

Characters with **lower frequencies go deeper**, resulting in **shorter codes for more frequent bytes**.

            [15]
           /    \
       [5:E]    [10]
               /    \
            [4:D]   [6]
                    /   \
                 [3:C]  [3]
                        /  \
                     [2:B] [1:A]

- Each node shows `[Weight:Character]`
- Internal nodes show just `[Weight]`, which is the sum of the two child nodes
- This tree is **binary**, with left = `0` and right = `1`


#### Step 3: Assign Binary Codes (Left=0, Right=1)

Start from the root and record the path:

| Char | Path   | Binary Code |
|------|--------|-------------|
| E    | L      | `0`         |
| D    | R-L    | `10`        |
| C    | R-R-L  | `110`       |
| B    | R-R-R-L| `1110`      |
| A    | R-R-R-R| `1111`      |

So, encoding `"ABBCCCDDDDEEEEE"` becomes:

A -> 1111 

B -> 1110 1110 

C -> 110 110 110 

D -> 10 10 10 10 

E -> 0 0 0 0 0

Encoded Bits: 1111 1110 1110 110 110 110 10 10 10 10 0 0 0 0 0

#### Why It Works

- More frequent characters (like **E**) get shorter codes.
- Less frequent ones (like **A**) get longer codes.
- This balances out the overall bit usage and reduces the total size.

You can see this process in the code by exploring:
- `BuildEncodingTable(...)` — Assigns binary paths
- `Compress(...)` — Encodes input into bits
- `Decompress(...)` — Traverses the tree to decode

---

## Next Steps
- Implement additional compression methods (LZ77, DEFLATE, Zstandard)
- Support archive formats for multi-file compression
- Improved UI
