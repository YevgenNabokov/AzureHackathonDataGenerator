using PopulateTestData.Extensions;
using PopulateTestData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.PatternGenerators
{
    public static class SpendingCategoryExtensions
    {
        public static TransactionRecord ToTransaction(
            this SpendingCategory subject,
            DateTime timestamp,
            Guid sourceAccount,
            Guid targetAccount,
            bool isFraud,
            Random rnd = null)
        {
            rnd = rnd ?? new Random();
            var tranTypes = subject.TransactionOptions.Keys.ToArray();
            var type = tranTypes[rnd.Next(tranTypes.Length - 1)];
            var authMethod = subject.TransactionOptions[type][rnd.Next(subject.TransactionOptions[type].Count - 1)];

            return new TransactionRecord()
            {
                Timestamp = timestamp,
                Amount = rnd.Next(subject.MinAmount, subject.MaxAmount),
                SourceAccountId = sourceAccount,
                TargetAccountId = targetAccount,
                Purpose = subject.Name,
                TransactionType = type,
                AuthorizationMethod = authMethod,
                IpAddress = subject.InternetTransactionIpAddresses != null && subject.InternetTransactionIpAddresses.Count > 0
                ? subject.InternetTransactionIpAddresses[rnd.Next(subject.InternetTransactionIpAddresses.Count - 1)]
                : null,
                IsFraud = isFraud
            };
        }
    }
}
