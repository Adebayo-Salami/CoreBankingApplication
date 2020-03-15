using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Models
{
    public class CotIncomeGL
    {
        public int id { get; set; }

        [Display(Name = "Account Entry")]
        public string acctEntry { get; set; }

        [Display(Name = "Customer Account Number")]
        public string customerAccNumb { get; set; }

        [Display(Name = "Customer Account Type")]
        public string customerAcctype { get; set; }

        [Display(Name = "Amount")]
        public double amt { get; set; }

        [Display(Name = "Description")]
        public string Entrydesc { get; set; }

        [Display(Name = "Date of Transaction")]
        public DateTime dateTr { get; set; }

        public double previousAmt { get; set; }
    }
}