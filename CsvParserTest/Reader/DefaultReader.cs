namespace CsvParser.Reader
{
    internal class DefaultReader : ReaderBase
    {
        private readonly StreamReader _inputStream;

        private LineExtractor.ExtractionRange[] buffer;
        public DefaultReader(StreamReader inputStream, ReaderConfiguration configuration)
            :base(configuration)
        {
            _inputStream = inputStream;

            buffer = new LineExtractor.ExtractionRange[Configuration.RecordLength];
        }


        public override async Task<Option<string[]>> ReadRecordAsync(CancellationToken cancellationToken = default)
        {
            var result = new List<string>(Configuration.RecordLength);

            int extracted;
            bool completed = false;

            do
            {
                var line = await _inputStream.ReadLineAsync(cancellationToken);
                if (line == null) return result.Count > 0 ? Option<string[]>.Some([.. result]) : Option<string[]>.None;
                if (line.Length == 0 && result.Count == 0) continue;

                rerun_extraction:
                (extracted, completed) = LineExtractor.ExtractFromLine(line, ref buffer, result.Count != 0, Separator, Escaper);

                //dynamicaly adjust record length
                if (!completed && extracted == buffer.Length)
                {
                    buffer = new LineExtractor.ExtractionRange[buffer.Length * 2];
                    goto rerun_extraction;
                }

                //for escaped multiline record
                if (result.Count != 0)
                {
                    result[^1] += '\n';
                    result[^1] += line[buffer[0].Start..buffer[0].End];
                }

                for (var i = result.Count == 0 ? 0 : 1; i < extracted; i++)
                {
                    result.Add(line[buffer[i].Start..buffer[i].End]);
                }
            }
            while (!completed);

            return Option<string[]>.Some([.. result]);
        }

        public override ValueTask DisposeAsync()
        {
            _inputStream.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
