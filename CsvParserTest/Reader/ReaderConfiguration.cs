namespace CsvParser.Reader
{
    public struct ReaderConfiguration
    {
        public char Separator { get; set; } = ',';
        public char Escaper { get; set; } = '"';
        public int RecordLength { get; set; } = 8;

        public ReaderConfiguration() { }
    }
}
