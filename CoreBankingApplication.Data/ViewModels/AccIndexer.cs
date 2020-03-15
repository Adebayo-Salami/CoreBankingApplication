using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoreBankingApplication.Core.Models;
using System.Data.Entity;

namespace CoreBankingApplication.Data
{
    public class AccIndexer
    {
        private ApplicationDbContext _content;

        public AccIndexer()
        {
            _content = new ApplicationDbContext();
            uRolee = _content.uRole.ToList();
            branc = _content.branches.ToList();
            var EStat = _content.eod.SingleOrDefault(m => m.id == 1);
            eodStatus = EStat.status;
            var subCategory = _content.subCategory.ToList();
            SubCategoryT = subCategory;
            selectAccountT = _content.selectAcct.ToList();
            //AccountT = _content.acctIndex.Where(m => m.id != 20).ToList();
            AccountTF = _content.acctIndex.Where(g => g.mainGLCategory == 3).ToList();
            type = _content.glCategory.ToList();
            status = _content.stat.ToList();
            userT = _content.Users.Where(m => m.tillAccount == String.Empty).ToList();
            tillT = _content.tellerDetails.Where(m => m.tillStatus == false).ToList();
            tillAmounT = _content.tillAmount.ToList();

            var allGLs = _content.acctIndex.Where(m => m.id != 20).ToList();
            var allTills = _content.tellerDetails.ToList();
            List<AccountIndex> gls = new List<AccountIndex>();
            foreach (var gl in allGLs)
            {
                gls.Add(new AccountIndex()
                {
                    accountName = gl.accountName,
                    id = gl.id
                });
            }

            foreach (var gl in allTills)
            {
                gls.Add(new AccountIndex()
                {
                    accountName = gl.tellerUsername,
                    id = gl.Id
                });
            }
            AccountT = gls;
        }

        public IEnumerable<GlCategory> type { get; set; }
        public AccountIndex AccountIndex { get; set; }  
        public Customer Customer { get; set; }    
        public CustomerAccounts CustomerAccounts { get; set; }
        public string accountNumber { get; set; }
        public IEnumerable<CustomerAccounts> CustomerAT { get; set; }
        public IEnumerable<AccountIndex> AccountT { get; set; }
        public IEnumerable<AccountIndex> AccountTF { get; set; }
        public IEnumerable<Customer> CustomerT { get; set; }
        public IEnumerable<ApplicationUser> userT { get; set; }
        public IEnumerable<TellerDetails> tillT { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<status> status { get; set; }
        public IEnumerable<TillAmount> tillAmounT { get; set; }
        public SelectTillsAmount SelectTillsAmount { get; set; }
        public PostWithdrawal PostWithdrawal { get; set; }
        public PostDeposit PostDeposit { get; set; }
        public loanPayment loanPayment { get; set; }
        public IEnumerable<TellerDetails> tellerT { get; set; }
        public AssignTill AssignTill { get; set; }
        public double tillB { get; set; }
        public double liabilityB { get; set; }
        public double assetB { get; set; }
        public double incomeB { get; set; }
        public double expenseB { get; set; }
        public double PLBal { get; set; }
        public double PLMinusBal { get; set; }
        public double debit { get; set; }
        public double netAsset { get; set; }
        public double capitalBal { get; set; }
        public double netBal { get; set; }
        public double netMinusBal { get; set; }
        public double incBal { get; set; }
        public double interestPay { get; set; }
        public double netMinusAsset { get; set; }
        public double balanceFig { get; set; }
        public double negativeFig { get; set; }
        public double credit { get; set; }
        public string acctIndexN { get; set; }
        public RegisterViewModel reg { get; set; }
        public IEnumerable<userRoles> uRolee { get; set; }
        public IEnumerable<branchDB> branc { get; set; }
        public IEnumerable<PostWithdrawal> withT { get; set; }
        public IEnumerable<PostDeposit> depoT { get; set; }
        public IEnumerable<loanPayment> loanT { get; set; }
        public IEnumerable<CurrentInterestLog> CIlogT { get; set; }
        public IEnumerable<SavingsInterestLog> SIlogT { get; set; }
        public IEnumerable<BankLog> Blog { get; set; }
        public bool eodStatus { get; set; }
        public IEnumerable<CotIncomeGL> cott { get; set; }
        public int profileKey { get; set; }
        public string profileName { get; set; }
        public IEnumerable<loanIncome> loan { get; set; }
        public SubCategory SubCategory { get; set; }
        public IEnumerable<SubCategory> SubCategoryT { get; set; }
        public string subCategoryName { get; set; }
        public IEnumerable<selectAccount> selectAccountT { get; set; }
        public PostGLTransactions PostGLTransactions { get; set; }
        public IEnumerable<LoanInterestAccrual> loanAccrualT { get; set; }
        public FundCapitalGL FundCapitalGL { get; set; }
        public IEnumerable<PostGLTransactions> postGLT { get; set; }
        public IEnumerable<FundCapitalGL> fundGLT { get; set; }
        public IEnumerable<TillLog> tillLogs { get; set; }
      
    }
}