namespace CsvParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvPropertyNameAttribute(string name) : Attribute
    {
        public string Name { get; private set; } = name;
    }
}
