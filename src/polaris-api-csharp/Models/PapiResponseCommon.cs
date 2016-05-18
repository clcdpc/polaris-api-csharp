using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        [XmlElement(ElementName = "PAPIErrorCode")]
        public int PAPIErrorCode { get; set; }

        /// <summary>
        /// Error message, if any
        /// </summary>
        [XmlElement(ElementName = "ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}
