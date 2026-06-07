using Clc.Rest;
using Clc.Polaris.Api.Models;
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
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/recordsets/{recordSetId}";
            var body = new { records = string.Join(",", records) };
            var request = PapiRestRequest.Put(url, body: body);
            request.QueryParameters.Add("action", action);
            request.QueryParameters.Add("userid", userId ?? UserId);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            return await ExecutePapiCoreAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
        public async Task<IRestResponse<PapiResponseCommon>> RecordSetContentAddAsync(int recordSetId, int recordId, int userId = 1, int workstationId = 1, CancellationToken cancellationToken = default)
        {
            return await RecordSetContentPutAsync(recordSetId, new[] { recordId }, RecordSetContentPutActions.Add, userId, workstationId, cancellationToken).ConfigureAwait(false);
        }
        public async Task<IRestResponse<PapiResponseCommon>> RecordSetContentAddAsync(int recordSetId, IEnumerable<int> records, int userId = 1, int workstationId = 1, CancellationToken cancellationToken = default)
        {
            return await RecordSetContentPutAsync(recordSetId, records, RecordSetContentPutActions.Add, userId, workstationId, cancellationToken).ConfigureAwait(false);
        }
        public async Task<IRestResponse<PapiResponseCommon>> RecordSetContentRemoveAsync(int recordSetId, int recordId, int userId = 1, int workstationId = 1, CancellationToken cancellationToken = default)
        {
            return await RecordSetContentPutAsync(recordSetId, new[] { recordId }, RecordSetContentPutActions.Remove, userId, workstationId, cancellationToken).ConfigureAwait(false);
        }
        public async Task<IRestResponse<PapiResponseCommon>> RecordSetContentRemoveAsync(int recordSetId, IEnumerable<int> records, int userId = 1, int workstationId = 1, CancellationToken cancellationToken = default)
        {
            return await RecordSetContentPutAsync(recordSetId, records, RecordSetContentPutActions.Remove, userId, workstationId, cancellationToken).ConfigureAwait(false);
        }
    }
}
