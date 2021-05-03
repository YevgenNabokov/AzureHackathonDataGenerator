using PopulateTestData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.PatternGenerators.Combined
{
    public class AccountHistoryMaker : IPatternGenerator
    {
        private readonly DateTime from;
        private readonly DateTime to;
        private readonly bool buysGrocery;
        private readonly bool buysElectronics;
        private readonly bool hasFraud;
        private readonly Guid? account;

        public AccountHistoryMaker(
            DateTime from,
            DateTime to,
            bool buysGrocery = true,
            bool buysElectronics = true,
            bool hasFraud = false,
            Guid? account = null)
        {
            this.from = from;
            this.to = to;
            this.buysGrocery = buysGrocery;
            this.buysElectronics = buysElectronics;
            this.hasFraud = hasFraud;
            this.account = account;
        }

        public IEnumerable<TransactionRecord> Generate(GeneratorContext context)
        {
            var rnd = new Random();
            var account = this.account ?? Guid.NewGuid();

            var legitHistoryGenerator = new LegitMainAccountHistory(
                account,
                Guid.NewGuid(),
                this.from,
                this.to,
                rnd.Next(2000, 20000),
                5,
                20,
                this.GetSpendingProfile(rnd, context, account));

            List<TransactionRecord> result = new List<TransactionRecord>();
            result.AddRange(legitHistoryGenerator.Generate(context));

            if (this.hasFraud)
            {
                var fraudStart = this.from.AddSeconds(rnd.Next(Convert.ToInt32((this.to - this.from).TotalSeconds)));

                var fraudGenerator = new FraudPatternSimple(
                account,
                fraudStart,
                rnd.Next(5, 15),
                TimeSpan.FromMinutes(2),
                TimeSpan.FromMinutes(10),
                new SpendingProfile()
                {
                    SpendingCategories = new List<SpendingCategory>()
                    {
                        new SpendingCategory(
                            "Electronics",
                            new DateTime(2020, 1, 1, 0, 0, 0),
                            new DateTime(2020, 1, 1, 23, 0, 0),
                            1,
                            1,
                            800,
                            1500,
                            new Dictionary<TransactionType, List<TransactionAuthorizationMethod>>() { { TransactionType.Internet, new List<TransactionAuthorizationMethod> { TransactionAuthorizationMethod.TwoFactor } } },
                            true,
                            false,
                            new List<string> { context.GetFraudSourceIpAddress() },
                            null,
                            null)
                    }
                });

                result.AddRange(fraudGenerator.Generate(context));
            }

            return result.OrderBy(t => t.Timestamp);
        }

        private SpendingProfile GetSpendingProfile(Random rnd, GeneratorContext context, Guid account)
        {
            var result = new SpendingProfile() { SpendingCategories = new List<SpendingCategory>() };

            this.AddGrocerySpendingCategory(result, rnd);
            this.AddElectronicsSpendingCategory(result, context, account);

            return result;
        }

        private void AddGrocerySpendingCategory(SpendingProfile profile, Random rnd)
        {
            if (this.buysGrocery)
            {
                profile.SpendingCategories.Add(
                    new SpendingCategory(
                            "Grocery",
                            new DateTime(2020, 1, 1, 18, 0, 0),
                            new DateTime(2020, 1, 1, 21, 0, 0),
                            2,
                            16,
                            50,
                            200,
                            new Dictionary<TransactionType, List<TransactionAuthorizationMethod>>() { { TransactionType.Retail, new List<TransactionAuthorizationMethod> { TransactionAuthorizationMethod.Pin, TransactionAuthorizationMethod.PayPass } } },
                            true,
                            false,
                            null,
                            null,
                            null));
            }
        }

        private void AddElectronicsSpendingCategory(SpendingProfile profile, GeneratorContext context, Guid account)
        {
            if (this.buysElectronics)
            {
                profile.SpendingCategories.Add(
                    new SpendingCategory(
                            "Electronics",
                            new DateTime(2020, 1, 1, 07, 0, 0),
                            new DateTime(2020, 1, 1, 23, 30, 0),
                            0,
                            2,
                            50,
                            1000,
                            new Dictionary<TransactionType, List<TransactionAuthorizationMethod>>() 
                            {
                                { TransactionType.Retail, new [] { TransactionAuthorizationMethod.Pin, TransactionAuthorizationMethod.PayPass }.ToList() },
                                { TransactionType.Internet, new [] { TransactionAuthorizationMethod.MobileApp, TransactionAuthorizationMethod.CvcCode }.ToList() },
                            },
                            true,
                            false,
                            new List<string> { context.GetLegitIpAddressForAccountUsage(account) },
                            null,
                            null));
            }
        }
    }
}
