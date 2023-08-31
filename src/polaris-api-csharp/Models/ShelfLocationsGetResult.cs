using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class ShelfLocationsGetResult : PapiResponseCommon
    {
        public Shelflocationsrow[] ShelfLocationsRows { get; set; }
    }

    public class Shelflocationsrow
    {
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public string Description { get; set; }

        public override string ToString() => $"{ID} - {OrganizationID} - {Description}";
    }
}
