using System;
using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class BibReplacementIDGetResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public List<BibReplacementIDRow> BibReplacementIDRows { get; set; }
    }

    public class BibReplacementIDRow
    {
        public int OriginalBibRecordID { get; set; }
        public int NewBibliographicRecordID { get; set; }
        public DateTime ReplacementDate { get; set; }
    }
}
