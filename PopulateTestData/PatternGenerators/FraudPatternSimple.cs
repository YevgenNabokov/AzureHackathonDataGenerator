using PopulateTestData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.PatternGenerators
{
    public class FraudPatternSimple : IPatternGenerator
    {
        private readonly Guid account;
        private readonly DateTime start;
        private readonly int transactionCount;
        private readonly TimeSpan minPeriod;
        private readonly TimeSpan maxPeriod;
        private readonly SpendingProfile profile;

        public FraudPatternSimple(Guid account, DateTime start, int transactionCount, TimeSpan minPeriod, TimeSpan maxPeriod, SpendingProfile profile)
        {
            this.account = account;
            this.start = start;
            this.transactionCount = transactionCount;
            this.minPeriod = minPeriod;
            this.maxPeriod = maxPeriod;
            this.profile = profile;
        }

        public IEnumerable<TransactionRecord> Generate(GeneratorContext context)
        {
            var rnd = new Random();
            List<TransactionRecord> result = new List<TransactionRecord>();
            var timestamp = start;

            for (var i = 0; i < this.transactionCount; i++)
            {
                var category = this.profile.SpendingCategories[rnd.Next(this.profile.SpendingCategories.Count - 1)];
                var merchant = context.GetLegitMerchantAccount();
                result.Add(category.ToTransaction(timestamp, account, merchant, true, rnd));

                timestamp = timestamp.AddSeconds(rnd.Next(Convert.ToInt32(this.minPeriod.TotalSeconds), Convert.ToInt32(this.maxPeriod.TotalSeconds)));
            }

            return result;
        }
    }
}
