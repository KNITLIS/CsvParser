using System.Collections;
using CsvParser;

namespace ParserTests
{
    public class ShapelessTests
    {
        [TestCaseSource(typeof(ShapelessTestsData), nameof(ShapelessTestsData.TestCases))]
        public async Task<string[][]> ShapelessTest(string input, int length)
        {
            var parser = CsvParserBuilder.GetBuilder()
                .SetDefaultReader(input.AsStreamReader())
                .ShapeAsStringArray()
                .SetRecordLenegth(length)
                .Build();

            List<string[]> result = [];

            await foreach(var res in parser)
            {
                result.Add(res);
            }

            return [..result];
        }
    }

    public class ShapelessTestsData
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return 
                    new TestCaseData(
                        "abc,asd,fthcdfg"
                        , 3)
                    .Returns(new string[][] { ["abc", "asd", "fthcdfg"] })
                    .SetName("Base")
                    .SetDescription("Super basic test to ensure overall workability");

                yield return
                    new TestCaseData("""
                        abc,asd,fthcdfg
                        sdf,rtynfdg,asdsdv
                        """, 3)
                    .Returns(new string[][]
                    {
                        ["abc", "asd", "fthcdfg"],
                        ["sdf", "rtynfdg", "asdsdv"]
                    })
                    .SetName("Multilline")
                    .SetDescription("Basic multiline test");

                yield return
                    new TestCaseData("""
                        abc,asd,fthcdfg
                        ,,
                        sdf,rtynfdg,asdsdv
                        """, 3)
                    .Returns(new string[][]
                    {
                        ["abc", "asd", "fthcdfg"],
                        ["", "", ""],
                        ["sdf", "rtynfdg", "asdsdv"]
                    })
                    .SetName("Empty line")
                    .SetDescription("Multiline with empty string");

                yield return
                    new TestCaseData("""
                        abc,"asd",fthcdfg
                        "sdf",rtynfdg,"asdsdv"
                        """, 3)
                    .Returns(new string[][]
                    {
                        ["abc", "asd", "fthcdfg"],
                        ["sdf", "rtynfdg", "asdsdv"]
                    })
                    .SetName("Escaped")
                    .SetDescription("Escaped values");

                yield return
                    new TestCaseData("""
                        abc,"asd, df",fthcdfg
                        "sdf",rtynfdg,"asdsdv, gg"
                        """, 3)
                    .Returns(new string[][]
                    {
                        ["abc", "asd, df", "fthcdfg"],
                        ["sdf", "rtynfdg", "asdsdv, gg"]
                    })
                    .SetName("Escaped + ','")
                    .SetDescription("Escaped values with commas");

                yield return
                    new TestCaseData("""""
                        abc,"mn""asd""mn",fthcdfg
                        "sdf",rtynfdg,"asdsdv"
                        """"", 3)
                    .Returns(new string[][]
                    {
                        ["abc", "mn\"asd\"mn", "fthcdfg"],
                        ["sdf", "rtynfdg", "asdsdv"]
                    })
                    .SetName("Escaped + '\"'")
                    .SetDescription("Escaped values with double quotes");

                yield return
                    new TestCaseData("""
                        abc,"asd,
                        mnn",fthcdfg
                        sdf,rtynfdg,asdsdv
                        """, 3)
                    .Returns(new string[][]
                    {
                        ["abc", "asd,\nmnn", "fthcdfg"],
                        ["sdf", "rtynfdg", "asdsdv"]
                    })
                    .SetName("Escaped + '\\n'")
                    .SetDescription("Escaped values with new line");

                yield return
                    new TestCaseData("""
                        abc,"asd,

                        mnn",fthcdfg
                        sdf,rtynfdg,asdsdv
                        """, 3)
                    .Returns(new string[][]
                    {
                        ["abc", "asd,\n\nmnn", "fthcdfg"],
                        ["sdf", "rtynfdg", "asdsdv"]
                    })
                    .SetName("Escaped + double '\\n'")
                    .SetDescription("Escaped values with new line");

                yield return
                    new TestCaseData("""
                        abc,asd,fthcdfg
                        "sdf"
                        """, 3)
                    .Returns(new string[][]
                    {
                        ["abc", "asd", "fthcdfg"],
                        ["sdf"]
                    })
                    .SetName("Variable name")
                    .SetDescription("Variable record length (incorrect syntaxis but supported)");

                yield return
                    new TestCaseData("""
                        abc,asd,fthcdfg,fgfhj,ergdfg
                        sdf,rtynfdg,asdsdv,rthfgh,dfhsgh
                        """, 3)
                    .Returns(new string[][]
                    {
                        ["abc", "asd", "fthcdfg", "fgfhj", "ergdfg"],
                        ["sdf", "rtynfdg", "asdsdv", "rthfgh", "dfhsgh"]
                    })
                    .SetName("Incorrect length")
                    .SetDescription("Incorrect record length (automatically adjusted)");
            }
        }
    }
}
