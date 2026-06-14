using Clc.Polaris.Api.Models;
using Clc.Rest;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public static class RecordSetContentPutExtensions
    {
        public static Task<IRestResponse<PapiResponseCommon>> RecordSetContentAddAsync(
            this IPapiClient client,
            int recordSetId,
            int recordId,
            int? userId = null,
            int? workstationId = null,
            CancellationToken cancellationToken = default)
        {
            return client.RecordSetContentPutAsync(recordSetId, [recordId], RecordSetContentPutActions.Add, userId, workstationId, cancellationToken);
        }

        public static Task<IRestResponse<PapiResponseCommon>> RecordSetContentAddAsync(
            this IPapiClient client,
            int recordSetId,
            IEnumerable<int> records,
            int? userId = null,
            int? workstationId = null,
            CancellationToken cancellationToken = default)
        {
            return client.RecordSetContentPutAsync(recordSetId, records, RecordSetContentPutActions.Add, userId, workstationId, cancellationToken);
        }

        public static Task<IRestResponse<PapiResponseCommon>> RecordSetContentRemoveAsync(
            this IPapiClient client,
            int recordSetId,
            int recordId,
            int? userId = null,
            int? workstationId = null,
            CancellationToken cancellationToken = default)
        {
            return client.RecordSetContentPutAsync(recordSetId, [recordId], RecordSetContentPutActions.Remove, userId, workstationId, cancellationToken);
        }

        public static Task<IRestResponse<PapiResponseCommon>> RecordSetContentRemoveAsync(
            this IPapiClient client,
            int recordSetId,
            IEnumerable<int> records,
            int? userId = null,
            int? workstationId = null,
            CancellationToken cancellationToken = default)
        {
            return client.RecordSetContentPutAsync(recordSetId, records, RecordSetContentPutActions.Remove, userId, workstationId, cancellationToken);
        }
    }
}
