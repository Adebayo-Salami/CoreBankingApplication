using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreBankingApplication.Core.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace CoreBankingApplication.Core.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [EmailAddress]
        public override string Email { get; set; }

        [Phone]
        public override string PhoneNumber { get; set; }

        [Display(Name = "Branch Name")]
        public string Branch { get; set; }

        [Display(Name = "Till Account Number")]
        public string tillAccount { get; set; }

        [Display(Name = "Home Address")]
        [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Pls No Special Characters Allowed")]
        public string homeAddress { get; set; }

        [Display(Name = "Full Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Pls No Special Characters or numbers allowed")]
        public string fullName { get; set; }

        [Display(Name = "User Role")]
        public string role { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}