namespace CsvParser.Shaper
{
    public interface IShaper<TShape> : IDisposable
    {
        void SetHeaders(string[] headersInOrder);
        TShape Shape(string[] record);
    }
}
