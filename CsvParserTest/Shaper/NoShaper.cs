namespace CsvParser.Shaper
{
    public class NoShaper : IShaper<string[]>
    {
        public NoShaper() { }

        public void SetHeaders(string[] headersOrder) { }

        public string[] Shape(string[] record)
        {
            return record;
        }

        public void Dispose() { }
    }
}
