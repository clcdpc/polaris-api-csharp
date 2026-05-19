using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class SAMobilePhoneCarriersGetResult : PapiResponseCommon
    {
        public List<SAMobilePhoneCarriersGetRow> SAMobilePhoneCarriersGetRows { get; set; }
    }

    public class SAMobilePhoneCarriersGetRow
    {
        public int CarrierID { get; set; }
        public string CarrierName { get; set; }
        public string Email2SMSEmailAddress { get; set; }
        public int NumberOfDigits { get; set; }
        public bool Display { get; set; }
    }
}
