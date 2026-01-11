using CsvParser.Reader;

namespace CsvParser.Parser
{
    public abstract class CsvParserBase<TOut> : ICsvParser<TOut>
    {
        public readonly ParserConfiguration Configuration;
        
        public readonly IReader Reader;

        public bool HasHeaders => Configuration.HasHeaders;


        public string[] Headers { get; protected set; } = [];

        public CsvParserBase(IReader reader, ParserConfiguration configuration)
        {
            Reader = reader;
            Configuration = configuration;
        }

        public abstract Task<Option<TOut>> GetRecord(CancellationToken cancellationToken = default);


        public bool AreHeadersMatch() => new HashSet<string>(Headers).SetEquals(typeof(TOut).GetProperties().Select(prop => prop.Name));

        public IAsyncEnumerator<TOut> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new CsvParserAsyncEnumerator<TOut>(this, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return Reader.DisposeAsync();
        }
    }
}
