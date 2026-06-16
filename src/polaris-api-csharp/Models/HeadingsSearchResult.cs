namespace Clc.Polaris.Api.Models
{
    public class HeadingsSearchResult : PapiResponseCommon
    {
        public List<HeadingsSearchRow> HeadingsSearchRows { get; set; } = new();
    }
}
