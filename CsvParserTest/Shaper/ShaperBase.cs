using CsvParser.Attributes;
using System.Reflection;

namespace CsvParser.Shaper
{
    public abstract class ShaperBase<TShape> : IShaper<TShape>
    {
        public ShaperConfiguration Configuration;

        protected readonly PropertyInfo[] _shapeProps;
        protected int[] _inputOrder;

        public ShaperBase(ShaperConfiguration configuration)
        {
            Configuration = configuration;

            _shapeProps = typeof(TShape).GetProperties();
            _inputOrder = Enumerable.Range(0, _shapeProps.Length).ToArray();
        }
        
        public void ResetOrder() => _inputOrder = Enumerable.Range(0, _shapeProps.Length).ToArray();
        public virtual void SetOrder(string[] headersInOrder)
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

        public abstract TShape Shape(string[] record);

        public virtual void Dispose() { }
    }
}
