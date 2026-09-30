namespace Clc.Polaris.Api.Models
{
    public class JobsPurchaseOrdersResultLineItem
    {
        public string? ExternalID { get; set; }
        public int? POLineItemID { get; set; }
        public string? Title { get; set; }
        public List<JobsPurchaseOrdersResultLineItemSegment>? Segments { get; set; }
    }
}
