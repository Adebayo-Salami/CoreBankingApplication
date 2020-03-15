using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class AssignTill
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "User Profile")]
        public string profile { get; set; }

        [Required]
        [Display(Name = "Available Tills")]
        public string tillAccount { get; set; }
    }
}