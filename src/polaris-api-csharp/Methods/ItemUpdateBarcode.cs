using System.Globalization;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PAPIResult>> ItemUpdateBarcodeAsync(string newBarcode, int? itemRecordId = null, int? transactionBranchId = null, string oldBarcode = "", CancellationToken cancellationToken = default)
        {
            Require.Argument(newBarcode);

            string itemIdentifier;
            var isBarcodeLookup = false;

            Require.PositiveIfProvided(itemRecordId);
            Require.PositiveIfProvided(transactionBranchId);

            if (itemRecordId is int itemRecordIdentifier)
            {
                itemIdentifier = itemRecordIdentifier.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                Require.Argument(oldBarcode);
                itemIdentifier = EncodeBarcodePathSegment(oldBarcode);
                isBarcodeLookup = true;
            }

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/cataloging/items/{itemIdentifier}/barcode";
            var body = new ItemUpdateBarcodeData
            {
                ItemBarcode = newBarcode,
                TransactionBranchId = transactionBranchId ?? OrganizationId
            };

            var request = PapiRestRequest.Put(url, body: body);
            request.QueryParameters.Add("wsid", WorkstationId);

            if (isBarcodeLookup)
            {
                request.QueryParameters.Add("isBarcode", 1);
            }

            return await ExecutePapiAsync<PAPIResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}