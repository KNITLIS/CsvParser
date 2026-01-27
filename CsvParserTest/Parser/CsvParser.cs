using CsvParser.Reader;
using CsvParser.Shaper;

namespace CsvParser.Parser
{
    internal class CsvParser<TOut> : ICsvParser<TOut>
    {
        public readonly ParserConfiguration Configuration;

        public readonly IReader Reader;
        public readonly IShaper<TOut> Shaper;

        public bool HasHeaders => Configuration.HasHeaders;
        public string[] Headers { get; protected set; } = [];

        private bool _firstIteration = true;

        internal CsvParser(IReader reader, ParserConfiguration configuration, IShaper<TOut> shaper)
        {
            Reader = reader;
            Configuration = configuration;
            Shaper = shaper;
        }

        public async Task<Option<TOut>> GetRecord(CancellationToken cancellationToken = default)
        {
            if (_firstIteration) await FirstIter(cancellationToken);

            var values = await Reader.ReadRecordAsync(cancellationToken);
            if (values.IsSome(out var record)) return Option<TOut>.Some(Shaper.Shape(record));
            return Option<TOut>.None;
        }

        private async Task FirstIter(CancellationToken cancellationToken)
        {
            if (HasHeaders)
            {
                var headersOpt = await Reader.ReadRecordAsync(cancellationToken);

                if (headersOpt.IsSome(out var headersVal)) Headers = headersVal;
                else throw new HeadersException("Attempt to read headers from empty file");

                if (Configuration.EnsureHeadersMatch)
                {
                    if (!AreHeadersMatch()) throw new HeadersException("Headers doesn't match");
                }

                Shaper.SetHeaders(Headers);
            }

            _firstIteration = false;
        }

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
