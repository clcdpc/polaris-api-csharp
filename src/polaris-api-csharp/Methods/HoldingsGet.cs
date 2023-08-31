using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;
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
        

        public IRestResponse<BibHoldingsGetResult> HoldingsGet(int bibId)
        {
            var url = $"/public/v1/1033/100/1/bib/{bibId}/holdings";
            var request = new PapiRestRequest(url);
            return Execute<BibHoldingsGetResult>(request);
        }
    }
}