namespace Clc.Polaris.Api.Models
{
    public class SortOptionsRow
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Options { get; set; }

        public override string ToString() => $"{Code ?? string.Empty} - {Description ?? string.Empty} - {Options ?? string.Empty}";
    }
}
