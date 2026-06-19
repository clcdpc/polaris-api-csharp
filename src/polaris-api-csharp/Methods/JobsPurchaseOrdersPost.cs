namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<JobsPurchaseOrdersPostResult>> JobsPurchaseOrdersPostAsync(JobsPurchaseOrdersCreateData data, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(data);
            ValidateJobsPurchaseOrdersCreateData(data);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/jobs/purchaseorders";
            var request = PapiRestRequest.Post(url, body: data);
            return await ExecutePapiAsync<JobsPurchaseOrdersPostResult>(request, cancellationToken).ConfigureAwait(false);
        }

        private static void ValidateJobsPurchaseOrdersCreateData(JobsPurchaseOrdersCreateData data)
        {
            Require.Argument(data.Vendor);
            Require.Argument(data.OrderedAtLocation);
            Require.Argument(data.OrderType);
            Require.Argument(data.PaymentMethod);
            Require.Argument(data.PONumber);
            Require.Argument(data.MARCLineItems);

            if (data.MARCLineItems.Count == 0)
            {
                throw new ArgumentException("At least one MARC line item is required.", nameof(data.MARCLineItems));
            }

            for (var i = 0; i < data.MARCLineItems.Count; i++)
            {
                var lineItem = data.MARCLineItems[i] ?? throw new ArgumentException("MARC line items cannot contain null entries.", nameof(data.MARCLineItems));
                Require.PositiveIfProvided(lineItem.Copies);

                if (lineItem.MarcRecord == null)
                {
                    throw new ArgumentException("Each MARC line item must include a MARC record.", nameof(data.MARCLineItems));
                }

                var hasLeader = !string.IsNullOrWhiteSpace(lineItem.MarcRecord.Leader);
                var hasControlFields = lineItem.MarcRecord.ControlFields?.Count > 0;
                var hasDataFields = lineItem.MarcRecord.DataFields?.Count > 0;
                if (!hasLeader && !hasControlFields && !hasDataFields)
                {
                    throw new ArgumentException("Each MARC record must include a leader, control field, or data field.", nameof(data.MARCLineItems));
                }
            }
        }
    }
}
