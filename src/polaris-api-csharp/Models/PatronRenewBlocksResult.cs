using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class PatronRenewBlocksResult : PapiResponseCommon
    {
        public PatronRenewBlock[] Blocks { get; set; }
        public bool CanPatronRenew { get; set; }
    }

    public class PatronRenewBlock
    {
        public int PatronId { get; set; }
        public string PatronName { get; set; }
        public string BlockDescription { get; set; }
    }
}
