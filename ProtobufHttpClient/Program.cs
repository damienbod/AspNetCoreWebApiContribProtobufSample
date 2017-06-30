using Model;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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
            HttpClient client = new HttpClient();
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

            var responseForPost = client.SendAsync(request).Result;

            var resultData = ProtoBuf.Serializer.Deserialize<Table>(await responseForPost.Content.ReadAsStreamAsync());
            return resultData;
        }
    }
}
