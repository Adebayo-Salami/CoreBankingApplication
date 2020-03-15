using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace CoreBankingApplication.Models
{
    public class loanPayment
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "Customer Loan Account Number")]
        [StringLength(20, ErrorMessage = "Pls Must be greater than 20 words")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string accountNumber { get; set; }

        [Display(Name = "Customer Account Number")]
        [StringLength(20, ErrorMessage = "Pls Must be greater than 20 words")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string customerAccountNumber { get; set; }

        //[Required]
        [Display(Name = "Account Type")]
        public string accountType { get; set; }

        [Required]
        [Display(Name = "Amount To Pay")]
        [Range(0, 9999999999999)]
        public double amountP { get; set; }

        //[Required]
        [Display(Name = "Account With Corresponding Entry")]
        public string accountToCredit { get; set; }

        [Required]
        [Display(Name = "Payment Description")]
        [StringLength(40, ErrorMessage = "Pls Must not be more than 40 words")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string transactionDescription { get; set; }

        [Required]
        [Display(Name = "Teller's Till Account Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Pls only numbers are allowed")]
        [StringLength(20, ErrorMessage = "Pls Must be greater than 20 words")]
        public string tillAccount { get; set; }

        [Display(Name = "Date of Deposit")]
        public DateTime date { get; set; }
    }
}