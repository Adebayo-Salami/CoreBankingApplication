using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class TellerDetails
    {
        public int Id { get; set; }

        //[Display(Name = "User ID")]
        //public int CustomerID { get; set; }

        [Display(Name = "Teller Username")]
        public string tellerUsername { get; set; }

        [Display(Name = "Till Account Number")]
        public string tillAccountNumber { get; set; }

        [Display(Name = "Current Till Balance")]
        public double? tillBalance { get; set; }

        [Display(Name = "Amount To Add")]
        public double amountToAdd { get; set; }

        [Display(Name = "till Status")]
        public bool tillStatus { get; set; }
    }
}