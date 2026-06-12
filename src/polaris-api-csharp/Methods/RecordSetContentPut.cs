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


        public async Task<IRestResponse<PapiResponseCommon>> RecordSetContentPutAsync(int recordSetId, IEnumerable<int> records, RecordSetContentPutActions action, int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(recordSetId);
            ArgumentNullException.ThrowIfNull(records);
            Require.PositiveIfProvided(userId);
            Require.PositiveIfProvided(workstationId);

            var recordList = records.ToArray();
            if (recordList.Length == 0)
            {
                throw new ArgumentException("At least one record is required.", nameof(records));
            }

            foreach (var recordId in recordList)
            {
                Require.Positive(recordId);
            }

            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/recordsets/{recordSetId}";
            var body = new { records = string.Join(",", recordList) };
            var request = PapiRestRequest.Put(url, body: body);
            request.QueryParameters.Add("action", action);
            request.QueryParameters.Add("userid", userId ?? UserId);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
