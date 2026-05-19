using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class GetAuthsByIDResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public List<GetAuthsByIDRow> GetAuthsByIDRows { get; set; }
    }

    public class GetAuthsByIDRow
    {
        public int AuthorityRecordID { get; set; }
        public string AuthorityRecordXML { get; set; }
    }
}
