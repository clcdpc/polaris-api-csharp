namespace Clc.Polaris.Api.Models
{
    public class ItemStatusesGetResult : PapiResponseCommon
    {
        public ItemStatusRow[] ItemStatusesRows { get; set; } = Array.Empty<ItemStatusRow>();

        public override string ToString() => string.Join("\r\n", ItemStatusesRows ?? Enumerable.Empty<ItemStatusRow>());
    }

    public class ItemStatusRow
    {
        public int ItemStatusId { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public string? BannerText { get; set; }

        public override string ToString() => $"{ItemStatusId} - {Name ?? string.Empty} - {Description ?? string.Empty}";
    }
}
