using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class MultipartGetResult : PapiResponseCommon
    {
        public MultipartRow[] MultipartRows { get; set; }
    }

    public class MultipartRow
    {
        public int Type { get; set; }
        public string Value { get; set; }
        public string ItemBarcode { get; set; }
    }
}
