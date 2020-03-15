using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class LoanInterestAccrual
    { 
        public int id { get; set; }

        [Display(Name = "Customer Account Name")]
        public string customerAcctName { get; set; }

        [Display(Name = "Customer Account Number")]
        public string customerAcctNo { get; set; }

        [Display(Name = "Linked Account Type")]
        public string linkedAcctName { get; set; }

        [Display(Name = "Linked Account Number")]
        public string linkedAcctNo { get; set; }

        [Display(Name = "Interest Accrual")]
        public double accruedAmt { get; set; }

        [Display(Name = "Interst Accrual Rate")]
        public double interestRate { get; set; }

        [Display(Name = "Total Loan Amount")]
        public double loanAmount { get; set; }

        [Display(Name = "Date Loan was taken")]
        public DateTime dateOfLoan { get; set; }

        [Display(Name = "Loan Duration(Months)")]
        public int duration { get; set; }
    }
}