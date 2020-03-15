using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class PostDeposit
    {
        [Display(Name = "Transaction ID")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Customer Account Number")]
        [StringLength(20, ErrorMessage = "Pls Must be greater than 20 words")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string accountNumber { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string accountType { get; set; }

        [Required]
        [Display(Name = "Amount To Deposit")]
        [Range(0, 9999999999999)]
        public double amountD { get; set; }

        [Display(Name = "Account To Credit")]
        public string accountToCredit { get; set; }

        [Display(Name = "Account Name")]
        public string accountName { get; set; } 

        [Required]
        [Display(Name = "Transaction Description")]
        [StringLength(40, ErrorMessage = "Pls Must not be more than 40 words")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string transactionDescription { get; set; }

        //[Required]
        [Display(Name = "Teller's Till Account Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Pls only numbers are allowed")]
        public string tillAccount { get; set; }

        [Display(Name = "Date of Withdrawal")]
        public DateTime date { get; set; }
    }
}