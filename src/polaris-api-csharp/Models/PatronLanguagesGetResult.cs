using System.Collections.Generic;
using System.Linq;

namespace Clc.Polaris.Api.Models
{
    public class PatronLanguagesGetResult : PapiResponseCommon
    {
        public List<PatronLanguageRow> PatronLanguagesRows { get; set; }

        public override string ToString()
        {
            return string.Join("\r\n", PatronLanguagesRows?.OrderBy(l => l.Description) ?? Enumerable.Empty<PatronLanguageRow>());
        }
    }

    public class PatronLanguageRow
    {
        public int LanguageID { get; set; }
        public string Description { get; set; }
        public int AdminLanguageID { get; set; }

        public override string ToString() => $"{LanguageID} - {Description}";
    }
}
