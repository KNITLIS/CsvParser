using CsvParser.Attributes;
using System.Reflection;
using System.Text;

namespace CsvParser
{
    public static partial class CsvWriter
    {
        public const char DEFAULT_SEPARATOR = ',';
        public const char DEFAULT_ESCAPER = '"';
        
        private static string Sanitize(string input, char separator, char escaper)
        {
            var sep = false;
            var esc = false;

            List<int> idxs = new(2);
            
            for(int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (c == separator) sep = true;
                if (c == escaper)
                {
                    esc = true;
                    idxs.Add(i);
                }
            }

            if (!(sep || esc)) return input;

            if (!esc) return $"{escaper}{input}{escaper}";

            StringBuilder sb = new(2 * idxs.Count + 3);
            sb.Append(escaper);

            int nextStart = 0;
            foreach (var idx in idxs)
            {
                sb.Append(input.AsSpan(nextStart, idx - nextStart));
                sb.Append(escaper);
                nextStart = idx + 1;
            }
            sb.Append(input.AsSpan(nextStart, input.Length - nextStart));
            sb.Append(escaper);

            return sb.ToString();
        }

        private static PropertyInfo[] ExtractProps<T>()
        {
            var props = typeof(T).GetProperties();

            props.Sort(static (a, b) =>
            {
                var ordA = a.GetCustomAttribute<CsvOrderAttribute>()?.Order;
                var ordB = b.GetCustomAttribute<CsvOrderAttribute>()?.Order;

                if (ordA == null && ordB == null) return a.Name.CompareTo(b.Name);

                if (ordA == null) return 1;
                if (ordB == null) return -1;

                return ((int)ordA).CompareTo((int)ordB);
            });

            return props.ToArray();
        }
    }
}
