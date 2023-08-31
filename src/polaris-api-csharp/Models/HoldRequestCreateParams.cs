using System;

namespace Clc.Polaris.Api.Models
{
	/// <summary>
	/// Contains the parameters required to create a hold request.
	/// </summary>
	public class HoldRequestCreateParams
	{
		/// <summary>
		/// The patron's PatronID.
		/// </summary>
		public int PatronID { get; set; }

		/// <summary>
		/// The bibliographic record's BibliograhpicRecordID
		/// </summary>
		public int BibID { get; set; }

		/// <summary>
		/// The barcode of the item for a title level hold.
		/// </summary>
		public string ItemBarcode { get; set; }

		/// <summary>
		/// Volume of the hold request.
		/// </summary>
		public string VolumeNumber { get; set; }

		/// <summary>
		/// Serial designation of the hold request.
		/// </summary>
		public string Designation { get; set; }

		/// <summary>
		/// OrganizationID of the hold pickup location.
		/// </summary>
		public int PickupOrgID { get; set; } = 0;

		/// <summary>
		/// If the request is a Borrow by Mail request.
		/// </summary>
		public int IsBorrowByMail { get; set; }

		/// <summary>
		/// Notes created by patron.
		/// </summary>
		public string PatronNotes { get; set; }

		/// <summary>
		/// The date this hold request will become active.
		/// </summary>
		public DateTime? ActivationDate { get; set; }

		/// <summary>
		/// ID of the workstation where this hold request was created.
		/// </summary>
		public int WorkstationID { get; set; } = 1;

		/// <summary>
		/// ID of the Polaris user that created this request.
		/// </summary>
		public int UserID { get; set; } = 1;

		/// <summary>
		/// ID of branch where this hold request was created.
		/// </summary>
		public int RequestingOrgID { get; set; } = 1;

		/// <summary>
		/// GUID of search target. ONLY USED IF NOT LOCAL.
		/// </summary>
		public Guid? TargetGUID { get; set; }

		public HoldRequestCreateParams()
		{

		}

		public HoldRequestCreateParams(int patronId, int bibId, int pickupBranchId, int requestingOrgId)
		{
			PatronID = patronId;
			BibID = bibId;
			PickupOrgID = pickupBranchId;
			RequestingOrgID = requestingOrgId;
		}
	}
}