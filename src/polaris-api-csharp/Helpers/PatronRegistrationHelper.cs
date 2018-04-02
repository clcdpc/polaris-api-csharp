using Clc.Polaris.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Clc.Polaris.Api.Helpers
{
    /// <summary>
    /// Helper methods for PAPI patron registration
    /// </summary>
    public class PatronRegistrationHelper
    {
        /// <summary>
        /// Build the XML for a PAPI patron registration
        /// </summary>
        /// <param name="_params"></param>
        /// <returns></returns>
        public static string BuildXml(PatronRegistrationParams _params)
        {
            var doc = new XDocument();
            var root = new XElement("PatronRegistrationCreateData");

            foreach (var info in _params.GetType().GetProperties())
            {
                var val = info.GetValue(_params, null);
                if (val == null) continue;
                
                root.Add(new XElement(info.Name, val.GetType() == typeof(DateTime) ? ((DateTime)val).ToString("s") : val));                
            }

            doc.Add(root);

            return doc.ToString();
        }
    }
}
