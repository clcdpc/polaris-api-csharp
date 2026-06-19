namespace Clc.Polaris.Api.Models
{
    public class SortOptionsGetResult : PapiResponseCommon
    {
        public List<SortOptionsRow> SortOptionsRows { get; set; } = new();

        public override string ToString() => string.Join("\r\n", SortOptionsRows ?? Enumerable.Empty<SortOptionsRow>());
    }
}
