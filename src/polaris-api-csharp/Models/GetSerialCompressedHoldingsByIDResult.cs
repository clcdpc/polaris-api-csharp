using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class GetSerialCompressedHoldingsByIDResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public List<GetSerialCompressedHoldingsRow> GetSerialCompressedHoldingsRows { get; set; }
    }
}
