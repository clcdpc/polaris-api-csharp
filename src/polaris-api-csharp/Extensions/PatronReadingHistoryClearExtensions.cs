namespace Clc.Polaris.Api
{
    public static class PatronReadingHistoryClearExtensions
    {
        public static Task<IRestResponse<PatronReadingHistoryClearResult>> PatronReadingHistoryClearAsync(this IPapiClient client, string barcode, IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            return client.PatronReadingHistoryClearAsync(barcode, "", ids, cancellationToken);
        }
    }
}
