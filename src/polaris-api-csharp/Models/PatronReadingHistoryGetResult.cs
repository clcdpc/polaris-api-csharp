using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class PatronReadingHistoryGetResult : PapiResponseCommon
    {
        public PatronReadingHistoryGetRow[] PatronReadingHistoryGetRows { get; set; }

        public override string ToString() => string.Join("\r\n", PatronReadingHistoryGetRows.Select(r => r));
    }

    public class PatronReadingHistoryGetRow
    {
        public int ItemID { get; set; }
        public string Barcode { get; set; }
        public int BibID { get; set; }
        public int FormatID { get; set; }
        public string FormatDescription { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string CallNumber { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int LoaningBranchID { get; set; }
        public string LoaningBranchName { get; set; }
        public int PatronReadingHistoryID { get; set; }

        public override string ToString() => $"{Barcode} - {Title} - {FormatDescription} - {CheckOutDate.ToShortDateString()}";
    }

}
