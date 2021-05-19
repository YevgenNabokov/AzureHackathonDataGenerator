using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessing.Contracts.Data
{
    public class ScoringInput
    {
        public decimal Amount { get; set; }

        public string InternetLocation { get; set; }

        public DateTime Timestamp { get; set; }

        public string Purpose { get; set; }

        public int TransactionType { get; set; }

        public int AuthorizationMethod { get; set; }

        public int LastTenMinutesTransactionCount { get; set; }

        public int LastHourTransactionCount { get; set; }

        public bool? IsFraud { get; set; }
    }
}
