using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class RemoteStorageItemsGetResult
    {
        public int PAPIErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public int RecordCount { get; set; }
        public int RowCount { get; set; }
        public RemoteStorageItemsGetRow[] RemoteStorageItemsGetRows { get; set; }
    }

    public class RemoteStorageItemsGetRow
    {
        public int ItemRecordID { get; set; }
        public int BibliographicRecordID { get; set; }
        public string Barcode { get; set; }
        public string BrowseTitle { get; set; }
        public object BrowseAuthor { get; set; }
        public int MaterialTypeID { get; set; }
        public string MaterialType { get; set; }
        public int CollectionID { get; set; }
        public string Collection { get; set; }
        public int ShelfLocationID { get; set; }
        public string ShelfLocation { get; set; }
        public string CallNumber { get; set; }
        public object CopyNumber { get; set; }
        public object VolumeNumber { get; set; }
    }
}
