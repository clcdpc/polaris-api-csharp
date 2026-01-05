using System;

namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Patron notes information.
    /// </summary>
    public struct PatronNotes
    {
        /// <summary>
        /// ID of branch where the non-blocking note was created.
        /// </summary>
        public int? NonBlockingBranchId { get; set; }

        /// <summary>
        /// Name of branch where the non-blocking note was created.
        /// </summary>
        public string NonBlockOrgName { get; set; }

        /// <summary>
        /// ID of the Polaris user that created the non-blocking note.
        /// </summary>
        public int? NonBlockingUserId { get; set; }

        /// <summary>
        /// Username of the Polaris user that created the non-blocking note.
        /// </summary>
        public string NonBlockUsrName { get; set; }

        /// <summary>
        /// ID of the workstation where the non-blocking note was created.
        /// </summary>
        public int? NonBlockingWorkStationID { get; set; }

        /// <summary>
        /// Display name of the workstation where the non-blocking note was created.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// ID of branch where the blocking note was created.
        /// </summary>
        public int? BlockingBranchID { get; set; }

        /// <summary>
        /// Name of branch where the blocking note was created.
        /// </summary>
        public string BlockingOrgName { get; set; }

        /// <summary>
        /// ID of the Polaris user that created the blocking note.
        /// </summary>
        public int? BlockingUserID { get; set; }

        /// <summary>
        /// Username of the Polaris user that created the blocking note.
        /// </summary>
        public string BlockingUsrName { get; set; }

        /// <summary>
        /// ID of the workstation where the non-blocking note was created.
        /// </summary>
        public int? BlockingWorkstationID { get; set; }

        /// <summary>
        /// Display name of the workstation where the blocking note was created.
        /// </summary>
        public string BlockingWorkstationDisplayName { get; set; }

        /// <summary>
        /// Non-blocking notes
        /// </summary>
        public string NonBlockingStatusNotes { get; set; }

        /// <summary>
        /// Date the non-blocking note were created.
        /// </summary>
        public DateTime? NonBlockingStatusNoteDate { get; set; }

        /// <summary>
        /// Blocking notes
        /// </summary>
        public string BlockingStatusNotes { get; set; }

        /// <summary>
        /// Date the blocking note were created.
        /// </summary>
        public DateTime? BlockingStatusNoteDate { get; set; }
    }
}
