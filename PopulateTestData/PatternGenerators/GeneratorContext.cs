using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.PatternGenerators
{
    public class GeneratorContext
    {
        private HashSet<Guid> legitCustomerAccounts = new HashSet<Guid>();

        private HashSet<Guid> legitMerchantAccounts = new HashSet<Guid>();

        private HashSet<Guid> fraudAccounts = new HashSet<Guid>();

        private Dictionary<Guid, IpAddressMask> legitAccountUsageIpAddressMasks = new Dictionary<Guid, IpAddressMask>();

        private Dictionary<Guid, IpAddressMask> fraudAccountUsageIpAddressMasks = new Dictionary<Guid, IpAddressMask>();

        public void Initialize(int initialNumberOfLegitMerchantAccounts)
        {
            for (var i = 0; i < initialNumberOfLegitMerchantAccounts; i++)
            {
                this.legitMerchantAccounts.Add(Guid.NewGuid());
            }
        }

        public Guid GetUnusedLegitCustomerAccount()
        {
            var result = Guid.NewGuid();
            this.legitCustomerAccounts.Add(result);
            return result;
        }

        public string GetLegitIpAddressForAccountUsage(Guid account)
        {
            var rnd = new Random();
            if (!this.legitAccountUsageIpAddressMasks.ContainsKey(account))
            {
                this.legitAccountUsageIpAddressMasks.Add(account, new IpAddressMask(rnd.Next(0, 255), rnd.Next(0, 255), null, null));
            }

            return this.legitAccountUsageIpAddressMasks[account].GetRandomIpAddressString();
        }

        public string GetFraudSourceIpAddress()
        {
            var rnd = new Random();
            return new IpAddressMask(rnd.Next(0, 255), rnd.Next(0, 255), null, null).GetRandomIpAddressString();
        }

        public Guid GetLegitMerchantAccount()
        {
            if (this.legitMerchantAccounts.Count < 1)
            {
                return this.GetLegitUnusedMerchantAccount();
            }

            return this.legitMerchantAccounts.ToArray()[new Random().Next(0, this.legitMerchantAccounts.Count - 1)];
        }

        public Guid GetLegitUnusedMerchantAccount()
        {
            var result = Guid.NewGuid();
            this.legitMerchantAccounts.Add(result);
            return result;
        }

        public Guid GetUnusedFraudAccount()
        {
            var result = Guid.NewGuid();
            this.fraudAccounts.Add(result);
            return result;
        }
    }
}
