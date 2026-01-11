using CsvParser.Reader;

namespace CsvParser.Parser
{
    internal class CsvUntypedParser : CsvParserBase<string[]>
    {
        internal CsvUntypedParser(IReader reader, ParserConfiguration configuration)
            : base(reader, configuration) { }

        public override async Task<Option<string[]>> GetRecord(CancellationToken cancellationToken = default)
        {
            return await Reader.ReadRecordAsync(cancellationToken);
        }
    }
}
