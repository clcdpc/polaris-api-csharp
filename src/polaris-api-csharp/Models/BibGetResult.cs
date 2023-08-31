using System.Collections.Generic;
using System.Linq;

namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Contains a list of elements representing data contained in the MARC record.
    /// </summary>
    public class BibGetResult : PapiResponseCommon
    {
        /// <summary>
        /// List of rows containing raw bibliographic record information.
        /// </summary>
        public List<BibGetRow> BibGetRows { get; set; }

        /// <summary>
        /// Publisher(s) of the record.
        /// </summary>
        public List<string> Publisher => GetBibResultRow(2);
        /// <summary>
        /// Description(s) of the record.
        /// </summary>
        public List<string> Description => GetBibResultRow(3);
        /// <summary>
        /// Edition(s) of the record.
        /// </summary>
        public List<string> Edition => GetBibResultRow(5);
        /// <summary>
        /// ISBN of the record.
        /// </summary>
        public string ISBN => GetBibResultRow(6).FirstOrDefault();

        /// <summary>
        /// Number of items associated with this record system-wide.
        /// </summary>
        public int? SystemItemsTotal => int.TryParse(GetBibResultRow(7).FirstOrDefault() + "", out int dummy) ? dummy : new int?();

        /// <summary>
        /// Current number of holds on this record.
        /// </summary>
        public int? CurrentHolds => int.TryParse(GetBibResultRow(8).FirstOrDefault() + "", out int dummy) ? dummy : new int?();

        /// <summary>
        /// Summary(ies) of this record.
        /// </summary>
        public List<string> Summary => GetBibResultRow(9);
        /// <summary>
        /// Number of local items associated with this record.
        /// </summary>
        public List<string> LocalItemsTotal => GetBibResultRow(10);
        /// <summary>
        /// Control number of this record.
        /// </summary>
        public int? ControlNumber => int.TryParse(GetBibResultRow(11).FirstOrDefault() + "", out int dummy) ? dummy : new int?();

        /// <summary>
        /// Call number(s) of this record.
        /// </summary>
        public List<string> CallNumber => GetBibResultRow(13);
        /// <summary>
        /// Number of local items available that are associated with this record.
        /// </summary>
        public int? LocalItemsAvailable => int.TryParse(GetBibResultRow(15).FirstOrDefault() + "", out int dummy) ? dummy : new int?();

        /// <summary>
        /// Number of available items associated with this record system-wide.
        /// </summary>
        public int? SystemItemsAvailable => int.TryParse(GetBibResultRow(16).FirstOrDefault() + "", out int dummy) ? dummy : new int?();

        /// <summary>
        /// Format of the record.
        /// </summary>
        public string Format => GetBibResultRow(17).FirstOrDefault();

        /// <summary>
        /// Author of the record.
        /// </summary>
        public List<string> Author => GetBibResultRow(18);

        /// <summary>
        /// Series of the record.
        /// </summary>
        public List<string> Series => GetBibResultRow(19);

        /// <summary>
        /// Subject of the record.
        /// </summary>
        public List<string> Subject => GetBibResultRow(20);

        /// <summary>
        /// Added author of the record.
        /// </summary>
        public List<string> AddedAuthor => GetBibResultRow(21);

        /// <summary>
        /// Added title of the record.
        /// </summary>
        public List<string> AddedTitle => GetBibResultRow(22);

        /// <summary>
        /// LCCN of the record.
        /// </summary>
        public string LCCN => GetBibResultRow(23).FirstOrDefault();

        /// <summary>
        /// ISSN of the record.
        /// </summary>
        public string ISSN => GetBibResultRow(24).FirstOrDefault();

        /// <summary>
        /// Other number of the record.
        /// </summary>
        public string OtherNumber => GetBibResultRow(25).FirstOrDefault();

        /// <summary>
        /// Genre of the title.
        /// </summary>
        public List<string> Genre => GetBibResultRow(27);

        /// <summary>
        /// Notes on the record.
        /// </summary>
        public List<string> Notes => GetBibResultRow(28);

        /// <summary>
        /// Contents of the record.
        /// </summary>
        public List<string> Contents => GetBibResultRow(29);

        /// <summary>
        /// Publisher number of the record.
        /// </summary>
        public List<string> PublisherNumber => GetBibResultRow(30);

        /// <summary>
        /// Web link of the record.
        /// </summary>
        public List<string> WebLink => GetBibResultRow(33);

        /// <summary>
        /// Uniform title of the record.
        /// </summary>
        public string UniformTitle => GetBibResultRow(34).FirstOrDefault();

        /// <summary>
        /// Title of the record.
        /// </summary>
        public string Title => GetBibResultRow(35).FirstOrDefault();

        /// <summary>
        /// Volume of the record.
        /// </summary>
        public string Volume => GetBibResultRow(36).FirstOrDefault();

        /// <summary>
        /// Frequency of the record.
        /// </summary>
        public string Frequency => GetBibResultRow(37).FirstOrDefault();

        /// <summary>
        /// Former title of the record.
        /// </summary>
        public List<string> FormerTitle => GetBibResultRow(39);

        /// <summary>
        /// Later title of the record.
        /// </summary>
        public List<string> LaterTitle => GetBibResultRow(40);

        /// <summary>
        /// STRN of the record.
        /// </summary>
        public List<string> STRN => GetBibResultRow(41);
        public List<string> UPC => GetBibResultRow(48);

        /// <summary>
        /// GPO item number of the record.
        /// </summary>
        public List<string> GPOItemNumber => GetBibResultRow(42);

        /// <summary>
        /// CODEN of the record.
        /// </summary>
        public List<string> CODEN => GetBibResultRow(43);

        /// <summary>
        /// Target audience of the record.
        /// </summary>
        public List<string> TargetAudience => GetBibResultRow(44);

        /// <summary>
        /// Medium of the record.
        /// </summary>
        public string Medium => GetBibResultRow(46).FirstOrDefault();

        private List<string> GetBibResultRow(int id)
        {
            if (BibGetRows.Any(b => b.ElementID == id))
            {
                return BibGetRows.Where(b => b.ElementID == id).Select(b => b.Value).ToList();
            }
            return new List<string>();
        }
    }


    /// <summary>
    /// Contains a the value of a bibliographic record field.
    /// </summary>
    public class BibGetRow
    {
        /// <summary>
        /// The element identifier.
        /// </summary>
        public int ElementID { get; set; }

        /// <summary>
        /// The occurrence of a given element matches the relative order of the underlying data in the MARC record. 
        /// </summary>
        public int Occurrence { get; set; }

        /// <summary>
        /// The label associated with this element.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The value of the element.
        /// </summary>
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Label} - {Value}";
        }
    }
}