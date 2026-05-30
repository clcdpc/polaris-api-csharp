using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PapiResponseCommon>> PatronAccountCreateCreditAsync(string barcode, double txnAmount, PaymentMethod paymentMethod, int? workstationId = null, int? userId = null, string note = "", CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/patron/{WebUtility.UrlEncode(barcode)}/account/createcredit";
            var body = new PatronAccountCreateCreditData { TxnAmount = txnAmount, PaymentMethodId = paymentMethod, FreeTextNote = note };
            var request = new PapiRestRequest(HttpMethod.Put, url) { Body = body };
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("userid", userId ?? UserId);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
