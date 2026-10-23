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
