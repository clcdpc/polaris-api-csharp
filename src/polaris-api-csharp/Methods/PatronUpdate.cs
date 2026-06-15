
using System;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronUpdateResult>> PatronUpdateAsync(string barcode, PatronUpdateParams updateParams, string password = "", bool ignoresa = true, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(updateParams);
            updateParams.LogonBranchId = Require.PositiveIfProvidedOrDefault(updateParams.LogonBranchId, OrganizationId);
            updateParams.LogonUserId = Require.PositiveIfProvidedOrDefault(updateParams.LogonUserId, UserId);
            updateParams.LogonWorkstationId = Require.PositiveIfProvidedOrDefault(updateParams.LogonWorkstationId, WorkstationId);
            Require.PositiveIfProvided(updateParams.RequestPickupBranchID);
            Require.PositiveIfProvided(updateParams.PatronBranchID);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}";
            var request = PapiRestRequest.Put(url, body: updateParams, password: password);
            request.QueryParameters.Add("ignoresa", ignoresa);
            return await ExecutePapiAsync<PatronUpdateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
