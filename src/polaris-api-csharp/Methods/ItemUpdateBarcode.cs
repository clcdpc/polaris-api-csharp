using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PapiResponseCommon>> ItemUpdateBarcodeAsync(string newBarcode, int? itemRecordId = null, int? transactionBranchId = null, string oldBarcode = "", CancellationToken cancellationToken = default)
        {
            await EnsureProtectedTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/cataloging/items/{(itemRecordId.HasValue ? itemRecordId.Value.ToString() : oldBarcode)}/barcode";
            var body = new ItemUpdateBarcodeData { ItemBarcode = newBarcode, TransactionBranchId = transactionBranchId ?? OrganizationId };
            var request = new PapiRestRequest(HttpMethod.Put, url) { Body = body };
            request.QueryParameters.Add("wsid", transactionBranchId ?? WorkstationId);
            if (!string.IsNullOrWhiteSpace(oldBarcode)) { request.QueryParameters.Add("isBarcode", 1); }
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }



    }
}