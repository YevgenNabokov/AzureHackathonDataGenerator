using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessing.Contracts.Data
{
    public class RawTransaction
    {
        public decimal Amount { get; set; }

        public string InternetLocation { get; set; }

        public DateTime Timestamp { get; set; }

        public string Purpose { get; set; }

        public int TransactionType { get; set; }

        public int AuthorizationMethod { get; set; }
    }
}
