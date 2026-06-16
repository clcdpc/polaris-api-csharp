namespace Clc.Polaris.Api
{
    public static class HoldRequestCreateExtensions
    {
        public static Task<IRestResponse<HoldRequestCreateResult>> HoldRequestCreateAsync(
            this IPapiClient client,
            int patronId,
            int bibId,
            int pickupBranchId = 0,
            DateTime? activationDate = null,
            int? userId = null,
            int? workstationId = null,
            int? requestingOrgId = null,
            CancellationToken cancellationToken = default)
        {
            var holdParams = new HoldRequestCreateParams
            {
                PatronID = patronId,
                BibID = bibId,
                ActivationDate = activationDate,
                PickupOrgID = pickupBranchId,
                UserID = userId ?? client.UserId,
                WorkstationID = workstationId ?? client.WorkstationId,
                RequestingOrgID = requestingOrgId ?? client.OrganizationId
            };

            return client.HoldRequestCreateAsync(holdParams, cancellationToken);
        }
    }
}
