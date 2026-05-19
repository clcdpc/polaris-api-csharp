using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class ILLRequestCancelResult : PapiResponseCommon
    {
        public List<ILLRequestCancelRow> ILLRequestCancelRows { get; set; }
    }

    public class ILLRequestCancelRow
    {
        public int ILLRequestID { get; set; }
        public int ReturnCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
