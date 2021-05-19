using Microsoft.Azure.Cosmos;
using PopulateTestData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData
{
    public class TransactionEnricher
    {
        public static async void EnrichTransactions(
            string endpoint,
            string primaryKey,
            string databaseName,
            string sourceContainerName = "test-transactions1",
            string targetContainerName = "test-transactions1processed")
        {
            Console.WriteLine("Enriching transactions...");

            var cosmosClient = new CosmosClient(endpoint, primaryKey);
            var sourceContainer = cosmosClient.GetContainer(databaseName, sourceContainerName);
            var targetContainer = cosmosClient.GetContainer(databaseName, targetContainerName);

            var transactions = sourceContainer.GetItemLinqQueryable<TransactionRecord>(allowSynchronousQueryExecution: true).ToList();

            var properties = typeof(TransactionRecord).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            transactions = transactions.OrderBy(t => t.Timestamp).ToList();

            Dictionary<Guid, Queue<DateTime>> lastTenMinutesTransactionTimes = new Dictionary<Guid, Queue<DateTime>>();
            Dictionary<Guid, Queue<DateTime>> lastHourTransactionTimes = new Dictionary<Guid, Queue<DateTime>>();

            var t = 0;
            foreach (var transaction in transactions)
            {
                var internetLocation = string.IsNullOrEmpty(transaction.IpAddress) ? null : string.Join(".", transaction.IpAddress.Split('.').Take(2));

                var enrichedTransaction = new TransactionRecordWithFeatures();

                foreach (var prop in properties)
                {
                    prop.SetValue(enrichedTransaction, prop.GetValue(transaction));
                }

                enrichedTransaction.InternetLocation = internetLocation;
                enrichedTransaction.LastTenMinutesTransactionCount = UpdateLastNMinutesTransactionCount(transaction, lastTenMinutesTransactionTimes, 10);
                enrichedTransaction.LastHourTransactionCount = UpdateLastNMinutesTransactionCount(transaction, lastHourTransactionTimes, 60);

                Task.Delay(10).Wait();

                var resultTask = targetContainer.CreateItemAsync(enrichedTransaction);
                resultTask.Wait();
                t++;

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"{Math.Round((Convert.ToSingle(t) / transactions.Count), 4) * 100}%          ");
            }

            Console.WriteLine("Done.");
        }

        private static int UpdateLastNMinutesTransactionCount(TransactionRecord transaction, Dictionary<Guid, Queue<DateTime>> transactionTimes, int minutes)
        {
            var cutOffTime = transaction.Timestamp.AddMinutes(-10);

            if (transactionTimes.ContainsKey(transaction.SourceAccountId))
            {
                var queue = transactionTimes[transaction.SourceAccountId];
                while (queue.Count > 0 && queue.Peek() < cutOffTime)
                {
                    queue.Dequeue();
                }

                queue.Enqueue(transaction.Timestamp);
                return queue.Count;
            }
            else
            {
                transactionTimes.Add(transaction.SourceAccountId, new Queue<DateTime>(new[] { transaction.Timestamp }));
                return 1;
            }
        }
    }
}
