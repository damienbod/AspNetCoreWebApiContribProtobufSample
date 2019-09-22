using Model;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufHttpClient
{
    class Program
    {
        public static void Main(string[] args) => RunAsync().GetAwaiter().GetResult();
        public static async Task RunAsync()
        {
            HttpClient httpClient = new HttpClient();
            Console.WriteLine("Get some data using protobuf");

            var tables = await CallServerAsync(httpClient);

            Console.WriteLine($"Got a list of tables with {tables.Length} items");
            Console.ReadLine();

            await WriteProtobufData(httpClient);

            Console.WriteLine($"Sent data to the server");
            Console.ReadLine();
        }

        static async Task<Table[]> CallServerAsync(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:31004/api/tables");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
            var result = await client.SendAsync(request);
            var tables = ProtoBuf.Serializer.Deserialize<Table[]>(await result.Content.ReadAsStreamAsync());
            return tables;
        }

        private static async Task WriteProtobufData(HttpClient client)
        {
            MemoryStream stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize<Table>(stream, new Table
            {
                Name = "jim",
                Dimensions = "190x80x90",
                Description = "top of the range from Migro",
                IntValue = 324567
            });
            var data = stream.ToArray();
            var content = new ByteArrayContent(data, 0, data.Length);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:31004/api/tables")
            {
                Content = content
            };

            var responseForPost = await client.SendAsync(request);
        }

    }
}
