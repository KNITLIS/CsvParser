namespace CsvParser.Shaper
{
    public interface IShaper<TShape> : IDisposable
    {
        void SetOrder(string[] headersOrder);
        TShape Shape(string[] record);
    }
}
