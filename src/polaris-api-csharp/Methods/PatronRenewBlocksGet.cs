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
        

        public IRestResponse<PatronRenewBlocksResult> PatronRenewBlocksGet(int patronId, int? branchId = null)
        {
            var url = $"/protected/v1/1033/100/{branchId ?? OrganizationId}/{Token.AccessToken}/circulation/patron/{patronId}/renewblocks";
            var request = new PapiRestRequest(url);
            return Execute<PatronRenewBlocksResult>(request);
        }
    }
}