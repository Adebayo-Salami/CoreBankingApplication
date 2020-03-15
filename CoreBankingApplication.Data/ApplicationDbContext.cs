using CoreBankingApplication.Core.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Data
{
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
        public DbSet<AssignTill> assignTill { get; set; }
        public DbSet<TillAmount> tillAmount { get; set; }
        public DbSet<On_Us_Withdrawal> onUsWithdrawal { get; set; }

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