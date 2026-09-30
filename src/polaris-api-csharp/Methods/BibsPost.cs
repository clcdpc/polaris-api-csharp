using System.Net.Http;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<BibsPostResult>> BibsPostAsync(string marcXml, string? importProfileName = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            Require.Argument(marcXml);
            Require.PositiveIfProvided(workstationId);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/bibs";
            var request = PapiRestRequest.Post(url);
            request.Content = new StringContent(marcXml, Encoding.UTF8, "application/xml");
            if (!string.IsNullOrWhiteSpace(importProfileName)) request.QueryParameters["ImportProfileName"] = importProfileName;
            if (workstationId.HasValue) request.QueryParameters["WorkstationID"] = workstationId.Value;
            return await ExecutePapiAsync<BibsPostResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
