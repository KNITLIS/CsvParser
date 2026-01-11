using System.Text;

namespace ParserTests
{
    internal static class StringExtension
    {
        extension(string str)
        {
            public StreamReader AsStreamReader()
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
                return new StreamReader(stream);
            }
        }
    }
}
