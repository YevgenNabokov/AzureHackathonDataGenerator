using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData
{
    public class DatesWriter
    {
        public static async void WriteDates(string endpoint, string primaryKey, string databaseName, string containerName = "test-records")
        {
            var cosmosClient = new CosmosClient(endpoint, primaryKey);

            var container = cosmosClient.GetContainer(databaseName, containerName);

            var rnd = new Random();

            Console.WriteLine("Creating records...");

            //// Creates timestamps with random datetimes, marking with flag the ones that are between 20:00 and 21:00
            for (var i = 0; i < 10000; i++)
            {
                var ts = new DateTime(rnd.Next(2010, 2021), rnd.Next(1, 12), rnd.Next(1, 28), rnd.Next(0, 23), rnd.Next(0, 59), rnd.Next(0, 59));
                var rec = new DateRecord()
                {
                    Id = Guid.NewGuid().ToString(),
                    Timestamp = ts,
                    Flag = ts.Hour == 20
                };

                Task.Delay(30).Wait();

                var resultTask = container.CreateItemAsync(rec);
                resultTask.Wait();
            }

            Console.WriteLine("Done.");
        }

        public class DateRecord
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "partition")]
            public int Partition { get; set; }

            public DateTime Timestamp { get; set; }

            public bool Flag { get; set; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }
}
