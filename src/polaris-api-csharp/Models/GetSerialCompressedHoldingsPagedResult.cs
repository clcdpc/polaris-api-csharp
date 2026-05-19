using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class GetSerialCompressedHoldingsPagedResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public int LastOrganizationID { get; set; }
        public int LastBibliographicRecordID { get; set; }
        public List<GetSerialCompressedHoldingsRow> GetSerialCompressedHoldingsRows { get; set; }
    }
}
