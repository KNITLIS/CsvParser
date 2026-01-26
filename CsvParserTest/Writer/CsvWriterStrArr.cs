using System.Text;

namespace CsvParser
{
    public static partial class CsvWriter
    {
        public static string Write(string[] input, char separator = DEFAULT_SEPARATOR, char escaper = DEFAULT_ESCAPER)
        {
            StringBuilder sb = new();

            foreach (string inputValue in input)
            {
                sb.Append(Sanitize(inputValue, separator, escaper));
            }

            return sb.ToString();
        }

        public static int Write(StreamWriter stream, string[] input, char separator = DEFAULT_SEPARATOR, char escaper = DEFAULT_ESCAPER)
        {
            var res = Write(input, separator, escaper);
            stream.WriteLine(res);
            stream.Flush();
            return res.Length;
        }

        public static int Write(Stream stream, string[] input, char separator = DEFAULT_SEPARATOR, char escaper = DEFAULT_ESCAPER)
        {
            return Write(new StreamWriter(stream), input, separator, escaper);
        }
    }
}
