namespace CsvParser.Reader
{
    public abstract class ReaderBase : IReader
    {
        public ReaderConfiguration Configuration;

        public char Separator => Configuration.Separator;
        public char Escaper => Configuration.Escaper;

        public ReaderBase(ReaderConfiguration configuration)
        {
            Configuration = configuration;
        }

        public abstract Task<Option<string[]>> ReadRecordAsync(CancellationToken cancellationToken = default);

        public abstract ValueTask DisposeAsync();
    }
}
