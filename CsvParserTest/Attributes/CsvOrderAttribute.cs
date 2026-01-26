using System.Runtime.CompilerServices;

namespace CsvParser.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class CsvOrderAttribute([CallerLineNumber]int order = -1) : Attribute
    {
        public int Order { get; set; } = order;
    }
}
