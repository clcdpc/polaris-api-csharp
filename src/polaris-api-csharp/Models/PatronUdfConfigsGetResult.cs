using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class PatronUdfConfigsGetResult : PapiResponseCommon
    {
        public List<PatronUdfConfigRow> PatronUdfConfigsRows { get; set; } = new();
    }

    public class PatronUdfConfigRow
    {
        public int UDFID { get; set; }
        public int OrganizationID { get; set; }
        public string? Description { get; set; }
        public string? Label { get; set; }
        public bool? Enabled { get; set; }
    }
}
