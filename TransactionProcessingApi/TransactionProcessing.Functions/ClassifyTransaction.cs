using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TransactionProcessing.Contracts.Data;

namespace TransactionProcessing.Functions
{
    public static class ClassifyTransaction
    {
        [FunctionName("ClassifyTransaction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] RawTransaction transaction,
            [Queue("incomingTransactions")] ICollector<string> outputQueueItem,
            ILogger log)
        {
            log.LogInformation($"Received request to {nameof(ClassifyTransaction)}.");

            outputQueueItem.Add(JsonConvert.SerializeObject(transaction));

            return new OkResult();
        }
    }
}
