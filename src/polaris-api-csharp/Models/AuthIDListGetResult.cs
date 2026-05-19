using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class AuthIDListGetResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public List<AuthIDListRow> AuthIDListRows { get; set; }
    }

    public class AuthIDListRow
    {
        public int AuthorityRecordID { get; set; }
    }
}
