using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using CoreBankingApplication.Core.Models;
using CoreBankingApplication.Logic;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using CoreBankingApplication.Data;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace CoreBankingApplication.Controllers
{
    [Authorize(Roles = RoleName.admin)]
    public class UsersController : Controller
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private CoreBankingApplication.Data.ApplicationDbContext _content;  //Creating an instance of the database connection.

        //Creating an instance of the database connection.
        public UsersController()
        {
            _content = new CoreBankingApplication.Data.ApplicationDbContext();
        }

        //Disposing of connnection to the database
        protected override void Dispose(bool disposing)
        {
            _content.Dispose();
        }

        // GET: Users
        public ActionResult Index()
        {
            var _users = _content.Users.ToList();

            var vm = new AccIndexer
            {
                userT = _users
            };

            return View(vm);
        }

        //GET: Tellers
        public ActionResult tellher()
        {
            return RedirectToAction("Index", "Tellers");
        }


        //GET: To updtae user's details
        public ActionResult editU(string id)
        {
            var userD = _content.Users.SingleOrDefault(m => m.Id == id);    //To load selected user details from the database

            if(userD == null)
            {
                return HttpNotFound();
            }
            else
            {
                var vm = new AccIndexer     //using view model
                {
                    ApplicationUser = userD

                };

                return View(vm);
            }
        }

        //POST: To update user's details
        [HttpPost]
        public ActionResult saveProfile(AccIndexer form)
        {
            if(!ModelState.IsValid)
            {
                return View("editU", form);
            }
            else
            {
                var profile = _content.Users.SingleOrDefault(m => m.Id.ToString() == form.ApplicationUser.Id.ToString());   //To load selected user details from the database.
                if(profile == null)     //To verify that profile exist
                {
                    TempData["Error"] = "User Profile Does not exist.";     //To display an error message to the user
                    return View("editU", form);
                }
                else
                {
                    //To update user details
                    profile.Branch = form.ApplicationUser.Branch;
                    profile.homeAddress = form.ApplicationUser.homeAddress;
                    profile.fullName = form.ApplicationUser.fullName;
                    profile.Email = form.ApplicationUser.Email;
                    profile.PhoneNumber = form.ApplicationUser.PhoneNumber;

                    _content.SaveChanges();     //To save update changes to database
                }
            }
            TempData["Success"] = "Update Successful";
            return RedirectToAction("Index", "Users");
        }

        //GET: To create new user
        public ActionResult createher()
        {
            return RedirectToAction("register", "Account");
        }

        //GET: To add new branch
        public ActionResult brancher()
        {
            return View();
        }

        //POST: To add new branch
        [HttpPost]
        public ActionResult saveBranch(branchDB form)
        {
            if(!ModelState.IsValid)
            {
                return View("brancher", form);
            }
            else
            {
                _content.branches.Add(form);
            }
            _content.SaveChanges();

            TempData["Success"] = "Branch Added Successfully";
            return RedirectToAction("viewBranches", "Users");
        }

        //GET: To assign till account
        public ActionResult assignT()
        {
            var users = _content.Users.Where(m => m.tillAccount == String.Empty).ToList();        //Get all the users from the database
            var tills = _content.tellerDetails.Where(m => m.tillStatus == false).ToList();   //Get all the till accounts from the database

            var vm = new AccIndexer
            {
                userT = users,
                tillT = tills,
                AssignTill = new AssignTill()
            };

            return View(vm);
        }

        //POST: To assign till account
        [HttpPost]
        public ActionResult assignTellerTill(AccIndexer form)
        {
            if(!ModelState.IsValid)
            {
                return View("assignT", form);
            }

            //To Fetch Till Account and User Profile Details
            var tillAccount = _content.tellerDetails.SingleOrDefault(m => m.tellerUsername == form.AssignTill.tillAccount);
            var profile = _content.Users.SingleOrDefault(m => m.UserName == form.AssignTill.profile);

            //To Assign Teller Role to the user
            var roleStore = new RoleStore<IdentityRole>(new CoreBankingApplication.Data.ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            roleManager.CreateAsync(new IdentityRole("Teller"));
            UserManager.AddToRole(profile.Id, "Teller");

            //To Assign Till Account to the user
            profile.tillAccount = tillAccount.tillAccountNumber;
            profile.role = "Teller";
            tillAccount.tellerUsername = profile.UserName;
            tillAccount.tillStatus = true;
            _content.SaveChanges();
            TempData["Success"] = "Till Account Assigned Successfully";
            return RedirectToAction("Index", "Users");
        }

        //GET: To generate till accounts
        public ActionResult generateTill()
        {
            var vm = new AccIndexer
            {
                SelectTillsAmount = new SelectTillsAmount()
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult generateTills(AccIndexer form)
        {
            if(!ModelState.IsValid)
            {
                return View("generateTill", form);
            }
            var number = form.SelectTillsAmount.number;
            var glCategory = _content.acctIndex.SingleOrDefault(m => m.id == 20);       //To get cash asset GL Account info 

            for (int i = 0; i < number; i ++)
            {
                var till = new TillGenerate().GetTillCode(glCategory.accountCode);         //To generate till account code for till

                var count = _content.tellerDetails.Count();
                //To log till account details for the newly created till account
                var stud = new TellerDetails
                {
                    tillAccountNumber = till.ToString(),
                    tillBalance = 0,
                    tellerUsername = "UNASSIGNED TILL" + (count + 1) + ""
                };

                _content.tellerDetails.Add(stud);
                _content.SaveChanges();
            }

            TempData["Success"] = "New Till Account(s) Successfully Created.";        //To display a success message to the user
            return RedirectToAction("AcctLog", "Accounts", new { id = 20 });
        }

        //GET: To get list of branches
        public ActionResult viewBranches()
        {
            var getBranches = _content.branches.ToList();
            return View(getBranches);
        }

    }
}