using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Models
{
    public class loanIncome
    {
        public int id { get; set; }

        [Display(Name = "Customer Account Name")]
        public string accountName { get; set; }

        [Display(Name = "Customer Account Number")]
        public string acctNo { get; set; }

        [Display(Name = "Income From Loan")]
        public string incomeG { get; set; }

        [Display(Name = "Date Issued")]
        public DateTime date { get; set; }

        [Display(Name = "Loan Duration")]
        public string duration { get; set; }

        [Display(Name = "Interest Rate of loan")]
        public string rate { get; set; }

        [Display(Name = "Amount Borrowed")]
        public string amountBorrowed { get; set; }

        [Display(Name = "Daily Accrued Income")]
        public double accruedInc { get; set; }
    }
}