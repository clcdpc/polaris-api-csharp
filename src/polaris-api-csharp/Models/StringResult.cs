namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Result of a SA_GetValueByOrg call
    /// </summary>
    public class StringResult : PapiResponseCommon
    {
        /// <summary>
        /// Value
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Returns value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value ?? string.Empty;
        }
    }
}
