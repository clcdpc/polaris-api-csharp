using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Models
{
    public class PapiRequestBodyCommon
    {
        /// <summary>
        /// Branch where the notification is being updates
        /// </summary>
        public int LogonBranchId { get; set; } = 1;

        /// <summary>
        /// User updating the notification
        /// </summary>
        public int LogonUserId { get; set; } = 1;

        /// <summary>
        /// Workstation the notification is being updated on
        /// </summary>
        public int LogonWorkstationId { get; set; } = 1;
    }
}
