using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Net;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<JobsPurchaseOrdersPostResult> JobsPurchaseOrdersPost(JobsPurchaseOrdersPostData data, int? orgId = null)
        {
            var url = $"/protected/v1/1033/100/{orgId ?? OrganizationId}/{Token.AccessToken}/jobs/purchaseorders";
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = data };
            return Execute<JobsPurchaseOrdersPostResult>(request);
        }

        public IRestResponse<JobsPurchaseOrdersPreorderValidationResult> JobsPurchaseOrdersPreorderValidation(JobsPurchaseOrdersPutData data, int? orgId = null)
        {
            var url = $"/protected/v1/1033/100/{orgId ?? OrganizationId}/{Token.AccessToken}/jobs/purchaseorders?preordervalidation=1";
            var request = new PapiRestRequest(HttpMethod.Put, url) { Body = data };
            return Execute<JobsPurchaseOrdersPreorderValidationResult>(request);
        }

        public IRestResponse<JobsPurchaseOrdersStatusResult> JobsPurchaseOrdersStatusGet(Guid jobGuid, int? orgId = null)
        {
            var url = $"/protected/v1/1033/100/{orgId ?? OrganizationId}/{Token.AccessToken}/jobs/purchaseorders/{jobGuid}/status";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<JobsPurchaseOrdersStatusResult>(request);
        }

        public IRestResponse<JobsPurchaseOrdersResultData> JobsPurchaseOrdersResultGet(Guid jobGuid, int? orgId = null)
        {
            var url = $"/protected/v1/1033/100/{orgId ?? OrganizationId}/{Token.AccessToken}/jobs/purchaseorders/{jobGuid}/result";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<JobsPurchaseOrdersResultData>(request);
        }
    }
}
