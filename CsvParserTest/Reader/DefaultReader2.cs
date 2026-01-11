using System.Collections;
using System.ComponentModel;
using System.Text;

namespace CsvParser.Reader
{
    internal class DefaultReader2 : ReaderBase
    {
        private readonly StreamReader _inputStream;

        private int _realLength;
        public DefaultReader2(StreamReader inputStream, ReaderConfiguration configuration)
            :base(configuration)
        {
            _inputStream = inputStream;
            _realLength = Configuration.RecordLength;
        }

        public override async Task<Option<string[]>> ReadRecordAsync(CancellationToken cancellationToken = default)
        {
            List<string> result = new(_realLength);
            string overflow = string.Empty;
            bool escaped = false;

            do
            {
                Queue<Token> tokens = new(_realLength * 2);

                var line = await _inputStream.ReadLineAsync(cancellationToken);
                if (line == null) return result.Count > 0 ? Option<string[]>.Some([.. result]) : Option<string[]>.None;

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == Separator) tokens.Enqueue(new() { Start = i, TokenType = TokenType.Separator });
                    else if (line[i] == Escaper) tokens.Enqueue(new() { Start = i, TokenType = TokenType.Escaper });
                }
                tokens.Enqueue(new() { Start = line.Length, TokenType = TokenType.End });

                var start = 0;
                var accum = string.Empty; //TODO: stringbuilder
                while (tokens.Count > 0)
                {
                    var token = tokens.Dequeue();

                    switch (token.TokenType)
                    {
                        case TokenType.End:
                            if (!escaped) result.Add(accum + line[start..token.Start]);
                            else overflow += $"{accum}{line[start..token.Start]}\n";
                            break;

                        case TokenType.Separator:
                            if (escaped) break;

                            if (overflow.Length > 0)
                            {
                                result.Add(overflow + accum + line[start..token.Start]);
                                overflow = string.Empty;
                            }
                            else result.Add(accum + line[start..token.Start]);

                            start = token.Start + 1;
                            accum = string.Empty;
                            break;

                        case TokenType.Escaper:
                            var next = tokens.Peek();

                            switch (next.TokenType)
                            {
                                case TokenType.Escaper:
                                    if (token.Start + 1 != next.Start) goto trivial_case;

                                    accum += line[start..(token.Start + 1)];
                                    start = token.Start + 2;
                                    tokens.Dequeue();
                                    break;

                                case TokenType.End:
                                case TokenType.Separator:
                                trivial_case:
                                    accum += line[start..token.Start];
                                    start = token.Start + 1;
                                    escaped = !escaped;
                                    break;
                            }
                            break;
                    }
                }
            }
            while (overflow.Length > 0);

            if (result.Count > _realLength) _realLength = result.Count;

            return Option<string[]>.Some([.. result]);
        }

        private string ExtractValue(string line, Queue<Token> tokens)
        {
            StringBuilder builder = new();

            do
            {
                var token = tokens.Dequeue();
                if (token.TokenType == TokenType.End)
                {

                }
            }
            while (true);
        }

        private struct Token
        {
            public int Start;
            public TokenType TokenType;
        }

        private enum TokenType
        {
            Separator,
            Escaper,
            End
        }
        
        public override ValueTask DisposeAsync()
        {
            _inputStream.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
