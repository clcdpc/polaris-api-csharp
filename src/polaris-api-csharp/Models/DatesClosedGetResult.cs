
namespace Clc.Polaris.Api.Models
{
    public class DatesClosedGetResult
    {
        public List<DatesClosedRow> DatesClosedRows { get; set; } = new();

        public override string ToString()
        {
            return string.Join(", ", DatesClosedRows);
        }
    }

    public class DatesClosedRow
    {
        public DateTime DateClosed { get; set; }

        public override string ToString()
        {
            return DateClosed.ToShortDateString();
        }
    }
}
