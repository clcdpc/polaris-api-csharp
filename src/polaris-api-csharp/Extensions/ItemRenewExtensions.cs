namespace Clc.Polaris.Api
{
    public static class ItemRenewExtensions
    {
        public static Task<IRestResponse<ItemRenewResultWrapper>> ItemRenewAllForPatronAsync(
            this IPapiClient client,
            string barcode,
            string password = "",
            ItemRenewOptions? renewOptions = null,
            CancellationToken cancellationToken = default)
        {
            return client.ItemRenewAsync(barcode, 0, password, renewOptions, cancellationToken);
        }
    }
}
