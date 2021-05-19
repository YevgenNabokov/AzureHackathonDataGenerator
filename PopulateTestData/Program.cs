using System;

namespace PopulateTestData
{
    class Program
    {
        private static readonly string endpointUri = "https://cosmosdb-transactiondata.documents.azure.com:443/";

        private static string databaseName = "cosmosdb-test-tp";

        static void Main(string[] args)
        {
            Console.WriteLine($"Enter primary key for {endpointUri}:");
            var key = Console.ReadLine();

            ////DatesWriter.WriteDates(endpointUri, key, databaseName);

            TransactionsWriter1.WriteTransactions(endpointUri, key, databaseName);
            TransactionEnricher.EnrichTransactions(endpointUri, key, databaseName);
        }
    }
}
