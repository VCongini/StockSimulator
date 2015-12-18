using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    public enum CreditTypeEnum
    {
        Unknown = 0,
        Check = 1,
        Cash = 2,
        MoneyOrder = 3,
        Wire = 4
    }

    public enum DebitTypeEnum
    {
        Unknown = 0,
        Check = 1,
        Cash = 2,
        ATM = 3
    }
}
