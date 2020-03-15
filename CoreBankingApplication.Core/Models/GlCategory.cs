using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Core.Models
{
    public class GlCategory
    {
        [Key]
        public byte id { get; set; }

        [Display(Name = "GL Account Category")]
        public string type { get; set; }
    }
}