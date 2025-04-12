using System.Text;

namespace FileCompressorLibrary.CompressionAlgorithms
{
    public static class RunLengthEncoding
    {
        public static string Encode(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            StringBuilder encoded = new StringBuilder();
            int count = 1;

            // Loop through string and count consecutive characters
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == input[i - 1])
                {
                    count++;
                }
                else
                {
                    encoded.Append(input[i - 1]).Append(count);
                    count = 1;
                }
            }

            encoded.Append(input[^1]).Append(count);
            return encoded.ToString();
        }

        public static string Decode(string encoded)
        {
            if (string.IsNullOrEmpty(encoded)) return "";

            StringBuilder decoded = new StringBuilder();

            // Loop through pairs (character + count)
            for (int i = 0; i < encoded.Length; i += 2)
            {
                char character = encoded[i];
                int count = int.Parse(encoded[i + 1].ToString());

                decoded.Append(character, count);
            }

            return decoded.ToString();
        }
    }
}
