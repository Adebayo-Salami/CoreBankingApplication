using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Models
{
    public class CloseAcct
    {
        public int id { get; set; }

        public int CustomerID { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        public string customerAccountNumber { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string customerAccountType { get; set; }

        [Required]
        [Display(Name = "Account Name")]
        public string accountName { get; set; }

        [Required]
        [Display(Name = "Account Balance")]
        public double accountBalance { get; set; }

        [Required]
        [Display(Name = "Balance Entry")]
        public string balanceStatus { get; set; }

        [Display(Name = "Account Branch")]
        public string branchCreated { get; set; }

        [Display(Name = "Date Account Created")]
        public DateTime dateCreated { get; set; }

        [Required]
        [Display(Name = "Interest Rate(monthly)")]
        public double interestRate { get; set; }

        [Display(Name = "Minimum Balance")]
        public double lien { get; set; }

        [Display(Name = "COT")]
        public double cot { get; set; }

        public string linkedAcctNumber { get; set; }

        public string linkAcctType { get; set; }

        public int months { get; set; }

        public bool loanStatus { get; set; }
    }
}