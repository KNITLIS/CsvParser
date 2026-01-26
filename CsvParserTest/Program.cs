using CsvParser.Reader;
using CsvParser.Attributes;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;

namespace CsvParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.Run(Parse).Wait();
        }

        private static async Task Parse()
        {
            var parser = CsvParserBuilder.GetBuilder()
                //.SetDefaultReader(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("abc,dnf,rtt"))))
                //.SetDefaultReader(new StreamReader("./test.txt"))
                .SetCustomReader((conf) => new DefaultReader3(new StreamReader("./test.txt"), conf))
                .SetDefaultShaper<Account>()
                .Build();

            await foreach (var record in parser)
            {
                Console.WriteLine(record.GetType().Name);
                foreach (var prop in record.GetType().GetProperties())
                {
                    Console.WriteLine($"    {prop.Name}: {((string?)prop.GetValue(record))?.Replace("\n", "\\n")}");
                }
                Console.WriteLine();
            }
        }
    }

    public struct Account
    {
        [CsvPropertyName("email")]
        public string Email { get; set; }

        [CsvPropertyName("password")]
        public string Password { get; set; }

        [CsvPropertyName("nick")]
        public string Nickname { get; set; }

        [CsvPropertyName("avatar")]
        public string Avatar { get; set; }

        [CsvPropertyName("clubId")]
        public string ClubId { get; set; }

        [CsvPropertyName("agentId")]
        public string AgentId { get; set; }

        #region Device

        [CsvPropertyName("device-id")]
        public string DeviceId { get; set; }

        [CsvPropertyName("device-model")]
        public string DeviceModel { get; set; }

        [CsvPropertyName("device-name")]
        public string DeviceName { get; set; }

        [CsvPropertyName("device-osVersion")]
        public string DeviceOsVersion { get; set; }

        #endregion

        #region Socks

        [CsvPropertyName("socks-host")]
        public string SocksHost { get; set; }

        [CsvPropertyName("socks-password")]
        public string SocksPassword { get; set; }

        [CsvPropertyName("socks-port")]
        public string SocksPort { get; set; }

        [CsvPropertyName("socks-user")]
        public string SocksUser { get; set; }

        #endregion
    }
}
