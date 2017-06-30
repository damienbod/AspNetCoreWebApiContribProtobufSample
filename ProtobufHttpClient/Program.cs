using Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ProtobufHttpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Get some data using protobuf");

            var tables = CallServerAsync().Result;

            Console.WriteLine($"Got a list of tables with {tables.Length} items");
            Console.ReadLine();
        }

        static async System.Threading.Tasks.Task<Table[]> CallServerAsync()
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:30515/api/tables");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
            var result = await client.SendAsync(request);
            var tables = ProtoBuf.Serializer.Deserialize<Table[]>(await result.Content.ReadAsStreamAsync());
            return tables;
        }
    }
}
