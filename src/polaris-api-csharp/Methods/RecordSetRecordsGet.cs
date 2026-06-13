using Clc.Rest;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<RecordSetRecordsGetResult>> RecordSetRecordsGetAsync(int recordSetId, int userId = 1, int workstationId = 1, int startIndex = 0, int numRecords = 1000, CancellationToken cancellationToken = default)
        {
            Require.Positive(recordSetId);
            Require.Positive(userId);
            Require.Positive(workstationId);
            Require.NonNegative(startIndex);
            Require.Positive(numRecords);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/recordsets/{recordSetId}/records";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("startIndex", startIndex);
            request.QueryParameters.Add("numRecords", numRecords);
            request.QueryParameters.Add("userid", userId);
            request.QueryParameters.Add("wsid", workstationId);
            return await ExecutePapiAsync<RecordSetRecordsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
