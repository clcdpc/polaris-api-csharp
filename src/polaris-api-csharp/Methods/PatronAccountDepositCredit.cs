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
        

        public IRestResponse<PapiResponseCommon> PatronAccountDepositCredit(string barcode, double txnAmount, int? workstationId = null, int? userId = null, string note = "")
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/patron/{barcode}/account/lumpsumdepositcredit?wsid={workstationId ?? WorkstationId}&userid={userId ?? UserId}";
            var body = new PatronAccountDepositCreditData { TxnAmount = txnAmount, FreeTextNote = note };
            var request = new PapiRestRequest(HttpMethod.Put, url) { Body = body };
            return Execute<PapiResponseCommon>(request);
        }
    }
}
