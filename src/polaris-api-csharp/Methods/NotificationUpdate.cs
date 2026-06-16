
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<NotificationUpdateResult>> NotificationUpdateAsync(NotificationUpdateParams updateParams, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(updateParams);
            updateParams.LogonBranchId = Require.PositiveIfProvidedOrDefault(updateParams.LogonBranchId, OrganizationId);
            updateParams.LogonUserId = Require.PositiveIfProvidedOrDefault(updateParams.LogonUserId, UserId);
            updateParams.LogonWorkstationId = Require.PositiveIfProvidedOrDefault(updateParams.LogonWorkstationId, WorkstationId);
            updateParams.ReportingOrgID = Require.PositiveIfProvidedOrDefault(updateParams.ReportingOrgID, OrganizationId);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/notification/{updateParams.NotificationTypeId}";
            var request = PapiRestRequest.Put(url, body: updateParams);
            return await ExecutePapiAsync<NotificationUpdateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}