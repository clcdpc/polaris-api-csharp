namespace Clc.Polaris.Api.Models
{
    public class ItemUpdateBarcodeData
    {
        public int TransactionBranchId { get; set; }
        public string ItemBarcode { get; set; } = string.Empty;

        public override string ToString() => $"{TransactionBranchId} - {ItemBarcode}";
    }
}
