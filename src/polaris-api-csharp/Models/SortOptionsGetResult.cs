using System.Collections.Generic;
using System.Linq;

namespace Clc.Polaris.Api.Models
{
    public class SortOptionsGetResult : PapiResponseCommon
    {
        public List<SortOptionRow> SortOptionsRows { get; set; }

        public override string ToString()
        {
            return string.Join("\r\n", SortOptionsRows?.OrderBy(so => so.Description) ?? Enumerable.Empty<SortOptionRow>());
        }
    }

    public class SortOptionRow
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Options { get; set; }

        public override string ToString() => $"{Code} - {Description}";
    }
}
