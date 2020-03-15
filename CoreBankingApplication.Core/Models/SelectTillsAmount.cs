using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class SelectTillsAmount
    {
        public int id { get; set; }

        [Display(Name = "Select Number of Tills")]
        public int number { get; set; }
    }
}