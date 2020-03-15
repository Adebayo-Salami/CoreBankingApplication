using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CoreBankingApplication.Core.Models
{
    public class AccountIndex
    {
        public int id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Pls Must be more than 3 letters and less than 31 letters", MinimumLength = 4)]
        [Display(Name =  "GL Account Name")]
        //[RegularExpression(@"^[A-Za-z0-9 ]*$")] -- NUMBERS,SPACE,ALPHABETS
        //[RegularExpression(@"^([a-zA-Z]{2,}\s[a-zA-z]{1,}'?-?[a-zA-Z]{2,}\s?([a-zA-Z]{1,})?)", ErrorMessage = "Pls No Special Characters or numbers allowed")] must be two names or more
        //[RegularExpression(@"^[a-zA-Z][a-zA-Z\\S]+$", ErrorMessage = "Pls No Special Characters or numbers allowed")] only one name
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Pls No Special Characters or numbers allowed")]
        public string accountName { get; set; }
        
        public byte mainGLCategory { get; set; }

        [Display(Name = "Date Created")]
        public DateTime? dateCreated { get; set; }

        [Display(Name = "Account Code")]
        public string accountCode { get; set; }

        [Display(Name = "Amount in GL Account")]
        public double amountInAcct { get; set; }

        public string amountStatus { get; set; }

        [Required]
        [Display(Name = "Account Branch")]
        public string accountBranch { get; set; }

        [Required]
        [StringLength(80, ErrorMessage = "Pls fill in brief and accurate description about the account not more than 80 words.", MinimumLength = 5)]
        [Display(Name = "Account Description")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string accountDescription { get; set; }

        [Required]
        [Display(Name = "GL Category")]
        public int SubCategoryid { get; set; }
    }
}