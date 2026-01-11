namespace CsvParser.Parser
{
    internal class CsvParserAsyncEnumerator<TOut>(CsvParserBase<TOut> parser, CancellationToken cancellationToken) : IAsyncEnumerator<TOut>
    {
        public TOut Current { get; private set; }

        public ValueTask DisposeAsync() => parser.DisposeAsync();

        public async ValueTask<bool> MoveNextAsync()
        {
            var recordOpt = await parser.GetRecord(cancellationToken);

            if (recordOpt.IsSome(out var recordVal))
            {
                Current = recordVal;
                return true;
            }

            return false;
        }
    }
}
