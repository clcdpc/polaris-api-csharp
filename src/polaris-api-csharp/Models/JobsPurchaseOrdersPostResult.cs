namespace Clc.Polaris.Api.Models
{
    public class JobsPurchaseOrdersPostResult
    {
        public int PAPIErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ExternalID { get; set; }
        public string? JobID { get; set; }
        public string? JobGuid { get; set; }
        public int? JobStatusID { get; set; }
        public string? JobStatusDescription { get; set; }
        public List<PurchaseOrderLineItemValidationError>? LineItemValidationErrors { get; set; }
    }
}
