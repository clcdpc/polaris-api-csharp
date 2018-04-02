using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class PatronAuthenticationToken
    {
        public string Barcode { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
