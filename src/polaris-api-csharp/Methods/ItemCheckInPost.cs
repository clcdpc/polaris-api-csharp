
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ItemCheckInResult>> ItemCheckInPostAsync(string itemBarcode, int? logonBranchId = null, int? logonUserId = null, int? logonWorkstationId = null, CancellationToken cancellationToken = default)
        {
            Require.Argument(itemBarcode);
            Require.PositiveIfProvided(logonBranchId);
            Require.PositiveIfProvided(logonUserId);
            Require.PositiveIfProvided(logonWorkstationId);

            var body = new ItemCheckInData(logonBranchId ?? OrganizationId, logonUserId ?? UserId, logonWorkstationId ?? WorkstationId);
            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/item/{EncodeBarcodePathSegment(itemBarcode)}/checkin";
            var request = PapiRestRequest.Post(url, body: body);
            return await ExecutePapiAsync<ItemCheckInResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
