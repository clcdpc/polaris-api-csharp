using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public IRestResponse<PapiResponseCommon> PatronAccountPay(string barcode, int txnId, double txnAmount, PaymentMethod paymentMethod, int? workstationId = null, int? userId = null, string note = "")
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/patron/{barcode}/account/{txnId}/pay?wsid={workstationId ?? WorkstationId}&userid={userId ?? UserId}";
            var body = new PatronAccountPayData { TxnAmount = txnAmount, PaymentMethodId = paymentMethod, FreeTextNote = note };
            var request = new PapiRestRequest(HttpMethod.Put, url) { Body = body };
            return Execute<PapiResponseCommon>(request);
        }
    }
}
