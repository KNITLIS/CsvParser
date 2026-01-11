namespace CsvParser
{
    public class HeadersException(string message) : Exception(message) { }

    public class SyntaxisException : Exception
    {
        public SyntaxisException(string message) : base(message) { }

        public SyntaxisException(int idx, char symbol) : base($"Invalid symbol \"{symbol}\" at {idx}") { }
    }
}
