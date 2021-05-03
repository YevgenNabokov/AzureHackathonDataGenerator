using PopulateTestData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.PatternGenerators
{
    public interface IPatternGenerator
    {
        public IEnumerable<TransactionRecord> Generate(GeneratorContext context);
    }
}
