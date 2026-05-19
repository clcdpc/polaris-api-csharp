using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class ILLRequestCreateData
    {
        public int PatronID { get; set; }
        public string VolumeNumber { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public string Edition { get; set; }
        public string PublicationDate { get; set; }
        public string ISBN { get; set; }
        public string ISSN { get; set; }
        public string LCCN { get; set; }
        public int ItemType { get; set; }
        public int MediumType { get; set; }
        public int PickupOrgID { get; set; }
        public int WorkstationID { get; set; }
        public int UserID { get; set; }
        public int HoldPickupAreaID { get; set; }
    }
}
