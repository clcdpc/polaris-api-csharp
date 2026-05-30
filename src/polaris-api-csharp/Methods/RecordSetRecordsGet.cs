using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public async Task<IRestResponse<RecordSetRecordsGetResult>> RecordSetRecordsGetAsync(int recordSetId, int userId = 1, int workstationId = 1, int startIndex = 0, int numRecords = 1000, CancellationToken cancellationToken = default)
        {
            await EnsureProtectedTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/recordsets/{recordSetId}/records";
            var request = new PapiRestRequest(url);
            request.QueryParameters.Add("startIndex", startIndex);
            request.QueryParameters.Add("numRecords", numRecords);
            request.QueryParameters.Add("userid", userId);
            request.QueryParameters.Add("wsid", workstationId);
            return await ExecutePapiAsync<RecordSetRecordsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
