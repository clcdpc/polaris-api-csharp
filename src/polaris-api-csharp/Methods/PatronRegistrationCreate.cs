
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronRegistrationCreateResult>> PatronRegistrationCreateAsync(PatronRegistrationParams _params, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(_params);
            _params.LogonBranchID = Require.PositiveIfProvidedOrDefault(_params.LogonBranchID, OrganizationId);
            _params.LogonUserID = Require.PositiveIfProvidedOrDefault(_params.LogonUserID, UserId);
            _params.LogonWorkstationID = Require.PositiveIfProvidedOrDefault(_params.LogonWorkstationID, WorkstationId);
            Require.Positive(_params.PatronBranchID);
            Require.Argument(_params.NameFirst);
            Require.Argument(_params.NameLast);
            Require.PositiveIfProvided(_params.RequestPickupBranchID);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron";
            var request = PapiRestRequest.Post(url, body: _params);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<PatronRegistrationCreateResult>(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IRestResponse<PatronRegistrationCreateResult>> PatronRegistrationCreateV2Async(PatronRegistrationData _params, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(_params);
            _params.LogonBranchID = Require.PositiveIfProvidedOrDefault(_params.LogonBranchID, OrganizationId);
            _params.LogonUserID = Require.PositiveIfProvidedOrDefault(_params.LogonUserID, UserId);
            _params.LogonWorkstationID = Require.PositiveIfProvidedOrDefault(_params.LogonWorkstationID, WorkstationId);
            Require.Positive(_params.PatronBranchID);
            Require.Argument(_params.NameFirst);
            Require.Argument(_params.NameLast);
            Require.PositiveIfProvided(_params.RequestPickupBranchID);

            var url = $"/public/v2/1033/100/{OrganizationId}/patron";
            var request = PapiRestRequest.Post(url, body: _params);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<PatronRegistrationCreateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}