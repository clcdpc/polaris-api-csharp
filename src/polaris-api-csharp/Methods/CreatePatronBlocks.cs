namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<CreatePatronBlocksResult>> CreatePatronBlocksAsync(string barcode, BlockType blockType, string blockValue, int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            Require.PositiveIfProvided(userId);
            Require.PositiveIfProvided(workstationId);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/patron/{EncodeBarcodePathSegment(barcode)}/blocks";
            var body = new CreatePatronBlocksRequest((int)blockType, blockValue);
            var request = PapiRestRequest.Post(url, body: body);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("userid", userId ?? UserId);

            return await ExecutePapiAsync<CreatePatronBlocksResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
