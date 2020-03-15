using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Models
{
    public class AccountTypeConfig
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string accountType { get; set; }

        [Required]
        [Range(0.000001, 10)]
        [Display(Name = "Interest Rate")]
        public double interestRate { get; set; }

        [Required]
        [Display(Name = "Lien")]
        public double lien { get; set; }
    }
}