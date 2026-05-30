
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="listId"></param>
        /// <param name="position">starts at 1, not 0</param>
        /// <param name="password"></param>
        /// <returns></returns>


        public async Task<IRestResponse<PapiResponseCommon>> PatronTitleListDeleteTitleAsync(string barcode, int listId, int position, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/patrontitlelistdeletetitle";
            var request = PapiRestRequest.Delete(url, password: password);
            request.QueryParameters.Add("list", listId);
            request.QueryParameters.Add("position", position);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
