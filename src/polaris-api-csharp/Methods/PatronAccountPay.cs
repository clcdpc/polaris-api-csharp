namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronAccountPayResult>> PatronAccountPayAsync(string barcode, int txnId, double txnAmount, PaymentMethod paymentMethod, int? workstationId = null, int? userId = null, string note = "", CancellationToken cancellationToken = default)
        {
            Require.Positive(txnId);
            Require.PositiveIfProvided(workstationId);
            Require.PositiveIfProvided(userId);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/patron/{EncodeBarcodePathSegment(barcode)}/account/{txnId}/pay";
            var body = new PatronAccountPayData { TxnAmount = txnAmount, PaymentMethodId = paymentMethod, FreeTextNote = note };
            var request = PapiRestRequest.Put(url, body: body);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("userid", userId ?? UserId);
            return await ExecutePapiAsync<PatronAccountPayResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
