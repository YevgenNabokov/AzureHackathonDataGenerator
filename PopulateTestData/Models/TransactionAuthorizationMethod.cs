using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.Models
{
    public enum TransactionAuthorizationMethod
    {
        MobileApp,
        Pin,
        PayPass,
        TwoFactor,
        CvcCode
    }
}
