namespace Clc.Polaris.Api.Models
{
    public class JobsPurchaseOrdersPreorderValidationLineItem
    {
        public string? ISBN { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int? Copies { get; set; }
        public List<JobsPurchaseOrdersPreorderValidationLineItemSegment>? Segments { get; set; }
    }
}
