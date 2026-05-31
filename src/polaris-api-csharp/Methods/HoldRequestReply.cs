using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<HoldRequestReplyResult>> HoldRequestReplyAsync(HoldRequestCreateResult holdCreateResult, int requestingOrgId, HoldRequestReplyAnswer answer, HoldRequestReplyState state, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/holdrequest/{holdCreateResult.RequestGuid}";
            var txnGroupQualifier = holdCreateResult.TxnGroupQualifier;
            var txnQualifier = holdCreateResult.TxnQualifier;

            if (string.IsNullOrEmpty(txnGroupQualifier))
            {
                throw new InvalidOperationException("HoldRequestCreateResult is missing the TxnGroupQualifier required to reply to the hold request.");
            }

            if (string.IsNullOrEmpty(txnQualifier))
            {
                throw new InvalidOperationException("HoldRequestCreateResult is missing the TxnQualifier required to reply to the hold request.");
            }

            var body = new HoldRequestReplyData
            {
                TxnGroupQualifier = txnGroupQualifier,
                TxnQualifier = txnQualifier,
                RequestingOrgID = requestingOrgId,
                Answer = (int)answer,
                State = (int)state
            };

            var request = PapiRestRequest.Put(url, body: body);
            return await ExecutePapiAsync<HoldRequestReplyResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}