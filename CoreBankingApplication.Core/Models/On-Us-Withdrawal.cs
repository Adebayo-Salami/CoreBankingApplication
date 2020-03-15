using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class On_Us_Withdrawal
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(30, ErrorMessage = "Pls Must be more than 3 letters and less than 31 letters", MinimumLength = 4)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Pls No Special Characters or numbers allowed")]
        public string Name { get; set; }

        [Required]
        public int terminal_ID { get; set; }

        [Required]
        [Display(Name = "Location")]
        [StringLength(50, ErrorMessage = "Pls Must not be more than 50 words")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string location { get; set; }
    }
}