namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronRegistrationUpdateResult>> PatronRegistrationUpdateV2Async(string barcode, PatronRegistrationData registrationData, string password = "", bool ignoresa = true, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(registrationData);
            registrationData.LogonBranchID = Require.PositiveIfProvidedOrDefault(registrationData.LogonBranchID, OrganizationId);
            registrationData.LogonUserID = Require.PositiveIfProvidedOrDefault(registrationData.LogonUserID, UserId);
            registrationData.LogonWorkstationID = Require.PositiveIfProvidedOrDefault(registrationData.LogonWorkstationID, WorkstationId);
            Require.Positive(registrationData.PatronBranchID);
            Require.PositiveIfProvided(registrationData.RequestPickupBranchID);

            var url = $"/public/v2/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}";
            var request = PapiRestRequest.Put(url, body: registrationData, password: password);
            request.QueryParameters.Add("ignoresa", ignoresa);
            return await ExecutePapiAsync<PatronRegistrationUpdateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
