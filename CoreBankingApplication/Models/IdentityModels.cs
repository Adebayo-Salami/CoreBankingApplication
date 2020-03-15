using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreBankingApplication.Models;
using CoreBankingApplication.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace CoreBankingApplication.Models
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

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<GlCategory> glCategory { get; set; }
        public DbSet<TellerDetails> tellerDetails { get; set; }
        public DbSet<AccountIndex> acctIndex { get; set; }
        public DbSet<Customer> custom { get; set; }
        public DbSet<CustomerAccounts> customAccts { get; set; }
        public DbSet<status> stat { get; set; }
        public DbSet<PostWithdrawal> postW { get; set; }
        public DbSet<PostDeposit> postD { get; set; }
        public DbSet<CloseAcct> closedAcct { get; set; }
        public DbSet<loanPayment> loanPay { get; set; }
        public DbSet<EodStatus> eod { get; set; }
        public DbSet<userRoles> uRole { get; set; }
        public DbSet<branchDB> branches { get; set; }
        public DbSet<SavingsInterestLog> SIlog { get; set; }
        public DbSet<CurrentInterestLog> CIlog { get; set; }
        public DbSet<BankLog> bankL { get; set; }
        public DbSet<CotIncomeGL> cotI { get; set; }
        public DbSet<loanIncome> loanI { get; set; }
        public DbSet<SubCategory> subCategory { get; set; }
        public DbSet<selectAccount> selectAcct { get; set; }
        public DbSet<TillLog> tillLog { get; set; }
        public DbSet<PostGLTransactions> postGL { get; set; }
        public DbSet<LoanInterestAccrual> loanAccrued { get; set; }
        public DbSet<FundCapitalGL> fundGL { get; set; }
        public DbSet<AccountTypeConfig> accountConfig { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {          
            return new ApplicationDbContext();
        }

    }
}