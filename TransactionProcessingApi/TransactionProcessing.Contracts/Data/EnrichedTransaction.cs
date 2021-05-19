using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessing.Contracts.Data
{
    public class EnrichedTransaction : RawTransaction
    {
        public int LastTenMinutesTransactionCount { get; set; }

        public int LastHourTransactionCount { get; set; }
    }
}
