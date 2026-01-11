using CsvParser.Reader;
using CsvParser.Shaper;

namespace CsvParser.Parser
{
    internal class CsvTypedParser<TOut> : CsvParserBase<TOut> where TOut : struct
    {
        public readonly IShaper<TOut> Shaper;

        private bool _firstIteration = true;

        internal CsvTypedParser(IReader reader, ParserConfiguration configuration, IShaper<TOut> shaper)
            : base(reader, configuration) 
        {
            Shaper = shaper;
        }

        public override async Task<Option<TOut>> GetRecord(CancellationToken cancellationToken = default)
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

                Shaper.SetOrder(Headers);
            }

            _firstIteration = false;
        }
    }
}
