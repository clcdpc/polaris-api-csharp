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
        

        public IRestResponse<PapiResponseCommon> PatronAccountPayAll(string barcode, double txnAmount, PaymentMethod paymentMethod, int? workstationId = 1, int? userId = 1, string note = "")
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/patron/{barcode}/account/lumpsumpayment?wsid={workstationId ?? WorkstationId}&userid={userId ?? UserId}";
            var body = new PatronAccountPayData { TxnAmount = txnAmount, PaymentMethodId = paymentMethod, FreeTextNote = note };
            var request = new PapiRestRequest(HttpMethod.Put, url) { Body = body };
            return Execute<PapiResponseCommon>(request);
        }
    }
}
