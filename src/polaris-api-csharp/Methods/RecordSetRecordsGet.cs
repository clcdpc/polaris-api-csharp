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
        

        public IRestResponse<RecordSetRecordsGetResult> RecordSetRecordsGet(int recordSetId, int userId = 1, int workstationId = 1, int startIndex = 0, int numRecords = 1000)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/recordsets/{recordSetId}/records?startIndex={startIndex}&numRecords={numRecords}&userid={userId}&wsid={workstationId}";
            var request = new PapiRestRequest(url);
            return Execute<RecordSetRecordsGetResult>(request);
        }
    }
}
