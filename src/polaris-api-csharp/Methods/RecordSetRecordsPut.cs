using Clc.Polaris.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public PapiResponse<StringResult> RecordSetRecordsPut(int recordSetId, string patrons, RecordSetPutActions action, int userId, int workstationId)
        {
            var url = $"/PAPIService/REST/protected/v1/1033/100/1/{Token.AccessToken}/recordsets/{recordSetId}?action={action.ToString()}&userid={userId}&wsid={workstationId}";
            var body = "<?xml version=\"1.0\"?><ModifyRecordSetContent><Records>228783</Records></ModifyRecordSetContent>";
            return Execute<StringResult>(HttpMethod.Put, url, Token.AccessSecret, body);
        }
    }

    public enum RecordSetPutActions
    {
        Remove = 0,
        Add
    }
}
