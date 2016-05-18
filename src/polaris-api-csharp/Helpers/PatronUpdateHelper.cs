using Clc.Polaris.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Clc.Polaris.Api.Helpers
{
    /// <summary>
    /// Contains helper methods for PAPI patron updates
    /// </summary>
    public class PatronUpdateHelper
    {
        /// <summary>
        /// Build the XML for a PAPI patron update
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string BuildXml(PatronUpdateParams options)
        {
            var doc = new XDocument();
            var root = new XElement("PatronUpdateData");

            root.Add(new XElement("LogonBranchID", options.BranchId));
            root.Add(new XElement("LogonUserID", options.UserId));
            root.Add(new XElement("LogonWorkstationID", options.WorkstationId));
            if (options.ReadingListEnabled.HasValue) root.Add(new XElement("ReadingListFlag", Convert.ToInt32(options.ReadingListEnabled)));
            if (options.EmailFormat.HasValue) root.Add(new XElement("EmailFormat", options.EmailFormat));
            if (options.DeliveryOption.HasValue) root.Add(new XElement("DeliveryOption", options.DeliveryOption));
            if (options.EmailAddress.HasValue()) root.Add(new XElement("EmailAddress", options.EmailAddress));
            if (options.PhoneVoice1.HasValue()) root.Add(new XElement("PhoneVoice1", options.PhoneVoice1));
            if (options.NewPassword.HasValue()) root.Add(new XElement("Password", options.NewPassword));

            doc.Add(root);

            return doc.ToString();
        }
    }
}
