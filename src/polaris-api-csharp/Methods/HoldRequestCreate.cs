using System;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public async Task<IRestResponse<HoldRequestCreateResult>> HoldRequestCreateAsync(HoldRequestCreateParams holdParams, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{holdParams.RequestingOrgID}/holdrequest";
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = holdParams };
            return await ExecutePapiAsync<HoldRequestCreateResult>(request, cancellationToken).ConfigureAwait(false);
        }
        public async Task<IRestResponse<HoldRequestCreateResult>> HoldRequestCreateAsync(int patronId, int bibId, int pickupBranchId = 0, DateTime? activationDate = null, int? userId = null, int? workstationId = null, int? requestingOrgId = null, CancellationToken cancellationToken = default)
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

            return await HoldRequestCreateAsync(holdParams, cancellationToken).ConfigureAwait(false);
        }
    }
}
