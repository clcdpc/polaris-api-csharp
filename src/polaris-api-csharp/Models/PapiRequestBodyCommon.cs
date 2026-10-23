namespace Clc.Polaris.Api.Models
{
    public class PapiRequestBodyCommon
    {
        /// <summary>
        /// Branch where the notification is being updates
        /// </summary>
        public int? LogonBranchId { get; set; }

        /// <summary>
        /// User updating the notification
        /// </summary>
        public int? LogonUserId { get; set; }

        /// <summary>
        /// Workstation the notification is being updated on
        /// </summary>
        public int? LogonWorkstationId { get; set; }
    }
}
