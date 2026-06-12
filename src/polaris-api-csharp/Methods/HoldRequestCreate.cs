using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<HoldRequestCreateResult>> HoldRequestCreateAsync(HoldRequestCreateParams holdParams, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(holdParams);
            Require.Positive(holdParams.PatronID);
            Require.Positive(holdParams.BibID);
            Require.PositiveIfProvided(holdParams.PickupOrgID == 0 ? null : holdParams.PickupOrgID);
            Require.Positive(holdParams.WorkstationID);
            Require.Positive(holdParams.UserID);
            Require.Positive(holdParams.RequestingOrgID);

            var url = $"/public/v1/1033/100/{holdParams.RequestingOrgID}/holdrequest";
            var request = PapiRestRequest.Post(url, body: holdParams);
            return await ExecutePapiAsync<HoldRequestCreateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
