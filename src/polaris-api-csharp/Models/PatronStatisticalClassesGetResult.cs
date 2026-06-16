
namespace Clc.Polaris.Api.Models
{
    public class PatronStatisticalClassesGetResult : PapiResponseCommon
    {
        public List<PatronStatisticalClassRow> PatronStatisticalClassesRows { get; set; } = new();
    }

    public class PatronStatisticalClassRow
    {
        public int StatisticalClassID { get; set; }
        public int OrganizationID { get; set; }
        public string? Description { get; set; }
    }
}
