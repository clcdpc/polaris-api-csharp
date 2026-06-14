using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class HoldRequestActivationData
    {
        public int UserID { get; set; }
        public DateTime ActivationDate { get; set; }

        public HoldRequestActivationData()
        {
            
        }

        public HoldRequestActivationData(int userId, DateTime activationDate)
        {
            UserID = userId;
            ActivationDate = activationDate;
        }
    }
}
