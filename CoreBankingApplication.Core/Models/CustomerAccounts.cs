using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class CustomerAccounts
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Customer ID")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Pls only numbers are allowed")]
        public int CustomerID { get; set; }

        [Display(Name = "Account Number")]
        [StringLength(20, ErrorMessage = "Pls Must be greater than 20 words")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string customerAccountNumber { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string customerAccountType { get; set; }

        //[Required]
        [Display(Name = "Account Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Pls No Special Characters or numbers allowed")]
        public string accountName { get; set; }
       
        [Required]
        [Display(Name = "Amount(Account Balance)")]
        [Range(0, 9999999999999)]
        public double accountBalance { get; set; }

        [Display(Name = "Balance Entry")]
        public string balanceStatus { get; set; }

        [Required]
        [Display(Name = "Account Branch")]
        public string branchCreated { get; set; }

        [Display(Name = "Date Account Created")]
        public DateTime dateCreated { get; set; }

        [Required]
        [Display(Name = "Interest Rate(monthly)%")]
        public double interestRate { get; set; }

        [Display(Name = "Minimum Balance(lien)")]
        [Range(0, 99999999)]
        public double lien { get; set; }

        [Display(Name = "COT(specify for current account only)")]
        public double cot { get; set; }

        [Display(Name = "Link Account Number")]
        [StringLength(20, ErrorMessage = "Pls Must be greater than 20 words")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string linkedAcctNumber { get; set; }

        [Display(Name = "Link Account Type")]
        public string linkAcctType { get; set; }

        [Display(Name = "Duration(months)")]
        [Range(0, 999)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Pls only numbers are allowed")]
        public int months { get; set; }

        public bool loanStatus { get; set; }
    }

}