using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public class TestSettings
    {
        public int PatronId { get; set; }
        public string PatronBarcode { get; set; }
        public string PatronPin { get; set; }
        public string FreeTextBlock { get; set; }
        public string PatronListName { get; set; }
        public string OrgEmail { get; set; }
    }
}
