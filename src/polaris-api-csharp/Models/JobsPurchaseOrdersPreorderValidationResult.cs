namespace Clc.Polaris.Api.Models
{
    public class JobsPurchaseOrdersPreorderValidationResult
    {
        public int PAPIErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ExternalID { get; set; }
        public List<PurchaseOrderLineItemValidationError>? LineItemValidationErrors { get; set; }
    }
}
