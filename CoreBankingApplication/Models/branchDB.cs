using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Models
{
    public class branchDB
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "Branch Name")]
        [StringLength(30, ErrorMessage = "Pls Must be more than 3 letters and less than 31 letters", MinimumLength = 4)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Pls No Special Characters or numbers allowed")]
        public string branchName { get; set; }

        [Required]
        [Display(Name = "Branch Location")]
        [StringLength(50, ErrorMessage = "Pls Must not be more than 50 words")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string branchLoc { get; set; }
    }
}