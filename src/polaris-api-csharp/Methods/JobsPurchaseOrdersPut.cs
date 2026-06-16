namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<JobsPurchaseOrdersPreorderValidationResult>> JobsPurchaseOrdersPutAsync(JobsPurchaseOrdersPreorderValidationData data, int preOrderValidation = 1, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(data);
            if (preOrderValidation is not 0 and not 1) throw new ArgumentOutOfRangeException(nameof(preOrderValidation));
            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/jobs/purchaseorders";
            var request = PapiRestRequest.Put(url, body: data);
            request.QueryParameters["preordervalidation"] = preOrderValidation;
            return await ExecutePapiAsync<JobsPurchaseOrdersPreorderValidationResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
