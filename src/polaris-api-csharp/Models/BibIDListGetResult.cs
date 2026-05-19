using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class BibIDListGetResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public List<BibIDListRow> BibIDListRows { get; set; }
    }

    public class BibIDListRow
    {
        public int BibliographicRecordID { get; set; }
    }
}
