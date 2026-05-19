using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class GetBibResourceCountsByIDResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public List<GetBibResourceCountsByIDRow> GetBibResourceCountsByIDRows { get; set; }
    }

    public class GetBibResourceCountsByIDRow
    {
        public int BibliographicRecordID { get; set; }
        public int HoldRequestsCount { get; set; }
        public int ItemRecordsCount { get; set; }
    }
}
