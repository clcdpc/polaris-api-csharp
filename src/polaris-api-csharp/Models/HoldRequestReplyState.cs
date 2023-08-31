using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public enum HoldRequestReplyState
    {
        ItemAvailableLocally = 1,
        AcceptILLPolicy,
        AcceptEvenWithExistingHolds,
        NoItemsAttachedStillPlaceHold,
        AcceptLocalHoldPolicyCharge
    }
}
