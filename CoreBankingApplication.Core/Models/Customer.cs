using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class Customer
    {
        [Display(Name = "Customer ID")]
        public int id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Name must not be greater than 30 letters or less than 4 letters", MinimumLength = 4)]
        [Display(Name = "Full Name")]
        [RegularExpression(@"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$", ErrorMessage = "Pls No Special Characters or numbers allowed")]
        public string customerName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Pls Input Valid Phone Number", MinimumLength = 5)]
        [Display(Name = "Phone Number")]
        [Phone]
        public string customerPhone { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string customerGender { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string customerEmail { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "Name must not be less than 5 words or greater than 40 words.", MinimumLength = 5)]
        [Display(Name = "Location")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string customerLocation { get; set; }

        [StringLength(30, ErrorMessage = "Must not be greater than 30 words.")]
        [Display(Name = "National ID Card Details(Not Compulsory)")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string customerNationalId { get; set; }

        [StringLength(30, ErrorMessage = "Must not be greater than 30 words.")]
        [Display(Name = "Voters ID Card Details(Not Compulsory)")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string customerVoterId { get; set; }

        [StringLength(30, ErrorMessage = "Must not be greater than 30 words.")]
        [Display(Name = "Electricity Bill Details(Not Compulsory)")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string customerElectricityId { get; set; }

    }
}