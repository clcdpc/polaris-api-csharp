using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class ApiResult : PapiResponseCommon
    {
        public string Version { get; set; }

        public override string ToString() => Version;
    }
}
