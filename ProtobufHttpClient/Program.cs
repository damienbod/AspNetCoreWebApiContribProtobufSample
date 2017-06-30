using Model;
using System;
using System.IO;
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

            var table = PostDataToServerAsync().Result;

            Console.WriteLine($"Sent data with Name: {table.Name} to the server");
            Console.ReadLine();
        }

        static async System.Threading.Tasks.Task<Table[]> CallServerAsync()
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:31004/api/tables");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
            var result = await client.SendAsync(request);
            var tables = ProtoBuf.Serializer.Deserialize<Table[]>(await result.Content.ReadAsStreamAsync());
            return tables;
        }

        static async System.Threading.Tasks.Task<Table> PostDataToServerAsync()
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:31004/api/tables");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/protobuf"));

            var table = new Table { Name = "jim", Dimensions = "190x80x90", Description = "top of the range from Migro" };

            MemoryStream stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize<Table>(stream, table);

            HttpContent data = new StreamContent(stream);

            request.Content = data;
            var result = await client.SendAsync(request);

            var resultData =  ProtoBuf.Serializer.Deserialize<Table>(await result.Content.ReadAsStreamAsync());
            return resultData;
        }
    }
}
