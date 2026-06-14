using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ItemCheckOutResult>> ItemCheckOutPostAsync(string patronBarcode, string itemBarcode, string password = "", int? logonBranchId = null, int? logonUserId = null, int? logonWorkstationId = null, CancellationToken cancellationToken = default)
        {
            Require.Argument(patronBarcode);
            Require.Argument(itemBarcode);
            Require.PositiveIfProvided(logonBranchId);
            Require.PositiveIfProvided(logonUserId);
            Require.PositiveIfProvided(logonWorkstationId);

            var body = new ItemCheckOutData(
                itemBarcode,
                logonBranchId ?? OrganizationId,
                logonUserId ?? UserId,
                logonWorkstationId ?? WorkstationId);
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(patronBarcode)}/itemsout";
            var request = PapiRestRequest.Post(url, body: body, password: password);
            return await ExecutePapiAsync<ItemCheckOutResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
