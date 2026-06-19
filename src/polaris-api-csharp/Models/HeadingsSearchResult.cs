namespace Clc.Polaris.Api.Models
{
    public class HeadingsSearchResult : PapiResponseCommon
    {
        public List<HeadingsSearchRow> HeadingsSearchRows { get; set; } = new();

        public override string ToString() => string.Join("\r\n", HeadingsSearchRows ?? Enumerable.Empty<HeadingsSearchRow>());
    }
}
