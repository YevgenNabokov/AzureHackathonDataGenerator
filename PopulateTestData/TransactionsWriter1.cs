using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using PopulateTestData.Models;
using PopulateTestData.PatternGenerators;
using PopulateTestData.PatternGenerators.Combined;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData
{
    public class TransactionsWriter1
    {
        public static async void WriteTransactions(string endpoint, string primaryKey, string databaseName, string containerName = "test-transactions1")
        {
            var cosmosClient = new CosmosClient(endpoint, primaryKey);

            var container = cosmosClient.GetContainer(databaseName, containerName);

            var rnd = new Random();

            Console.WriteLine("Creating records...");

            var context = new GeneratorContext();
            context.Initialize(100);

            var timePeriodStart = new DateTime(2020, 01, 01);
            var timePeriodEnd = new DateTime(2021, 01, 01);

            List<TransactionRecord> transactions = new List<TransactionRecord>();

            for (var i = 0; i < 50; i++)
            {
                var historyMaker = new AccountHistoryMaker(timePeriodStart, timePeriodEnd, true, rnd.Next(0, 1) == 1, true);
                transactions.AddRange(historyMaker.Generate(context));
            }

            var c = 0;
            foreach (var transaction in transactions)
            {
                transaction.Id = Guid.NewGuid().ToString();
                transaction.Partition = 0;

                Task.Delay(10).Wait();

                var resultTask = container.CreateItemAsync(transaction);
                resultTask.Wait();
                c++;
                Console.Write($"{Math.Round((Convert.ToSingle(c) / transactions.Count), 4) * 100}%          ");
                Console.SetCursorPosition(0, Console.CursorTop);
            }

            Console.WriteLine("Done.");
        }
    }
}
