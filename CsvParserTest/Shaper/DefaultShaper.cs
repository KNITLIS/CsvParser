using CsvParser.Attributes;
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
            _inputOrder = Enumerable.Range(0, _shapeProps.Length).ToArray();
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
            var propNames = _shapeProps.Select(prop =>
            {
                var attr = Attribute.GetCustomAttribute(prop, typeof(CsvPropertyNameAttribute));

                return attr == null ? prop.Name : ((CsvPropertyNameAttribute)attr).Name;
            }).ToArray();

            for (int i = 0; i < headersInOrder.Length; i++)
            {
                _inputOrder[i] = propNames.IndexOf(headersInOrder[i]);
            }
        }

        public void Dispose() { }
    }
}
