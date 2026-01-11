namespace CsvParser
{
    public class LineExtractor
    {
        public struct ExtractionRange
        {
            public int Start;
            public int End;
        }

        /// <summary>
        /// range [start, end) (end is excluded)
        /// </summary>
        /// <param name="line"></param>
        /// <param name="ranges"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static (int extracted, bool complete) ExtractFromLine(
            string line, ref ExtractionRange[] ranges,
            bool startEscaped = false, char separator = ',', char escaper = '"')
        {
            if (line.Length == 0)
            {
                ranges[0] = new ExtractionRange() { Start = 0, End = 0 };
                return (1, false);
            }

            var extracted = 0;
            var nextStart = 0;

            do
            {
                if (line[nextStart] == escaper || startEscaped)
                {
                    startEscaped = false;
                     if (extracted != 0) nextStart++;
                    var (start, end, lineComplete) = ExtractQuoted(line, nextStart, escaper);

                    ranges[extracted] = new ExtractionRange() { Start = start, End = end };
                    if (!lineComplete) return (extracted + 1, false);

                    nextStart = ranges[extracted].End + 2;
                }
                else
                {
                    var (start, end) = ExtractNext(line, nextStart, separator);
                    ranges[extracted] = new ExtractionRange() { Start = start, End = end };
                    nextStart = ranges[extracted].End + 1;
                }

                extracted++;
            }
            while (extracted < ranges.Length && nextStart < line.Length);

            // last value is empty
            if (nextStart == line.Length && line[^1] == separator)
            {
                if (extracted == ranges.Length) return (extracted, false);

                ranges[extracted] = new ExtractionRange() { Start = line.Length, End = line.Length };
                extracted++;
            }

            var isExtractedWholeString = ranges[extracted - 1].End == line.Length;

            return (extracted, isExtractedWholeString);
        }

        private static (int start, int end) ExtractNext(string line, int start, char separator)
        {   
            var end = start;

            for (; end < line.Length; end++)
            {
                if (line[end] == separator) break;
            }

            return (start, end);
        }

        private static (int start, int end, bool lineComplete) ExtractQuoted(string line, int start, char escaper)
        {
            var end = start;

            for (; end < line.Length; end++)
            {
                if (line[end] == escaper)
                {
                    // if quote escaped with another quote
                    if (line[end + 1] == escaper) { end++; continue; } //TODO: test for "abc ""cde"", efg" case

                    return (start, end, true);
                }
            }

            // if new line is part of value ("abc \n def",abc)
            return (start, end, false);
        }
    }
}
