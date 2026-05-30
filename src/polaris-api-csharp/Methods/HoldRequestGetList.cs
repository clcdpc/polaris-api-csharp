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
        public IRestResponse<HoldRequestGetListResult> HoldRequestGetList(int branchId, RequestListBranchType branchType = RequestListBranchType.PickupBranch, HoldStatus status = HoldStatus.Held)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/circulation/requests/list";
            var request = new PapiRestRequest(url);
            request.QueryParameters.Add("branch", branchId);
            request.QueryParameters.Add("branchtype", (int)branchType);
            request.QueryParameters.Add("requeststatus", (int)status);
            return Execute<HoldRequestGetListResult>(request);
        }
    }    
}
