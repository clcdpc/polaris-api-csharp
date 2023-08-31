using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class ItemUpdateBarcodeData
    {
        public int TransactionBranchId { get; set; }
        public string ItemBarcode { get; set; }
    }
}
