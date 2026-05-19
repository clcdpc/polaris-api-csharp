using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class PickupAreasGetResult : PapiResponseCommon
    {
        public List<PickupAreasRow> PickupAreasRows { get; set; }
    }

    public class PickupAreasRow
    {
        public int OrganizationID { get; set; }
        public int PickupAreaID { get; set; }
        public string Description { get; set; }
        public int SequenceID { get; set; }
        public bool Selected { get; set; }
    }
}
