namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<JobsPurchaseOrdersStatusGetResult>> JobsPurchaseOrdersStatusGetAsync(Guid jobGuid, CancellationToken cancellationToken = default)
        {
            if (jobGuid == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(jobGuid));
            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/jobs/purchaseorders/{jobGuid}/status";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<JobsPurchaseOrdersStatusGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
