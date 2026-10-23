namespace Clc.Polaris.Api.Models
{
    public class JobsPurchaseOrdersResultGetResult
    {
        public int PAPIErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ExternalID { get; set; }
        public string? PONumber { get; set; }
        public int? PurchaseOrderID { get; set; }
        public List<JobsPurchaseOrdersResultLineItem>? LineItems { get; set; }
        public List<JobsPurchaseOrdersResultLineItemError>? LineItemErrors { get; set; }
        public List<JobsPurchaseOrdersResultLineItemSegmentError>? ItemRecordCreateErrors { get; set; }
    }
}
