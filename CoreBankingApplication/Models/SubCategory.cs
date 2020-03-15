using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Models
{
    public class SubCategory
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "GL Category Name")]
        [StringLength(30, ErrorMessage = "Pls Must be more than 3 letters and less than 31 letters", MinimumLength = 4)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Pls No Special Characters or numbers allowed")]
        public string categoryName { get; set; }

        public GlCategory glCategory { get; set; }

        [Required]
        [Display(Name = "GL Main Category")]
        public byte glCategoryId { get; set; }

        [Required]
        [StringLength(80, ErrorMessage = "Pls fill in brief and accurate description about the account not more than 80 words.", MinimumLength = 5)]
        [Display(Name = "Category Description")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string accountDescription { get; set; }
    }
}