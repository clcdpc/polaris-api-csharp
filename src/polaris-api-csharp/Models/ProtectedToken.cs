using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Protected token used to authenticate protected methods and public method overrides
    /// </summary>
    [XmlRoot(ElementName = "AuthenticationResult")]
    public class ProtectedToken : PapiResponseCommon
    {
        public const string Placeholder = "__PAPI_PROTECTED_ACCESS_TOKEN__";

        /// <summary>
        /// Access token
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Access secret
        /// </summary>
        public string? AccessSecret { get; set; }

        /// <summary>
        /// Token expiration date
        /// </summary>
        [XmlElement(ElementName = "AuthExpDate")]
        [JsonProperty("AuthExpDate")]
        public DateTime? ExpirationDate { get; set; }

        public ProtectedToken()
        {

        }

        public ProtectedToken(ProtectedToken pt)
        {
            AccessToken = pt.AccessToken;
            AccessSecret = pt.AccessSecret;
            ExpirationDate = pt.ExpirationDate;
        }
    }
}
