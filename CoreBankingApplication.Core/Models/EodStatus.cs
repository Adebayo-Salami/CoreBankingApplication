using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class EodStatus
    {
        public int id { get; set; }

        public bool status { get; set; }

        public int countDays { get; set; }
    }
}