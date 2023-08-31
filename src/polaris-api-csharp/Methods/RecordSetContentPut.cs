using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public IRestResponse<PapiResponseCommon> RecordSetContentPut(int recordSetId, IEnumerable<int> records, RecordSetContentPutActions action, int? userId = null, int? workstationId = null)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/recordsets/{recordSetId}?action={action}&userid={userId ?? UserId}&wsid={workstationId ?? WorkstationId}";
            var body = new { records = string.Join(",", records.Select(r => r.ToString())) };
            var request = new PapiRestRequest(HttpMethod.Put, url) { Body = body };
            return Execute<PapiResponseCommon>(request);
        }
        public IRestResponse<PapiResponseCommon> RecordSetContentAdd(int recordSetId, int recordId, int userId = 1, int workstationId = 1)
        {
            return RecordSetContentPut(recordSetId, new[] { recordId }, RecordSetContentPutActions.Add, userId, workstationId);
        }
        public IRestResponse<PapiResponseCommon> RecordSetContentAdd(int recordSetId, IEnumerable<int> records, int userId = 1, int workstationId = 1)
        {
            return RecordSetContentPut(recordSetId, records, RecordSetContentPutActions.Add, userId, workstationId);
        }
        public IRestResponse<PapiResponseCommon> RecordSetContentRemove(int recordSetId, int recordId, int userId = 1, int workstationId = 1)
        {
            return RecordSetContentPut(recordSetId, new[] { recordId }, RecordSetContentPutActions.Remove, userId, workstationId);
        }
        public IRestResponse<PapiResponseCommon> RecordSetContentRemove(int recordSetId, IEnumerable<int> records, int userId = 1, int workstationId = 1)
        {
            return RecordSetContentPut(recordSetId, records, RecordSetContentPutActions.Remove, userId, workstationId);
        }
    }    
}
