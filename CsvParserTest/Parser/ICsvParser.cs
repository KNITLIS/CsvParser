namespace CsvParser.Parser
{
    public interface ICsvParser<TOut> : IAsyncEnumerable<TOut> 
    {
        Task<Option<TOut>> GetRecord(CancellationToken cancellationToken = default);
    }
}
