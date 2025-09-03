using System.Collections.Generic;
using System;

namespace Clc.Polaris.Models
{
    public class PatronRegistrationData
    {
        // Required
        public int LogonBranchID { get; set; } = 1;
        public int LogonUserID { get; set; } = 1;
        public int LogonWorkstationID { get; set; } = 1;

        // Optional elements
        public bool? ReadingListFlag { get; set; }             // "No" in table – optional
        public int? EmailFormat { get; set; }                 // 1=Plain,2=HTML
        public int? DeliveryOptionID { get; set; }
        public string EmailAddress { get; set; }
        public string AltEmailAddress { get; set; }
        public bool? EnableSMS { get; set; }
        public string PhoneVoice1 { get; set; }
        public string PhoneVoice2 { get; set; }
        public string PhoneVoice3 { get; set; }
        public int? Phone1CarrierID { get; set; }
        public int? Phone2CarrierID { get; set; }
        public int? Phone3CarrierID { get; set; }
        public int? TxtPhoneNumber { get; set; }
        public int? EReceiptOptionID { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public string PatronCode { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? AddrCheckDate { get; set; }
        public bool? ExcludeFromAlmostOverdueAutoRenew { get; set; }
        public bool? ExcludeFromPatronRecExpiration { get; set; }
        public bool? ExcludeFromInactivePatron { get; set; }
        public int? RequestPickupBranchID { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public string User3 { get; set; }
        public string User4 { get; set; }
        public string User5 { get; set; }
        public int? LanguageID { get; set; }
        public string FormerID { get; set; }
        public string NameFirst { get; set; }
        public string NameLast { get; set; }
        public string NameMiddle { get; set; }
        public string LegalNameFirst { get; set; }
        public string LegalNameLast { get; set; }
        public string LegalNameMiddle { get; set; }
        public bool? UseLegalNameOnNotices { get; set; }
        public DateTime? Birthdate { get; set; }
        public int? GenderID { get; set; }
        public string UserName { get; set; }
        public string Barcode { get; set; }
        public int? PatronBranchID { get; set; }
        public int? StatisticalClassID { get; set; }
        public bool? UseSingleName { get; set; }

        public List<PatronRegistrationAddressData> Addresses { get; set; }

        public class PatronRegistrationAddressData
        {
            public string FreeTextLabel { get; set; }
            public string StreetOne { get; set; }
            public string StreetTwo { get; set; }
            public string StreetThree { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string County { get; set; }
            public string PostalCode { get; set; }
            public string ZipPlusFour { get; set; }
            public string Country { get; set; }
            public int? CountryID { get; set; }
            public int? AddressTypeID { get; set; }
        }

        public PatronRegistrationData()
        {
            Addresses = new List<PatronRegistrationAddressData>();
        }
    }
}