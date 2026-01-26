using System.Text;

namespace CsvParser
{
    public static partial class CsvWriter
    {
        public static string Write<TIn>(TIn[] inputArr, char separator = DEFAULT_SEPARATOR, char escaper = DEFAULT_ESCAPER) where TIn : struct
        {
            var res = new StringBuilder(inputArr.Length);
            var propsInfo = ExtractProps<TIn>();

            foreach (var inputVal in inputArr)
            {
                res.AppendLine(
                    Write(propsInfo.Select((prop) => prop.GetValue(inputVal)?.ToString() ?? "").ToArray(),
                        separator, escaper));
            }

            return res.ToString();
        }

        public static int Write<TIn>(StreamWriter stream, TIn[] inputArr, char separator = DEFAULT_SEPARATOR, char escaper = DEFAULT_ESCAPER) where TIn : struct
        {
            int res = 0;
            var propsInfo = ExtractProps<TIn>();

            foreach (var inputVal in inputArr)
            {
                var resStr = Write(propsInfo.Select((prop) => prop.GetValue(inputVal)?.ToString() ?? "").ToArray(),
                        separator, escaper);
                stream.WriteLine(resStr);
                res += resStr.Length;
            }

            stream.Flush();
            return res;
        }

        public static int Write<TIn>(Stream stream, TIn[] input, char separator = DEFAULT_SEPARATOR, char escaper = DEFAULT_ESCAPER) where TIn : struct
        {
            return Write(new StreamWriter(stream), input, separator, escaper);
        }
    }
}
