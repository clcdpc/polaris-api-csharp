namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Payment methods
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// Cash
        /// </summary>
        Cash = 11,

        /// <summary>
        /// Credit card
        /// </summary>
        CreditCard,

        /// <summary>
        /// Debit card
        /// </summary>
        DebitCard,

        /// <summary>
        /// Check
        /// </summary>
        Check,

        /// <summary>
        /// Voucher
        /// </summary>
        Voucher,

        /// <summary>
        /// Smart card
        /// </summary>
        SmartCard = 17
    }
}
