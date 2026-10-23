namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Common elements of every PAPI response
    /// </summary>
    public class PapiResponseCommon
    {
        /// <summary>
        /// PAPI error code, see PAPI documentation for more information
        /// </summary>
        public int PAPIErrorCode { get; set; }

        /// <summary>
        /// Error message, if any
        /// </summary>
        public string? ErrorMessage { get; set; }

        public override string ToString()
        {
            return $"{PAPIErrorCode} - {ErrorMessage}";
        }
    }
}
