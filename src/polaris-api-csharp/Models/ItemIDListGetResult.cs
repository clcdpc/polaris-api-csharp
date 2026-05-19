using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class ItemIDListGetResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public List<ItemIDListRow> ItemIDListRows { get; set; }
    }

    public class ItemIDListRow
    {
        public int ItemRecordID { get; set; }
    }
}
