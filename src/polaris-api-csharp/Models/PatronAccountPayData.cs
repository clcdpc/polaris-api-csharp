using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class PatronAccountPayData
    {
        public double TxnAmount { get; set; }
        public PaymentMethod PaymentMethodId { get; set; }
        public string FreeTextNote { get; set; }
    }
}
