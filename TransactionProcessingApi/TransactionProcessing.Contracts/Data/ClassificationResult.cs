using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessing.Contracts.Data
{
    public class ClassificationResult
    {
        public EnrichedTransaction Transaction { get; set; }

        public bool IsFraud { get; set; }

        public double Rate { get; set; }
    }
}
