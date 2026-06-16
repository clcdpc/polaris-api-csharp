namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<JobsPurchaseOrdersPreorderValidationResult>> JobsPurchaseOrdersPutAsync(JobsPurchaseOrdersPreorderValidationData data, int preOrderValidation = 1, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(data);
            if (preOrderValidation is not 0 and not 1) throw new ArgumentOutOfRangeException(nameof(preOrderValidation));
            ValidateJobsPurchaseOrdersPreorderValidationData(data);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/jobs/purchaseorders";
            var request = PapiRestRequest.Put(url, body: data);
            request.QueryParameters["preordervalidation"] = preOrderValidation;
            return await ExecutePapiAsync<JobsPurchaseOrdersPreorderValidationResult>(request, cancellationToken).ConfigureAwait(false);
        }

        private static void ValidateJobsPurchaseOrdersPreorderValidationData(JobsPurchaseOrdersPreorderValidationData data)
        {
            Require.Argument(data.Vendor);
            Require.Argument(data.OrderedAtLocation);
            Require.Argument(data.OrderType);
            Require.Argument(data.PaymentMethod);
            Require.Argument(data.LineItems);

            if (data.LineItems.Count == 0)
            {
                throw new ArgumentException("At least one line item is required.", nameof(data.LineItems));
            }

            for (var i = 0; i < data.LineItems.Count; i++)
            {
                var lineItem = data.LineItems[i] ?? throw new ArgumentException("Line items cannot contain null entries.", nameof(data.LineItems));
                if (string.IsNullOrWhiteSpace(lineItem.Title) && string.IsNullOrWhiteSpace(lineItem.ISBN))
                {
                    throw new ArgumentException("Each line item must include a title or ISBN.", nameof(data.LineItems));
                }

                Require.PositiveIfProvided(lineItem.Copies);
                Require.Argument(lineItem.Segments);
                if (lineItem.Segments.Count == 0)
                {
                    throw new ArgumentException("Each line item must include at least one segment.", nameof(data.LineItems));
                }

                for (var segmentIndex = 0; segmentIndex < lineItem.Segments.Count; segmentIndex++)
                {
                    var segment = lineItem.Segments[segmentIndex] ?? throw new ArgumentException("Line item segments cannot contain null entries.", nameof(data.LineItems));
                    Require.Argument(segment.Collection);
                    Require.Argument(segment.Fund);
                    Require.PositiveIfProvided(segment.Copies);
                    if (segment.UnitPrice < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(segment.UnitPrice), segment.UnitPrice, "Value cannot be negative.");
                    }
                }
            }
        }
    }
}
