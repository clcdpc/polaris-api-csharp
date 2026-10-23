namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        /// <summary>
        /// Get an SA value
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public async Task<IRestResponse<StringResult>> SA_GetValueByOrgAsync(string attribute, int? organizationId = null, CancellationToken cancellationToken = default)
        {
            Require.PositiveIfProvided(organizationId);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/organization/{organizationId ?? OrganizationId}/sysadmin/attribute/{attribute}";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<StringResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
