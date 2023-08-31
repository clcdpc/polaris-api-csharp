using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public IRestResponse<RemoteStorageItemsGetResult> RemoteStorageItemsGet(int branchId, string startDate, string endDate, int maxItems, int listType, int? startItemRecordId = null)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/cataloging/remotestorage/items?branch={branchId}&startdate={startDate}&enddate={endDate}&maxitems={maxItems}&listtype={(int)listType}";
            if (startItemRecordId.HasValue) { url += $"&startitemrecordid={startItemRecordId.Value}"; }
            var request = new PapiRestRequest(url);
            return Execute<RemoteStorageItemsGetResult>(request);
        }
    }
}