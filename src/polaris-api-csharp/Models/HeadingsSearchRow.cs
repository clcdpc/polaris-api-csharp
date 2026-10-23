namespace Clc.Polaris.Api.Models
{
    public class HeadingsSearchRow
    {
        public int? Position { get; set; }
        public int? DisplayType { get; set; }
        public string? DisplayConstant { get; set; }
        public string? DisplayTerm { get; set; }
        public int? GlobalOccurrences { get; set; }
        public string? HeadingID { get; set; }

        public override string ToString() => $"{Position?.ToString() ?? string.Empty} - {HeadingID ?? string.Empty} - {DisplayTerm ?? DisplayConstant ?? string.Empty}";
    }
}
