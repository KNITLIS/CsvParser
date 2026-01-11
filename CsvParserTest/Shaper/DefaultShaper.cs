namespace CsvParser.Shaper
{
    public class DefaultShaper<TShape>(ShaperConfiguration configuration) 
        : ShaperBase<TShape>(configuration) where TShape : struct
    {
        public override TShape Shape(string[] record)
        {
            TShape shaper = new();

            object boxed = shaper;

            for (int i = 0; i < record.Length; i++)
            {
                if (i >= _inputOrder.Length) break;
                if (_inputOrder[i] == -1) continue;

                var prop = _shapeProps[_inputOrder[i]];

                prop.SetValue(boxed, record[i]);
            }

            return (TShape) boxed;
        }
    }
}
