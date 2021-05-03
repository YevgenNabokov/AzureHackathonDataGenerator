using PopulateTestData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.PatternGenerators
{
    public class SpendingCategory
    {
        public SpendingCategory(
            string name,
            DateTime minUsualTimeOfDay,
            DateTime maxUsualTimeOfDay,
            int minTimesPerMonth,
            int maxTimesPerMonth,
            int minAmount,
            int maxAmount,
            Dictionary<TransactionType, List<TransactionAuthorizationMethod>> transactionOptions,
            bool merchantReceiverAccount,
            bool sameReceiverAccount,
            List<string> internetTransactionIpAddresses,
            int? minUsualDayOfMonth,
            int? maxUsualDayOfMonth)
        {
            MinUsualTimeOfDay = minUsualTimeOfDay;
            MaxUsualTimeOfDay = maxUsualTimeOfDay;
            Name = name;
            MinTimesPerMonth = minTimesPerMonth;
            MaxTimesPerMonth = maxTimesPerMonth;
            MinAmount = minAmount;
            MaxAmount = maxAmount;
            TransactionOptions = transactionOptions;
            MerchantReceiverAccount = merchantReceiverAccount;
            SameReceiverAccount = sameReceiverAccount;
            InternetTransactionIpAddresses = internetTransactionIpAddresses;
            MinUsualDayOfMonth = minUsualDayOfMonth;
            MaxUsualDayOfMonth = maxUsualDayOfMonth;
        }

        public List<string> InternetTransactionIpAddresses { get; set; }

        public Dictionary<TransactionType, List<TransactionAuthorizationMethod>> TransactionOptions { get; set; }

        public DateTime MinUsualTimeOfDay { get; set; }

        public DateTime MaxUsualTimeOfDay { get; set; }

        public int? MinUsualDayOfMonth { get; set; }

        public int? MaxUsualDayOfMonth { get; set; }

        public string Name { get; set; }

        public int MinTimesPerMonth { get; set; }

        public int MaxTimesPerMonth { get; set; }

        public int MinAmount { get; set; }

        public int MaxAmount { get; set; }

        public bool MerchantReceiverAccount { get; set; }

        public bool SameReceiverAccount { get; set; }
    }
}
