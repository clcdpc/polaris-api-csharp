using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<RecordSetRecordsGetResult>> RecordSetRecordsGetAsync(int recordSetId, int userId = 1, int workstationId = 1, int startIndex = 0, int numRecords = 1000, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/recordsets/{recordSetId}/records";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("startIndex", startIndex);
            request.QueryParameters.Add("numRecords", numRecords);
            request.QueryParameters.Add("userid", userId);
            request.QueryParameters.Add("wsid", workstationId);
            return await ExecutePapiAsync<RecordSetRecordsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
