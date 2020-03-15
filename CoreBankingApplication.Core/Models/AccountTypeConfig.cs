using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class AccountTypeConfig
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string accountType { get; set; }

        [Required]
        [Display(Name = "Interest Rate(%)")]
        public double interestRate { get; set; }

        [Required]
        [Display(Name = "Lien")]
        public double lien { get; set; }
    }
}