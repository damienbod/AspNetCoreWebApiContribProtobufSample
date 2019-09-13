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

            // NOT working
            //var table = await PostStreamDataToServerAsync(httpClient);

            //Console.WriteLine($"Sent data with Name: {table.Name} to the server");
            //Console.ReadLine();

            var tableB = await PostStringDataToServerAsync(httpClient);

            Console.WriteLine($"Sent data with Name: {tableB.Name} to the server");
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

        //static async Task<Table> PostStreamDataToServerAsync(HttpClient client)
        //{
        //    // not working...
        //    client.DefaultRequestHeaders
        //          .Accept
        //          .Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));

        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
        //        "http://localhost:31004/api/tables");
        //    request.Headers.TryAddWithoutValidation("CONTENT-TYPE", "application/x-protobuf");

        //    MemoryStream stream = new MemoryStream();
        //    ProtoBuf.Serializer.Serialize<Table>(stream, new Table
        //    {
        //        Name = "jim",
        //        Dimensions = "190x80x90",
        //        Description = "top of the range from Migro"
        //    });

        //    stream.Position = 0;
        //    request.Content = new StreamContent(stream);

        //    // HTTP POST with Protobuf Request Body
        //    var responseForPost = await client.SendAsync(request);

        //    var resultData = ProtoBuf.Serializer.Deserialize<Table>(await responseForPost.Content.ReadAsStreamAsync());
        //    return resultData;
        //}
        static async Task<Table> PostStringDataToServerAsync(HttpClient client)
        {
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));//ACCEPT header

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                "http://localhost:31004/api/tables");

            MemoryStream stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize<Table>(stream, new Table {
                Name = "jim", Dimensions = "190x80x90", Description = "top of the range from Migro"
            });

            stream.Position = 0;
            var sr = new StreamReader(stream);
            var myStr = sr.ReadToEnd();

            request.Content = new StringContent(myStr,
                Encoding.UTF8,
                "application/x-protobuf");//CONTENT-TYPE header

            var responseForPost = await client.SendAsync(request);

            var resultData = ProtoBuf.Serializer.Deserialize<Table>(await responseForPost.Content.ReadAsStreamAsync());
            return resultData;
        }

    }
}
