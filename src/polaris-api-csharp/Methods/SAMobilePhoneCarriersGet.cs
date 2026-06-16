
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<SAMobilePhoneCarriersGetResult>> SAMobilePhoneCarriersGetAsync(int? organizationId = null, CancellationToken cancellationToken = default)
        {
            Require.PositiveIfProvided(organizationId);

            var url = $"/protected/v1/1033/100/{organizationId ?? OrganizationId}/{ProtectedToken.Placeholder}/sysadmin/mobilephonecarriers";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<SAMobilePhoneCarriersGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
