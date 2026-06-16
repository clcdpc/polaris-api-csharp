namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<JobsPurchaseOrdersPostResult>> JobsPurchaseOrdersPostAsync(JobsPurchaseOrdersCreateData data, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(data);
            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/jobs/purchaseorders";
            var request = PapiRestRequest.Post(url, body: data);
            return await ExecutePapiAsync<JobsPurchaseOrdersPostResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
