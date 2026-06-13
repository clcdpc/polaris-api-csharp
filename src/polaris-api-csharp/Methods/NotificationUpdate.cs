using Clc.Polaris.Api.Models;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<NotificationUpdateResult>> NotificationUpdateAsync(NotificationUpdateParams updateParams, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/notification/{updateParams.NotificationTypeId}";
            var request = PapiRestRequest.Put(url, body: updateParams);
            return await ExecutePapiAsync<NotificationUpdateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}