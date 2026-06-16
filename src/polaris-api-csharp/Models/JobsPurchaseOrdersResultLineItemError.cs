namespace Clc.Polaris.Api.Models
{
    public class JobsPurchaseOrdersResultLineItemError
    {
        public string? ExternalID { get; set; }
        public int? LineItemNumber { get; set; }
        public string? ErrorMessage { get; set; }
        public List<JobsPurchaseOrdersResultLineItemSegmentError>? SegmentErrors { get; set; }
    }
}
