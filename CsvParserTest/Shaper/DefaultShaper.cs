using System.Reflection;

namespace CsvParser.Shaper
{
    public class DefaultShaper<TShape> : IShaper<TShape> where TShape : struct
    {
        public ShaperConfiguration Configuration;

        protected readonly PropertyInfo[] _shapeProps;
        protected int[] _inputOrder;

        public DefaultShaper(ShaperConfiguration configuration)
        {
            Configuration = configuration;

            _shapeProps = typeof(TShape).GetProperties();
            _inputOrder = new int[_shapeProps.Length];
            SetHeaders(TypeUtils.SortProps(_shapeProps).Select(TypeUtils.GetCsvName).ToArray());
        }

        public TShape Shape(string[] record)
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

        public void SetHeaders(string[] headersInOrder)
        {
            _inputOrder = new int[headersInOrder.Length];
            var propNames = _shapeProps.Select(TypeUtils.GetCsvName).ToArray();

            for (int i = 0; i < headersInOrder.Length; i++)
            {
                _inputOrder[i] = propNames.IndexOf(headersInOrder[i]);
            }
        }

        public void Dispose() { }
    }
}
