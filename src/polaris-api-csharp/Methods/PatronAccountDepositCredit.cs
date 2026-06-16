using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronAccountDepositCreditResult>> PatronAccountDepositCreditAsync(string barcode, double txnAmount, int? workstationId = null, int? userId = null, string note = "", CancellationToken cancellationToken = default)
        {
            Require.PositiveIfProvided(workstationId);
            Require.PositiveIfProvided(userId);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/patron/{EncodeBarcodePathSegment(barcode)}/account/lumpsumdepositcredit";
            var body = new PatronAccountDepositCreditData { TxnAmount = txnAmount, FreeTextNote = note };
            var request = PapiRestRequest.Put(url, body: body);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("userid", userId ?? UserId);
            return await ExecutePapiAsync<PatronAccountDepositCreditResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
