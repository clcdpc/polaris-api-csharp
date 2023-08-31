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
        

        public IRestResponse<HoldRequestCreateResult> HoldRequestCreate(HoldRequestCreateParams holdParams)
        {
            var url = $"/public/v1/1033/100/{holdParams.RequestingOrgID}/holdrequest";
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = holdParams };
            return Execute<HoldRequestCreateResult>(request);
        }
        public IRestResponse<HoldRequestCreateResult> HoldRequestCreate(int patronId, int bibId, int pickupBranchId = 0, DateTime? activationDate = null, int? userId = null, int? workstationId = null, int? requestingOrgId = null)
        {
            var holdParams = new HoldRequestCreateParams
            {
                PatronID = patronId,
                BibID = bibId,
                ActivationDate = activationDate,
                PickupOrgID = pickupBranchId,
                UserID = userId ?? UserId,
                WorkstationID = workstationId ?? WorkstationId,
                RequestingOrgID = requestingOrgId ?? OrganizationId
            };

            return HoldRequestCreate(holdParams);
        }
    }
}