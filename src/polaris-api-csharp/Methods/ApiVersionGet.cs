using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        /// <summary>
		/// Returns PAPI version information
		/// </summary>
		/// <returns>ApiVersionGetResponse</returns>
		/// <seealso cref="ApiResult"/>
		public async Task<IRestResponse<ApiResult>> ApiVersionGetAsync(CancellationToken cancellationToken = default)
        {
            var url = "/public/v1/1033/100/1/api";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<ApiResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
