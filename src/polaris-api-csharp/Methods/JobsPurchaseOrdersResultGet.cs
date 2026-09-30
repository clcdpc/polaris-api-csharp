namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<JobsPurchaseOrdersResultGetResult>> JobsPurchaseOrdersResultGetAsync(Guid jobGuid, CancellationToken cancellationToken = default)
        {
            if (jobGuid == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(jobGuid));
            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/jobs/purchaseorders/{jobGuid}/result";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<JobsPurchaseOrdersResultGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
