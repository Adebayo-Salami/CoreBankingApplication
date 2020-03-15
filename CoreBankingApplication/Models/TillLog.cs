using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Models
{
    public class TillLog
    {
        public int id { get; set; }

        public string accountEntry { get; set; }

        public string accountSending { get; set; }

        public double amount { get; set; }

        public DateTime dateOfTransaction { get; set; }
    }
}