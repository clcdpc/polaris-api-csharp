namespace Clc.Polaris.Api.Models
{
    public class PatronLanguagesGetResult : PapiResponseCommon
    {
        public List<PatronLanguageRow> PatronLanguagesRows { get; set; } = new();

        public override string ToString() => string.Join("\r\n", PatronLanguagesRows ?? Enumerable.Empty<PatronLanguageRow>());
    }

    public class PatronLanguageRow
    {
        public int LanguageID { get; set; }
        public string? Description { get; set; }

        public override string ToString() => $"{LanguageID} - {Description ?? string.Empty}";
    }
}
