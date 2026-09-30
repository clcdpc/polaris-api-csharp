namespace Clc.Polaris.Api.Models
{
    public class PurchaseOrderLineItemValidationError
    {
        public string? ExternalID { get; set; }
        public int? LineItemNumber { get; set; }
        public List<PurchaseOrderValidationError>? ValidationErrors { get; set; }
    }
}
