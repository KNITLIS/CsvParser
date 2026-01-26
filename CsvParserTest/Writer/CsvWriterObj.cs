namespace CsvParser
{
    public static partial class CsvWriter
    {
        public static string Write<TIn>(TIn input, char separator = DEFAULT_SEPARATOR, char escaper = DEFAULT_ESCAPER) where TIn : struct
        {
            return Write(ExtractProps<TIn>().Select((prop) => prop.GetValue(input)?.ToString() ?? "").ToArray(), separator, escaper);
        }

        public static int Write<TIn>(StreamWriter stream, TIn input, char separator = DEFAULT_SEPARATOR, char escaper = DEFAULT_ESCAPER) where TIn : struct
        {
            var res = Write(ExtractProps<TIn>().Select((prop) => prop.GetValue(input)?.ToString() ?? "").ToArray(), separator, escaper);
            stream.WriteLine(res);
            stream.Flush();
            return res.Length;
        }

        public static int Write<TIn>(Stream stream, TIn input, char separator = DEFAULT_SEPARATOR, char escaper = DEFAULT_ESCAPER) where TIn : struct
        {
            return Write(new StreamWriter(stream), input, separator, escaper);
        }
    }
}
