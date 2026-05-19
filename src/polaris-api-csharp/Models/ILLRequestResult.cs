using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class ILLRequestResult : PapiResponseCommon
    {
        public string RequestGUID { get; set; }
        public string TxnGroupQualifer { get; set; }
        public string TxnQualifier { get; set; }
        public int StatusType { get; set; }
        public int StatusValue { get; set; }
        public string Message { get; set; }
    }
}
