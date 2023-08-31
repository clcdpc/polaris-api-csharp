using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class PatronPreferencesGetResult : PapiResponseCommon
    {
        public Patronpreferences PatronPreferences { get; set; }
    }

    public class Patronpreferences
    {
        public int PatronID { get; set; }
        public string Barcode { get; set; }
        public bool ReadingListEnabled { get; set; }
        public int DeliveryMethodID { get; set; }
        public string DeliveryMethodDescription { get; set; }
        public int DeliveryEmailFormatID { get; set; }
        public string DeliveryEmailFormatDescription { get; set; }
    }

}
