namespace CsvParser.Reader
{
    public interface IReader : IAsyncDisposable
    {
        public Task<Option<string[]>> ReadRecordAsync(CancellationToken cancellationToken = default);
    }
}
