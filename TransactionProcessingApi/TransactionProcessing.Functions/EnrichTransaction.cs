using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TransactionProcessing.Contracts.Data;

namespace TransactionProcessing.Functions
{
    public static class EnrichTransaction
    {
        [FunctionName("EnrichTransaction")]
        public static async Task<IActionResult> Run(
            [QueueTrigger("incomingTransactions")] string queueItem,
            [Queue("enrichedTransactions")] ICollector<string> outputQueueItem,
            ILogger log)
        {
            log.LogInformation($"Received queue item {queueItem}.");

            var transaction = JsonConvert.DeserializeObject<RawTransaction>(queueItem);

            var rnd = new Random();

            var lastTenMinsTranCount = rnd.Next(1, 5);

            var enrichedTransaction = new EnrichedTransaction()
            {
                Amount = transaction.Amount,
                AuthorizationMethod = transaction.AuthorizationMethod,
                InternetLocation = transaction.InternetLocation,
                Purpose = transaction.Purpose,
                Timestamp = transaction.Timestamp,
                TransactionType = transaction.TransactionType,
                LastHourTransactionCount = rnd.Next(lastTenMinsTranCount, 10),
                LastTenMinutesTransactionCount = lastTenMinsTranCount
            };

            outputQueueItem.Add(JsonConvert.SerializeObject(enrichedTransaction));

            return new OkResult();
        }
    }
}
