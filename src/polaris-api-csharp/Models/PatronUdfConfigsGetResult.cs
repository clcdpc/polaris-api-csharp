using System.Collections.Generic;
using System.Linq;

namespace Clc.Polaris.Api.Models
{
    public class PatronUdfConfigsGetResult : PapiResponseCommon
    {
        public List<PatronUdfConfigsRow> PatronUdfConfigsRows { get; set; }

        public override string ToString()
        {
            return string.Join("\r\n", PatronUdfConfigsRows?.OrderBy(pc => pc.Label) ?? Enumerable.Empty<PatronUdfConfigsRow>());
        }
    }

    public class PatronUdfConfigsRow
    {
        public int PatronUdfID { get; set; }
        public string Label { get; set; }
        public bool Display { get; set; }
        public string Values { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }

        public override string ToString() => $"{PatronUdfID} - {Label}";
    }
}
