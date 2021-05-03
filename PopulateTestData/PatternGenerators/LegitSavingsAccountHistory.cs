using PopulateTestData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.PatternGenerators
{
    public class LegitSavingsAccountHistory : IPatternGenerator
    {
        private readonly Guid account;
        private readonly DateTime fromDate;
        private readonly DateTime toDate;

        public LegitSavingsAccountHistory(Guid account, Guid sourceAccount, DateTime fromDate, DateTime toDate, decimal maxOneTimeAmount)
        {
            this.account = account;
            this.fromDate = fromDate;
            this.toDate = toDate;
        }

        public IEnumerable<TransactionRecord> Generate(GeneratorContext context)
        {
            return Enumerable.Empty<TransactionRecord>();
        }
    }
}
