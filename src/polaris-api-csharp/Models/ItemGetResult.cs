using System;
using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class ItemGetResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public List<ItemGetRow> ItemGetRows { get; set; }
    }

    public class ItemGetRow
    {
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public int CollectionID { get; set; }
        public string CollectionName { get; set; }
        public string Barcode { get; set; }
        public string PublicNote { get; set; }
        public string CallNumber { get; set; }
        public string Designation { get; set; }
        public string VolumeNumber { get; set; }
        public string ShelfLocation { get; set; }
        public string CircStatus { get; set; }
        public string LastCircDate { get; set; }
        public string MaterialType { get; set; }
        public string TextualHoldingsNote { get; set; }
        public string RetentionStatement { get; set; }
        public string HoldingsStatement { get; set; }
        public string HoldingsNote { get; set; }
        public bool Holdable { get; set; }
        public string DueDate { get; set; }
        public int ItemRecordID { get; set; }
        public int BibliographicRecordID { get; set; }
        public bool IsDisplayInPAC { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? FirstAvailableDate { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}
