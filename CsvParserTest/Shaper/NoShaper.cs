namespace CsvParser.Shaper
{
    public class NoShaper : ShaperBase<string[]>
    {
        public NoShaper() : base(default) { }

        public override void SetOrder(string[] headersOrder) { }

        public override string[] Shape(string[] record)
        {
            return record;
        }
    }
}
