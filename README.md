# Parser for CSV files (WIP)

> [!WARNING]
> work in progress. Non-backwards-compatible changes could be introdused

use as is
```C#
var parser = CsvParserBuilder.GetBuilder()
    .SetDefaultReader(input.AsStreamReader())
    .ShapeAsStringArray()
    .SetRecordLenegth(length)
    .Build();

await foreach(var res in parser)
{
    ...
}
```
or add custom components
```C#
CsvParserBuilder.GetBuilder()
  .SetCustomReader((conf) => new DefaultReader3(new StreamReader("./test.txt"), conf))
  .SetDefaultShaper<Account>()
  .Build();
```
