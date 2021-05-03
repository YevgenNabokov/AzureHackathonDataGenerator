using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.Models
{
    public class TransactionRecordWithFeatures : TransactionRecord
    {
        public string InternetLocation { get; set; }

        public int LastTenMinutesTransactionCount { get; set; }

        public int LastHourTransactionCount { get; set; }
    }
}
