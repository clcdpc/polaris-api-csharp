using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public enum HoldStatus
    {
        Inactive = 1,
        Active = 3,
        Pending = 4,
        Shipped = 5,
        Held = 6,
        NotSupplied = 7,
        Unclaimed = 8,
        Expired = 9,
        Cancelled = 16,
        Out = 17,
        Located = 18
    }
}
