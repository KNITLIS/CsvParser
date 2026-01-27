using CsvParser.Attributes;
using System.Reflection;

namespace CsvParser
{
    internal static class TypeUtils
    {
        public static PropertyInfo[] ExtractProps<T>() => typeof(T).GetProperties();

        public static string GetCsvName(PropertyInfo prop) =>
            prop.GetCustomAttribute<CsvPropertyNameAttribute>()?.Name ?? prop.Name;

        public static PropertyInfo[] SortProps(PropertyInfo[] props)
        {
            var propsCopy = new PropertyInfo[props.Length];
            props.CopyTo(propsCopy);

            propsCopy.Sort(static (a, b) =>
            {
                var ordA = a.GetCustomAttribute<CsvOrderAttribute>()?.Order;
                var ordB = b.GetCustomAttribute<CsvOrderAttribute>()?.Order;

                if (ordA == null && ordB == null) return a.Name.CompareTo(b.Name);

                if (ordA == null) return 1;
                if (ordB == null) return -1;

                return ((int)ordA).CompareTo((int)ordB);
            });

            return propsCopy;
        }
    }
}
