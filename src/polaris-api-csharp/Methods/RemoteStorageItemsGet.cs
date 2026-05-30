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
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/cataloging/remotestorage/items";
            var request = new PapiRestRequest(url);
            request.QueryParameters.Add("branch", branchId);
            request.QueryParameters.Add("startdate", startDate);
            request.QueryParameters.Add("enddate", endDate);
            request.QueryParameters.Add("maxitems", maxItems);
            request.QueryParameters.Add("listtype", (int)listType);
            if (startItemRecordId.HasValue) { request.QueryParameters.Add("startitemrecordid", startItemRecordId.Value); }
            return Execute<RemoteStorageItemsGetResult>(request);
        }
    }
}