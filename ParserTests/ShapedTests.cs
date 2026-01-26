using CsvParser;
using CsvParser.Attributes;

namespace ParserTests
{
    public class ShapedTests
    {
        [TestCaseSource(typeof(ShapedTestsData), nameof(ShapedTestsData.TestCases))]
        public async Task<Shaper[]> ShapedTest(string input)
        {
            var parser = CsvParserBuilder.GetBuilder()
                .SetDefaultReader(input.AsStreamReader())
                .SetDefaultShaper<Shaper>()
                .HasHeaders()
                .SetRecordLenegth(3)
                .Build();

            List<Shaper> result = [];

            await foreach (var res in parser)
            {
                result.Add(res);
            }

            return [.. result];
        }

        // Might be flickering but whatever, it's testing dangerous functionality 
        [TestCaseSource(typeof(ShapedTestsData), nameof(ShapedTestsData.TestCasesHeaderless))]
        public async Task<Shaper[]> ShapedTests_headerless(string input)
        {
            var parser = CsvParserBuilder.GetBuilder()
                .SetDefaultReader(input.AsStreamReader())
                .SetDefaultShaper<Shaper>()
                .SetRecordLenegth(3)
                .Build();

            List<Shaper> result = [];

            await foreach (var res in parser)
            {
                result.Add(res);
            }

            return [.. result];
        }

        public class ShapedTestsData
        {
            public static IEnumerable<TestCaseData> TestCases
            {
                get
                {
                    yield return
                        new TestCaseData(Header + """
                            sdf,rthg,ertfh
                            rybfj,ryvety,ervtser
                            """)
                        .Returns(new Shaper[]
                        {
                            new() {
                                A = "sdf",
                                B = "rthg",
                                C = "ertfh"
                            },
                            new()
                            {
                                A = "rybfj",
                                B = "ryvety",
                                C = "ervtser"
                            }
                        })
                        .SetName("Basic")
                        .SetDescription("Basic test to ensure shaping capability");

                    yield return
                        new TestCaseData("""
                            C,field1,field2
                            sdf,rthg,ertfh
                            rybfj,ryvety,ervtser
                            """)
                        .Returns(new Shaper[]
                        {
                            new()
                            {
                                A = "rthg",
                                B = "ertfh",
                                C = "sdf"
                            },
                            new()
                            {
                                A = "ryvety",
                                B = "ervtser",
                                C = "rybfj"
                            }
                        })
                        .SetName("Headers order")
                        .SetDescription("Shape with shuffled order of headers");

                    yield return
                        new TestCaseData(Header + """
                            sdf,rthg,ertfh,fgh
                            rybfj,ryvety,ervtser,erhb
                            """)
                        .Returns(new Shaper[]
                        {
                            new() {
                                A = "sdf",
                                B = "rthg",
                                C = "ertfh"
                            },
                            new()
                            {
                                A = "rybfj",
                                B = "ryvety",
                                C = "ervtser"
                            }
                        })
                        .SetName("More than headers")
                        .SetDescription("Extra values over what headers states");

                    yield return
                        new TestCaseData(Header + """
                            sdf,rthg
                            rybfj,ryvety
                            """)
                        .Returns(new Shaper[]
                        {
                            new() {
                                A = "sdf",
                                B = "rthg",
                                C = null
                            },
                            new()
                            {
                                A = "rybfj",
                                B = "ryvety",
                                C = null
                            }
                        })
                        .SetName("Less than headers")
                        .SetDescription("Less values than what headers states");

                    yield return
                        new TestCaseData("""
                            field1,fied2,C
                            sdf,rthg,ertfh
                            rybfj,ryvety,ervtser
                            """)
                        .Returns(new Shaper[]
                        {
                            new() {
                                A = "sdf",
                                B = null,
                                C = "ertfh"
                            },
                            new()
                            {
                                A = "rybfj",
                                B = null,
                                C = "ervtser"
                            }
                        })
                        .SetName("Misspell")
                        .SetDescription("Misspelled headers should be skipped");
                }
            }

            public static IEnumerable<TestCaseData> TestCasesHeaderless
            {
                get
                {
                    yield return
                        new TestCaseData("""
                            sdf,rthg,ertfh
                            rybfj,ryvety,ervtser
                            """)
                        .Returns(new Shaper[]
                        {
                            new() {
                                A = "sdf",
                                B = "rthg",
                                C = "ertfh"
                            },
                            new()
                            {
                                A = "rybfj",
                                B = "ryvety",
                                C = "ervtser"
                            }
                        })
                        .SetName("Headerless")
                        .SetDescription("Shaping input without headers");
                }
            }
        }

        public static string Header = "field1,field2,C\n";

        public struct Shaper
        {
            [CsvPropertyName("field1")]
            public string A { get; set; }
            [CsvPropertyName("field2")]
            public string B { get; set; }
            public string C { get; set; }
        }
    }
}
