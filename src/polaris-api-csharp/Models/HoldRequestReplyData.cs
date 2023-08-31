using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class HoldRequestReplyData
    {
        public string TxnGroupQualifier { get; set; }
        public string TxnQualifier { get; set; }
        public int RequestingOrgID { get; set; }
        public int Answer { get; set; }
        public int State { get; set; }
    }
}
