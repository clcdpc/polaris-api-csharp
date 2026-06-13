using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronRegistrationCreateResult>> PatronRegistrationCreateAsync(PatronRegistrationParams _params, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(_params);
            Require.Positive(_params.LogonBranchID);
            Require.Positive(_params.LogonUserID);
            Require.Positive(_params.LogonWorkstationID);
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
            Require.Positive(_params.LogonBranchID);
            Require.Positive(_params.LogonUserID);
            Require.Positive(_params.LogonWorkstationID);
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