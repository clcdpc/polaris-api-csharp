using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<PapiResponseCommon> NotificationQueueGet(int orgId = 1)
        {
            //"protected/{Version}/{LangID}/{AppID}/{OrgID}/{AccessToken}/notification
            var url = $"/protected/v1/1033/24/{orgId}/{Token.AccessToken}/notification/";
            var request = new PapiRestRequest(url);
            return Execute<PapiResponseCommon>(request);
        }
    }
}
