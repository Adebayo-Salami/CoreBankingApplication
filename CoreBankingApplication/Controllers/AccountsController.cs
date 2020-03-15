using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using CoreBankingApplication.Core.Models;
using System.Web.Mvc;
using CoreBankingApplication.Data;

namespace CoreBankingApplication.Controllers
{
    [Authorize(Roles = RoleName.admin)]
    public class AccountsController : Controller
    {
        private CoreBankingApplication.Data.ApplicationDbContext _content;      //Creating an instance of the database connection.

        //Creating an instance of the database connection.
        public AccountsController()
        {
            _content = new CoreBankingApplication.Data.ApplicationDbContext(); 
        }

        //Disposing of connnection to the database
        protected override void Dispose(bool disposing)
        {
            _content.Dispose();
        }

        // GET: Accounts
        public ActionResult Index()
        {
            //var info = _content.acctIndex.Include(m => m.mainGLCategory).ToList();
            var acctIndex = _content.acctIndex.ToList();
            var mainGlCategory = _content.glCategory.ToList();
            var vm = new AccIndexer
            {
                type = mainGlCategory,
               AccountT = acctIndex
            };

            return View(vm);
        }

        //GET: To Create GL Category Account
        public ActionResult CreateSubCategory()
        {
            var gl = _content.glCategory.ToList();  //To get the list of GL Main Categories
            var VM = new AccIndexer         //Using View Model
            {
                SubCategory = new SubCategory(),
                type = gl
            };
            return View(VM);     //Returning the view
        }

        //POST: To Create GL Category Account
        [HttpPost]
        public ActionResult CreateCategoryGL(AccIndexer form)
        {
            if(!ModelState.IsValid)
            {
                return View("CreateSubCategory", form);     //Return View if model state is not valid with the form contents.
            }
            else
            {
                var checkName = _content.subCategory.SingleOrDefault(m => m.categoryName == form.SubCategory.categoryName);
                if(checkName != null)
                {
                    TempData["Error"] = "Sorry Category Name already exists in the database.";
                    return View("CreateSubCategory", form);
                }
                _content.subCategory.Add(form.SubCategory); //Add new GL Category to database.
                _content.SaveChanges(); //Save Changes to database.
                TempData["Success"] = "GL Category Created Successfully!";  //Display Success message to user.
                return RedirectToAction("ViewSubGL", "Accounts");    //To return view back to the index page.
            }
        }

        //GET: To Create GL Account
        public ActionResult CreateGLAccount()
        {
            var glCategory = _content.subCategory.ToList(); //To get the list of GL Sub Categories
            var VM = new AccIndexer         //Using View Model
            {
                AccountIndex = new AccountIndex(),
                SubCategoryT = glCategory
            };

            return View(VM);        //Returning the view
        }

        //POST: To Create GL Account
        [HttpPost]
        public ActionResult CreateGL(AccIndexer form)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateGLAccount", form);        //Return View if model state is not valid with the form contents.
            }
            else
            {
                var checkAcctName = _content.acctIndex.SingleOrDefault(m => m.accountName == form.AccountIndex.accountName);
                if(checkAcctName != null)
                {
                    TempData["Error"] = "Sorry account Name already exists in the database";
                    return View("CreateGLAccount", form);
                }
                var glCategory = _content.subCategory.SingleOrDefault(m => m.id == form.AccountIndex.SubCategoryid);    //Get Selected Sub GL Category details from the database.
                var mainGL = glCategory.glCategoryId;   //To get the Main GL category id.
                form.AccountIndex.mainGLCategory = mainGL;  //Save Main GL category id to GL Account Table.
                Random rn = new Random();   

                int unique = _content.acctIndex.Count();  
                int addUnique = unique + 1;     //Generate a unique number to make account code unique.
                int generaterTill = rn.Next(000, 999);  //Generate Random Numbers 
                string code = form.AccountIndex.mainGLCategory + "" + generaterTill + "" + addUnique;   //Creating account code for GL Account.
                form.AccountIndex.accountCode = code;

                form.AccountIndex.amountStatus = "NAIRA";
                form.AccountIndex.dateCreated = DateTime.Now;

                _content.acctIndex.Add(form.AccountIndex);
                _content.SaveChanges();
                TempData["Success"] = "GL Account Created Successfully!";   //Display Success message to user.
                return RedirectToAction("Index", "Accounts");   //Return view back to index page
            }
        }

        //GET: To Edit GL Account Details
        public ActionResult EditAcc(int id)
        {
            var editAccount = _content.acctIndex.SingleOrDefault(m => m.id == id);  //To get selected GL Account detai from database.
            if (editAccount == null)
            {
                return HttpNotFound();
            }
            else
            {
                var vm = new AccIndexer //Using view model
                {
                    type = _content.glCategory.ToList(),
                    AccountIndex = editAccount
                };

                return View(vm);
            }
        }

        //POST: To Edit GL Account Details
        [HttpPost]
        public ActionResult SaveGLUpdate(AccIndexer form)
        {
            if (ModelState.IsValid)
            {
                var glAcct = _content.acctIndex.SingleOrDefault(m => m.id == form.AccountIndex.id); //To load selected GL Account details
                if (glAcct == null) //To Verify GL Account exists.
                {
                    return View("EditAcc", form);
                }
                else
                {
                    //To Save GL Acccount Details to Database
                    glAcct.accountBranch = form.AccountIndex.accountBranch;    
                    glAcct.accountName = form.AccountIndex.accountName;
                    glAcct.accountDescription = form.AccountIndex.accountDescription;

                    _content.SaveChanges();

                    TempData["Success"] = "GL Account Updated Successfully!";   //To Display a success message to user
                    return RedirectToAction("Index", "Accounts");   //To return view back to the index page.
                }
            }
            else
            {
                TempData["Error"] = "Invalid Request";  //To Display error message to the user.
                return View("EditAcc", form);   
            }
        }

        //GET: To display GL Account Details
        public ActionResult GLDetails(int id)
        {
            var GLDetail = _content.acctIndex.SingleOrDefault(m => m.id == id);  //To Load the selected GL Account details page from the database.
            if (GLDetail == null)
            {
                return HttpNotFound();
            }
            else
            {
                var subGlCategory = _content.subCategory.SingleOrDefault(m => m.id == GLDetail.SubCategoryid);      //To load GL Sub Categories from the database.
                var vm = new AccIndexer //Using a view model
                {
                    subCategoryName = subGlCategory.categoryName,
                    AccountIndex = GLDetail
                };

                return View(vm);
            }
        }

        //GET: To display all till accounts
        public ActionResult tills()
        {
            return RedirectToAction("AcctLog", "Accounts", new { id = 20 });
        }

        //GET: To display GL Accounts Postings
        public ActionResult AcctLog(int id)
        {
            var acctN = _content.acctIndex.SingleOrDefault(m => m.id == id);    //To Load the selected GL Account details page from the database.
            var Silog = _content.SIlog.ToList();    //To Load all the postings in the savings interest GL Account from the database.
            var Cilog = _content.CIlog.ToList();    //To Load all the postings in the current interest GL Account from the database.
            var cotL = _content.cotI.ToList();      //To Load all the postings in the COT GL Account from the database.
            var loanInc = _content.loanI.ToList();  //To Load all the postings in the loans Income GL Account from the database.
            var Bilog = _content.bankL.ToList();    //To Load all the postings in the Bank's GL Account from the database.
            var loanAccrued = _content.loanAccrued.ToList();    //To Load all the customer loan interest accrued from the database.
            var loadGLTransactions = _content.postGL.ToList();  //To Load all the GL Transactions postings
            var tillLogs = _content.tillLog.ToList();           //To Load all till postings from the database

            if (acctN == null)
            {
                return HttpNotFound();
            }
            else
            {
                var acctName = acctN.accountName;   //To get the selected GL Account Name.
                var withLog = _content.postW.ToList();  //To load all the withdrawals log.
                var depoLog = _content.postD.ToList();  //To Load all the deposits log.
                var loanLog = _content.loanPay.ToList();    //To Load all the loan repayment log.
                var subTills = _content.tellerDetails.ToList(); //To Load all the teller posting log.

                var vm = new AccIndexer     //Using a view model
                {
                    tellerT = subTills, 
                    acctIndexN = acctName,
                    withT = withLog,
                    depoT = depoLog,
                    loanT = loanLog,
                    CIlogT = Cilog,
                    SIlogT = Silog,
                    cott = cotL,
                    Blog = Bilog,
                    loan = loanInc,
                    loanAccrualT = loanAccrued,
                    postGLT = loadGLTransactions,
                    tillLogs = tillLogs
                };

                //Returning Specific views for some specific requests
                if (id == 7)
                {
                    return View("SInterestLog", vm);
                }
                else if (id == 8)
                {
                    return View("CInterestLog", vm);
                }
                else if (id == 13)
                {
                    return View("BankLog", vm);
                }
                else if(id == 15)
                {
                    return View("COTincome", vm);
                }
                else if(id == 9)
                {
                    return View("LoanInc", vm);
                }
                else if(id == 20)
                {
                    return View("CashAssetsLog", vm);
                }
                else if(id == 23)
                {
                    return View("loanInterestAccrual", vm);
                }
                else if(id == 26)
                {
                    return View("tillLogs", vm);
                }
                else
                {
                    return View(vm);
                }

            }

        }

        //GET: To display GL Sub Categories
        public ActionResult ViewSubGL()
        {
            var subCategories = _content.subCategory.Include(m => m.glCategory).ToList();

            return View(subCategories);
        }

        //GET: To Edit GL Category
        public ActionResult EditSubL(int id)
        {
            var subCategory = _content.subCategory.SingleOrDefault(m => m.id == id);    //To load selected GL Category from the database.
            if(subCategory == null)
            {
                return HttpNotFound();
            }
            else
            {
                var mainGlCategory = _content.glCategory.ToList();      //To load GL Main Ccategories from the database.
                var vm = new AccIndexer    //Using view model.
                {
                    SubCategory = subCategory,
                    type = mainGlCategory
                };

                return View(vm);
            }
        }

        //POST: To Edit GL Category
        [HttpPost]
        public ActionResult saveGLCategory(AccIndexer form)
        {
            if(!ModelState.IsValid)
            {
                return View("EditSubL", form);      //Return View if model state is not valid with the form contents.
            }
            else
            {
                var selectedGL = _content.subCategory.SingleOrDefault(m => m.id == form.SubCategory.id);    //To Load selected GL Category from the database.
               
                //Saving GL Category updates to the database.
                selectedGL.categoryName = form.SubCategory.categoryName;    
                selectedGL.accountDescription = form.SubCategory.accountDescription;
                _content.SaveChanges();

                TempData["Success"] = "Update Successful";      //To Display Success message to the viewer
                return RedirectToAction("ViewSubGL", "Accounts");   //To return user back to the GL Category index view.
            }
        }

        //To display all on us withdrawals logs in the database
        public ActionResult viewOnUsWithdrawals()
        {
            var logs = _content.onUsWithdrawal.ToList();

            return View(logs);
        }
    }
}