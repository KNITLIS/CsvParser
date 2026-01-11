using System.Text;

namespace CsvParser.Reader
{
    public class DefaultReader3 : ReaderBase
    {
        private readonly StreamReader _inputStream;

        private int _realLength;
        public DefaultReader3(StreamReader inputStream, ReaderConfiguration configuration)
            :base(configuration)
        {
            _inputStream = inputStream;
            _realLength = Configuration.RecordLength;
        }

        public override async Task<Option<string[]>> ReadRecordAsync(CancellationToken cancellationToken = default)
        {
            List<string> result = new(_realLength);
            bool escaped = false;
            StringBuilder accum = new();
            string? line = null;
            int nextStart = 0;

            do
            {
                line = await _inputStream.ReadLineAsync(cancellationToken);
                if (line == null) return OnStreamEnd();

                nextStart = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    var sym = line[i];
                    if (sym != Separator && sym != Escaper) continue;

                    if (escaped)
                    {
                        if (sym == Separator) continue;

                        if ((i + 1) < line.Length && line[i + 1] == Escaper)
                        {
                            Accumulate(i + 1);
                            i++;
                        }
                        else
                        {
                            Accumulate(i);
                            escaped = false;
                        }

                        continue;
                    }

                    if (sym == Escaper)
                    {
                        Accumulate(i);
                        escaped = true;

                        continue;
                    }

                    if (sym == Separator)
                    {
                        Accumulate(i);
                        result.Add(accum.ToString());
                        accum.Clear();
                    }
                }

                Accumulate(line.Length);

                if (!escaped)
                {
                    result.Add(accum.ToString());
                    accum.Clear();
                }
                else
                {
                    accum.Append('\n');
                }
            }
            while (accum.Length > 0);

            if (result.Count > _realLength) _realLength = result.Count;

            return Option<string[]>.Some([.. result]);



            void Accumulate(int i)
            {
                accum.Append(line.AsSpan(nextStart, i - nextStart));
                nextStart = i + 1;
            }

            Option<string[]> OnStreamEnd()
            {
                if (result.Count == 0) return Option<string[]>.None;

                result.Add(accum.ToString());
                return Option<string[]>.Some([.. result]);
            }
        }
        
        public override ValueTask DisposeAsync()
        {
            _inputStream.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
