using PopulateTestData.Extensions;
using PopulateTestData.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.PatternGenerators
{
    public class LegitMainAccountHistory : IPatternGenerator
    {
        private const string monthNameFormat = "MMMM";

        private readonly Guid account;
        private readonly Guid employerAccount;
        private readonly DateTime fromDate;
        private readonly DateTime toDate;
        private readonly decimal incomeAmount;
        private readonly int maxIncomeDeviationPercentage;
        private readonly decimal savingsPercentage;
        private readonly SpendingProfile profile;

        public LegitMainAccountHistory(Guid account, Guid employerAccount, DateTime fromDate, DateTime toDate, decimal incomeAmount, int maxIncomeDeviationPercentage, decimal savingsPercentage, SpendingProfile profile)
        {
            this.account = account;
            this.employerAccount = employerAccount;
            this.fromDate = fromDate;
            this.toDate = toDate;
            this.incomeAmount = incomeAmount;
            this.maxIncomeDeviationPercentage = maxIncomeDeviationPercentage;
            this.savingsPercentage = savingsPercentage;
            this.profile = profile;
        }

        public IEnumerable<TransactionRecord> Generate(GeneratorContext context)
        {
            Dictionary<SpendingCategory, Guid> fixedMerchantAccounts = new Dictionary<SpendingCategory, Guid>();

            foreach (var category in this.profile.SpendingCategories)
            {
                if (category.SameReceiverAccount)
                {
                    fixedMerchantAccounts.Add(category, context.GetLegitMerchantAccount());
                }
            }

            var monthsDiff = ((this.toDate.Year - this.fromDate.Year) * 12) + this.toDate.Month - this.fromDate.Month;

            if (this.fromDate.Year == this.toDate.Year && this.fromDate.Month == this.toDate.Month)
            {
                return GenerateMonth(context, this.fromDate, this.toDate, fixedMerchantAccounts);
            }
            else
            {
                List<TransactionRecord> result = new List<TransactionRecord>();

                result.AddRange(this.GenerateMonth(context, this.fromDate, new DateTime(this.fromDate.Year, this.fromDate.Month, DateTime.DaysInMonth(this.fromDate.Year, this.fromDate.Month)), fixedMerchantAccounts));

                for (int m = 1; m < monthsDiff; m++)
                {
                    var date = this.fromDate.AddMonths(m);
                    result.AddRange(this.GenerateMonth(context, new DateTime(date.Year, date.Month, 1), new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)), fixedMerchantAccounts));
                }

                result.AddRange(this.GenerateMonth(context, new DateTime(this.toDate.Year, this.toDate.Month, 1), this.toDate, fixedMerchantAccounts));

                return result;
            }
        }

        private IEnumerable<TransactionRecord> GenerateMonth(GeneratorContext context, DateTime from, DateTime to, Dictionary<SpendingCategory, Guid> fixedMerchantAccounts)
        {
            var rnd = new Random();

            var result = new List<TransactionRecord>();

            var eom = to.EndOfMonth();

            if (to >= eom)
            {
                result.Add(new TransactionRecord()
                {
                    Timestamp = eom.AddDays(-rnd.Next(0, 5)).WithTime(18, 0, 0),
                    Amount = this.incomeAmount + (rnd.Next(0, this.maxIncomeDeviationPercentage) * this.incomeAmount) / 100,
                    SourceAccountId = this.employerAccount,
                    TargetAccountId = this.account,
                    Purpose = $"Remuneration for {to.ToString(monthNameFormat, CultureInfo.InvariantCulture)} {to.Year}",
                    TransactionType = TransactionType.WireTransfer,
                    AuthorizationMethod = TransactionAuthorizationMethod.MobileApp,
                    IpAddress = context.GetLegitIpAddressForAccountUsage(this.employerAccount),
                    IsFraud = false
                });
            }

            foreach (var category in this.profile.SpendingCategories)
            {
                if ((!category.MinUsualDayOfMonth.HasValue || category.MinUsualDayOfMonth <= to.Day)
                    && (!category.MaxUsualDayOfMonth.HasValue || category.MaxUsualDayOfMonth >= from.Day))
                {
                    var numberOfTransactions = rnd.Next(category.MinTimesPerMonth, category.MaxTimesPerMonth);
                    for (var i = 0; i < numberOfTransactions; i++)
                    {
                        var timestamp = new DateTime(
                                from.Year,
                                from.Month,
                                rnd.Next(category.MinUsualDayOfMonth.HasValue ? category.MinUsualDayOfMonth.Value : 1,
                                    category.MaxUsualDayOfMonth.HasValue ? category.MaxUsualDayOfMonth.Value : to.Day))
                            .WithTime(category.MinUsualTimeOfDay.AddSeconds(rnd.Next((int)(category.MaxUsualTimeOfDay - category.MinUsualTimeOfDay).TotalSeconds)));

                        var merchantAccount = fixedMerchantAccounts.ContainsKey(category)
                            ? fixedMerchantAccounts[category]
                            : context.GetLegitMerchantAccount();

                        result.Add(category.ToTransaction(
                            timestamp,
                            this.account,
                            merchantAccount,
                            false,
                            rnd));
                    }
                }
            }

            return result.OrderBy(t => t.Timestamp);
        }
    }
}
