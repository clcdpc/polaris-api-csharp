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
            if (options.AddressCheckDate != null) root.Add(new XElement("AddrCheckDate", options.AddressCheckDate.ToUniversalTime().ToString("R")));
            if (options.ExpirationDate != null) root.Add(new XElement("ExpirationDate", options.ExpirationDate.ToUniversalTime().ToString("R")));


            //var addressListElement = new XElement("PatronAddresses");

            //foreach (var address in options.Addresses.Take(1))
            //{
            //    var addressElement = new XElement("Address");
            //    if (address.AddressId > 0) addressElement.Add(new XElement("AddressID", address.AddressId));
            //    addressElement.Add(new XElement("FreeTextLabel", address.FreeTextLabel));
            //    addressElement.Add(new XElement("StreetOne", address.StreetOne));
            //    addressElement.Add(new XElement("StreetTwo", address.StreetTwo));
            //    addressElement.Add(new XElement("City", address.City));
            //    addressElement.Add(new XElement("State", address.State));
            //    addressElement.Add(new XElement("County", address.County));
            //    if (address.PostalCode > 0) addressElement.Add(new XElement("PostalCode", address.PostalCode));
            //    //addressElement.Add(new XElement("ZipPlusFour", address.ZipPlusFour));
            //    addressElement.Add(new XElement("Country", address.Country));
            //    if (address.CountryID > 0) addressElement.Add(new XElement("CountryID", address.CountryID));
            //    addressElement.Add(new XElement("AddressTypeID", address.AddressTypeID));
            //    addressListElement.Add(addressElement);
            //}

            //root.Add(addressListElement);

            doc.Add(root);

            return doc.ToString();
        }
    }
}
