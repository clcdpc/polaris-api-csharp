using System.Collections.Generic;
using System.Linq;

namespace Clc.Polaris.Api.Models
{
    public class PatronStatisticalClassesGetResult : PapiResponseCommon
    {
        public List<PatronStatisticalClassRow> PatronStatisticalClassesRows { get; set; }

        public override string ToString()
        {
            return string.Join("\r\n", PatronStatisticalClassesRows?.OrderBy(sc => sc.Description) ?? Enumerable.Empty<PatronStatisticalClassRow>());
        }
    }

    public class PatronStatisticalClassRow
    {
        public int StatisticalClassID { get; set; }
        public int OrganizationID { get; set; }
        public string Description { get; set; }

        public override string ToString() => $"{StatisticalClassID} - {Description}";
    }
}
