using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class PatronSavedSearchesGetResult : PapiResponseCommon
    {
        public Patronsavedsearchesgetrow[] PatronSavedSearchesGetRows { get; set; }
    }

    public class Patronsavedsearchesgetrow
    {
        public int SDISearchID { get; set; }
        public string SDIName { get; set; }
        public string SearchCriteria { get; set; }
        public string SearchPeriod { get; set; }
        public DateTime LastRunDate { get; set; }
        public bool NotifyOnNoResults { get; set; }
        public string EmailResultsTo { get; set; }
        public int ResultsCount { get; set; }

        public override string ToString() => $"{SearchPeriod} - {SDIName} - {SearchCriteria} - {LastRunDate.ToShortDateString()}";
    }

}
