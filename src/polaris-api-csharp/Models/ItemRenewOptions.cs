using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Options whenr renewing an item
    /// </summary>
    public class ItemRenewOptions
    {
        public string Action { get; set; } = "renew";
        /// <summary>
        /// The branch where the renewal takes place
        /// </summary>
        public int LogonBranchID { get; set; } = 1;

        /// <summary>
        /// The user performing the renewal
        /// </summary>
        public int LogonUserID { get; set; } = 1;

        /// <summary>
        /// The workstation the renewal takes place
        /// </summary>
        public int LogonWorkstationID { get; set; } = 1;

        public RenewData RenewData { get; set; } = new RenewData();

    }

    public class RenewData
    {
        /// <summary>
        /// Ignore ignorable errors and continue if possible
        /// </summary>
        public bool IgnoreOverrideErrors { get; set; } = true;
    }
}
