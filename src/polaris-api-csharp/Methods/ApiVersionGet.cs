
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
            var request = PapiRestRequest.Get($"/public/v1/1033/100/{OrganizationId}/api");
            return await ExecutePapiAsync<ApiResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
