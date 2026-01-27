namespace CsvParser.Parser
{
    public interface ICsvParser<TOut> : IAsyncEnumerable<TOut>, IAsyncDisposable
    {
        Task<Option<TOut>> GetRecord(CancellationToken cancellationToken = default);
    }
}
