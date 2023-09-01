using System;
using System.Collections.Generic;
using System.Linq;

namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// The result of a PatronBasicDataGet API call.
    /// </summary>
    public class PatronBasicDataGetResult : PapiResponseCommon
    {
        /// <summary>
        /// Patron information for the supplied patron.
        /// </summary>
        public PatronData PatronBasicData { get; set; }

        public override string ToString()
        {
            if (PatronBasicData?.PatronID == 0) return base.ToString();
            return $"{PatronBasicData.PatronID} - {PatronBasicData.Barcode} - {PatronBasicData.NameFirst} {PatronBasicData.NameLast}";
        }
    }

    /// <summary>
    /// Information about a patron.
    /// </summary>
    public class PatronData
    {
        public int PatronID { get; set; }
        public string Barcode { get; set; }
        public string NameFirst { get; set; }
        public string NameLast { get; set; }
        public string NameMiddle { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int ItemsOutCount { get; set; }
        public int ItemsOverdueCount { get; set; }
        public int ItemsOutLostCount { get; set; }
        public int HoldRequestsTotalCount { get; set; }
        public int HoldRequestsCurrentCount { get; set; }
        public int HoldRequestsShippedCount { get; set; }
        public int HoldRequestsHeldCount { get; set; }
        public int HoldRequestsUnclaimedCount { get; set; }
        public double ChargeBalance { get; set; }
        public double CreditBalance { get; set; }
        public double DepositBalance { get; set; }
        public string NameTitle { get; set; }
        public string NameSuffix { get; set; }
        public string PhoneNumber2 { get; set; }
        public string PhoneNumber3 { get; set; }
        public int Phone1CarrierID { get; set; }
        public int Phone2CarrierID { get; set; }
        public int Phone3CarrierID { get; set; }
        public string CellPhone { get; set; }
        public int CellPhoneCarrierID { get; set; }
        public string AltEmailAddress { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? AddrCheckDate { get; set; }
        public int MessageNewCount { get; set; }
        public int MessageReadCount { get; set; }
        public int PatronOrgID { get; set; }
        public int PatronCodeID { get; set; }
        public int DeliveryOptionID { get; set; }
        public bool ExcludeFromAlmostOverdueAutoRenew { get; set; }
        public bool ExcludeFromPatronRecExpiration { get; set; }
        public bool ExcludeFromInactivePatron { get; set; }
        public int EReceiptOptionID { get; set; }
        public int TxtPhoneNumber { get; set; }
        public int EmailFormatID { get; set; }
        public List<PatronAddress> PatronAddresses { get; set; }
        public string User1 { get; set;}
        public string User2 { get; set; }
        public string User3 { get; set; }
        public string User4 { get; set; }
        public string User5 { get; set; }

        string fixpn(string pn) => new string(pn.Where(c => char.IsDigit(c)).ToArray());

        public string TxtDeliveryPhoneNumber => TxtPhoneNumber == 1 ? fixpn(PhoneNumber) : TxtPhoneNumber == 2 ? fixpn(PhoneNumber2) : TxtPhoneNumber == 3 ? fixpn(PhoneNumber3) : "";

        public string DeliveryString
        {
            get
            {
                switch (DeliveryOptionID)
                {
                    case 1:
                        if (!PatronAddresses.Any()) { return ""; }
                        var address = PatronAddresses.First();
                        var street = !string.IsNullOrWhiteSpace(address.StreetTwo) ? $"{address.StreetOne} {address.StreetTwo}" : address.StreetOne;
                        return $"{street} {address.City}, {address.State} {address.PostalCode}";
                    case 2:
                        return EmailAddress;
                    case 3:
                        return fixpn(PhoneNumber);
                    case 4:
                        return fixpn(PhoneNumber2);
                    case 5:
                        return fixpn(PhoneNumber3);
                    case 8:
                        return TxtDeliveryPhoneNumber;
                }

                return "";
            }
        }
    }
}