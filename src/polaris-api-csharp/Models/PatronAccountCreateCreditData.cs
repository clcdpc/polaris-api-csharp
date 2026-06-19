namespace Clc.Polaris.Api.Models
{
    public class PatronAccountCreateCreditData
    {
        public double TxnAmount { get; set; }
        public PaymentMethod PaymentMethodId { get; set; }
        public string FreeTextNote { get; set; } = string.Empty;
    }
}
