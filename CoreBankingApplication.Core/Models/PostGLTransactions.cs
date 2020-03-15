using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class PostGLTransactions
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "GL Account To Debit")]
        public string glSending { get; set; }
        
        [Required]
        [Display(Name = "GL Account To Credit")]
        public string glReceiving { get; set; }

        [Required]
        [Display(Name = "Debit Amount")]
        [Range(0, 9999999999999)]
        public double amountDebit { get; set; }

        [Required]
        [Display(Name = "Credit Amount")]
        [Range(0, 9999999999999)]
        public double amountCredit { get; set; }

        [Required]
        [Display(Name = "Narration")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string narration { get; set; }

        [Display(Name = "Date of Transaction")]
        public DateTime dateOfTransaction { get; set; }

        public string glSendingAccountCode { get; set; }

        public string glReceivingAccountCode { get; set; }
    }
}