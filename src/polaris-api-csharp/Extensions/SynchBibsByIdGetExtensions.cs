namespace Clc.Polaris.Api
{
    public static class SynchBibsByIdGetExtensions
    {
        public static Task<IRestResponse<Sync_BibsByIdGetResult>> Synch_BibsByIdGetAsync(
            this IPapiClient client,
            int bibId,
            bool includeItems = false,
            CancellationToken cancellationToken = default)
        {
            Require.Positive(bibId);

            return client.Synch_BibsByIdGetAsync([bibId], includeItems, cancellationToken);
        }
    }
}
