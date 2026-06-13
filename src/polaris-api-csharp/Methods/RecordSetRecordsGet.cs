using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<RecordSetRecordsGetResult>> RecordSetRecordsGetAsync(int recordSetId, int? userId = null, int? workstationId = null, int startIndex = 0, int numRecords = 1000, CancellationToken cancellationToken = default)
        {
            Require.Positive(recordSetId);
            Require.PositiveIfProvided(userId);
            Require.PositiveIfProvided(workstationId);
            Require.NonNegative(startIndex);
            Require.Positive(numRecords);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/recordsets/{recordSetId}/records";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("startIndex", startIndex);
            request.QueryParameters.Add("numRecords", numRecords);
            request.QueryParameters.Add("userid", userId ?? UserId);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            return await ExecutePapiAsync<RecordSetRecordsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
