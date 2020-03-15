using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class userRoles
    {
        public int id { get; set; }

        [Display(Name = "User Role")]
        public string role { get; set; }
    }
}