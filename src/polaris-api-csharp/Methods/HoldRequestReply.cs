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
        public async Task<IRestResponse<HoldRequestReplyResult>> HoldRequestReplyAsync(HoldRequestCreateResult holdCreateResult, int requestingOrgId, HoldRequestReplyAnswer answer, HoldRequestReplyState state, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/holdrequest/{holdCreateResult.RequestGuid}";
            var body = new HoldRequestReplyData
            {
                TxnGroupQualifier = holdCreateResult.TxnGroupQualifier,
                TxnQualifier = holdCreateResult.TxnQualifier,
                RequestingOrgID = requestingOrgId,
                Answer = (int)answer,
                State = (int)state
            };

            var request = new PapiRestRequest(HttpMethod.Put, url) { Body = body };
            return await ExecutePapiAsync<HoldRequestReplyResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}