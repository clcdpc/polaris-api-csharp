using System;
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
        public IRestResponse<PapiResponseCommon> ItemUpdateBarcode(string newBarcode, int? itemRecordId = null, int? transactionBranchId = null, string oldBarcode = "")
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/cataloging/items/{(itemRecordId.HasValue ? itemRecordId.Value.ToString() : oldBarcode)}/barcode?wsid={transactionBranchId ?? WorkstationId}";
            if (!string.IsNullOrWhiteSpace(oldBarcode)) { url += "&isBarcode=1"; }
            var body = new ItemUpdateBarcodeData { ItemBarcode = newBarcode, TransactionBranchId = transactionBranchId ?? OrganizationId };
            var request = new PapiRestRequest(HttpMethod.Put, url) { Body = body };
            return Execute<PapiResponseCommon>(request);
        }

        
        
    }
}