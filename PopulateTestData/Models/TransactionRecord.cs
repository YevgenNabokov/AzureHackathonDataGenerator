using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.Models
{
    public class TransactionRecord
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "partition")]
        public int Partition { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid SourceAccountId { get; set; }

        public Guid TargetAccountId { get; set; }

        public decimal Amount { get; set; }

        public string IpAddress { get; set; }

        public string Purpose { get; set; }

        public TransactionType TransactionType { get; set; }

        public TransactionAuthorizationMethod AuthorizationMethod { get; set; }

        public bool IsFraud { get; set; }
    }
}
