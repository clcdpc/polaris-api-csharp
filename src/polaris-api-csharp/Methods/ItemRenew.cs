using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ItemRenewResultWrapper>> ItemRenewAsync(string barcode, int itemId, string password = "", ItemRenewOptions? renewOptions = null, CancellationToken cancellationToken = default)
        {
            Require.NonNegative(itemId);

            renewOptions ??= new ItemRenewOptions();
            renewOptions.LogonBranchID = Require.PositiveIfProvidedOrDefault(renewOptions.LogonBranchID, OrganizationId);
            renewOptions.LogonUserID = Require.PositiveIfProvidedOrDefault(renewOptions.LogonUserID, UserId);
            renewOptions.LogonWorkstationID = Require.PositiveIfProvidedOrDefault(renewOptions.LogonWorkstationID, WorkstationId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/itemsout/{itemId}";
            var request = PapiRestRequest.Put(url, body: renewOptions, password: password);
            return await ExecutePapiAsync<ItemRenewResultWrapper>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
