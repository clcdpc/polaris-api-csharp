using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class GetSubscriptionsByIDResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public List<GetSubscriptionsByIDRow> GetSubscriptionsByIDRows { get; set; }
    }

    public class GetSubscriptionsByIDRow
    {
        public int BibliographicRecordID { get; set; }
        public string SubscriptionXMLRecord { get; set; }
    }
}
