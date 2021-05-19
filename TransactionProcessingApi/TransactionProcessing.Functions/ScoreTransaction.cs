using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TransactionProcessing.Contracts.Data;

namespace TransactionProcessing.Functions
{
    public static class ScoreTransaction
    {
        public static string SCORING_SERVICE_URL = nameof(SCORING_SERVICE_URL);

        [FunctionName("ScoreTransaction")]
        public static async void Run(
            [QueueTrigger("enriched-transactions", Connection = "AzureWebJobsStorage")] string queueItem,
            [Queue("scored-transactions", Connection = "AzureWebJobsStorage")] ICollector<string> outputQueueItem,
            ILogger log)
        {
            log.LogInformation($"Received queue item {queueItem}.");

            var transaction = JsonConvert.DeserializeObject<EnrichedTransaction>(queueItem);

            var handler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
            };

            var classificationResult = new ClassificationResult()
            {
                Transaction = transaction
            };

            var input = new ScoringInput()
            {
                Amount = transaction.Amount,
                AuthorizationMethod = transaction.AuthorizationMethod,
                InternetLocation = transaction.InternetLocation,
                LastHourTransactionCount = transaction.LastHourTransactionCount,
                LastTenMinutesTransactionCount = transaction.LastTenMinutesTransactionCount,
                Purpose = transaction.Purpose,
                Timestamp = transaction.Timestamp,
                TransactionType = transaction.TransactionType,
                IsFraud = null
            };

            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(GetEnvironmentVar(SCORING_SERVICE_URL));
                var content = new StringContent(JsonConvert.SerializeObject(input));

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("", content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Result: {0}", result);

                    var scoringResult = JsonConvert.DeserializeObject<ScoringOutput>(result);
                    if (scoringResult.result == null || scoringResult.result.Length < 1)
                    {
                        throw new InvalidOperationException("Scoring result was null or contained no items.");
                    }

                    if (scoringResult.result[0].Length < 3)
                    {
                        throw new InvalidOperationException($"Scoring result did not contain expected number of fields, field count: {scoringResult.result[0].Length}");
                    }

                    bool isFraud;
                    if (!bool.TryParse(scoringResult.result[0][0]?.ToLower(), out isFraud))
                    {
                        throw new InvalidOperationException($"Unable to parse IsFraud indicator, value: {scoringResult.result[0][0]}");
                    }

                    double rate = 0;
                    if (!double.TryParse(scoringResult.result[0][1], out rate))
                    {
                        throw new InvalidOperationException($"Unable to parse Rate, value: {scoringResult.result[0][1]}");
                    }

                    classificationResult.IsFraud = isFraud;
                    classificationResult.Rate = rate;
                }
                else
                {
                    string responseContent = string.Empty;

                    try
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                    }
                    catch
                    {
                    }

                    log.LogError($"Scoring failed with status code: {response.StatusCode}, Headers: {response.Headers}, Content: {responseContent}");
                }
            }

            outputQueueItem.Add(JsonConvert.SerializeObject(classificationResult));
        }

        private static string GetEnvironmentVar(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
