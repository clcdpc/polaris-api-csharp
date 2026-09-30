using System.Xml.Serialization;

namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Result of a PatronAccountPay call
    /// </summary>
    [XmlRoot(ElementName = "PatronAccountPayResult")]
    public class PatronAccountPayResult : PapiResponseCommon
    {
    }
}
