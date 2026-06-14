
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ItemRenewResultWrapper>> ItemRenewAsync(string barcode, int itemId, string password = "", ItemRenewOptions? renewOptions = null, CancellationToken cancellationToken = default)
        {
            Require.NonNegative(itemId);

            if (renewOptions == null)
            {
                renewOptions = new ItemRenewOptions(OrganizationId, UserId, WorkstationId);
            }
            else
            {
                Require.Positive(renewOptions.LogonBranchID);
                Require.Positive(renewOptions.LogonUserID);
                Require.Positive(renewOptions.LogonWorkstationID);
            }

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/itemsout/{itemId}";
            var request = PapiRestRequest.Put(url, body: renewOptions, password: password);
            return await ExecutePapiAsync<ItemRenewResultWrapper>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
