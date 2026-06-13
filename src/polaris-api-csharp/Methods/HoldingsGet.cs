using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        /// <summary>
        /// Returns holdings information for a supplied record.
        /// </summary>
        /// <param name="bibId">BibliograhpicRecordID of the record.</param>
        /// <returns>An object containing a list of holdings information for specified BibliographicRecordID.</returns>
        /// <seealso cref="BibHoldingsGetResult"/>


        public async Task<IRestResponse<BibHoldingsGetResult>> HoldingsGetAsync(int bibId, CancellationToken cancellationToken = default)
        {
            Require.Positive(bibId);

            var url = $"/public/v1/1033/100/{OrganizationId}/bib/{bibId}/holdings";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<BibHoldingsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
