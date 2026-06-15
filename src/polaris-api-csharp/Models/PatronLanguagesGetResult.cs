using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class PatronLanguagesGetResult : PapiResponseCommon
    {
        public List<PatronLanguageRow> PatronLanguagesRows { get; set; } = new();
    }

    public class PatronLanguageRow
    {
        public int LanguageID { get; set; }
        public string? Description { get; set; }
    }
}
