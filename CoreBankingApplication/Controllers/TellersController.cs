using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using CoreBankingApplication.Core.Models;
using System.Web.Mvc;

namespace CoreBankingApplication.Controllers
{
    [Authorize(Roles = RoleName.admin)]
    public class TellersController : Controller
    {
        private CoreBankingApplication.Data.ApplicationDbContext _content;      //Creating an instance of the database connection.

        //Creating an instance of the database connection.
        public TellersController()
        {
            _content = new CoreBankingApplication.Data.ApplicationDbContext();
        }

        //Disposing of connnection to the database
        protected override void Dispose(bool disposing)
        {
            _content.Dispose();
        }

        // GET: Tellers
        public ActionResult Index()
        {
            var tellersInfo = _content.Users.ToList();      //To load all tellers from the database
            return View(tellersInfo);
        }

        //GET: To display teller details
        public ActionResult Details(string id)
        {
            var tellerDetails = _content.tellerDetails.SingleOrDefault(m => m.tillAccountNumber == id);     //To load selected teller details from the database.
            if(tellerDetails == null)   
            {
                return RedirectToAction("Index", "Tellers");
            }
            else
            {
              return View(tellerDetails);
            }
        }

        //GET: To Credit teller's till account
        public ActionResult EditTill(int id)
        {
            var tillDetails = _content.tellerDetails.SingleOrDefault(m => m.Id == id);      //To load selected till account details from the database
            if(tillDetails == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(tillDetails);
            }
        }

        //POST: To Credit teller's till account
        [HttpPost]
        public ActionResult UpdateTill(TellerDetails dt)
        {
            if(!ModelState.IsValid)
            {
                return View("EditTill", dt);
            }
            if(dt.amountToAdd <= 0)
            {
                TempData["Error"] = "Amount to add must not be negative or zero";   //To display an error message to the user.
                return View("EditTill", dt);
            }
            var tellerInDb = _content.tellerDetails.SingleOrDefault(m => m.Id == dt.Id);    //To fetch till details from the database.
            if(tellerInDb == null)
            {
                return HttpNotFound();
            }
            else
            {
                var vaultAccountBal = _content.acctIndex.SingleOrDefault(m => m.id == 26);  //To get vault's current account details
                if(dt.amountToAdd >= vaultAccountBal.amountInAcct)      //Verify that there is enough fund in bank's account to fund teller's till accounts
                {
                    TempData["Error"] = "Insufficient funds in the vault account";      //Display error message to the user
                    return View("EditTill", dt);
                }
                vaultAccountBal.amountInAcct = vaultAccountBal.amountInAcct - dt.amountToAdd;   //Debit Bank's Account
                tellerInDb.tillBalance = tellerInDb.tillBalance + dt.amountToAdd;   //Credit Teller till's account.

                //Log transaction  into database.s
                TillLog till = new TillLog();
                till.accountEntry = "Credit";
                till.accountSending = vaultAccountBal.accountName;
                till.accountRecieving = dt.tellerUsername + "(" + dt.tillAccountNumber + ")";
                till.amount = dt.amountToAdd;
                till.dateOfTransaction = DateTime.Now;
                _content.tillLog.Add(till);

                var accountIndex = _content.acctIndex.SingleOrDefault(m => m.id == 20);
                accountIndex.amountInAcct = accountIndex.amountInAcct + dt.amountToAdd;
            }
            _content.SaveChanges();
            TempData["Success"] = "Till Account Credited Successfully"; //To display success message to the user
            return RedirectToAction("Details", "Tellers", new { id = dt.tillAccountNumber });
        }

        //GET: Edit teller's details
        public ActionResult Edit(int id)
        {
            var editTeller = _content.Users.SingleOrDefault(m => m.tillAccount == id.ToString());

            if(editTeller == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View("EditDetails", editTeller);
            }
        }

        //POST: Edit teller's details
        [HttpPost]
        public ActionResult EditUpdate(ApplicationUser ui)
        {
            var editId = _content.Users.SingleOrDefault(m => m.UserName == ui.UserName);    //To fetch selected teller details from the database
            string check = editId.ToString();
            if(check == null || check == String.Empty)
            {
                return HttpNotFound();
            }
            else
            {
                //To update teller details
                editId.fullName = ui.fullName;
                editId.Branch = ui.Branch;
                editId.Email = ui.Email;
                editId.PhoneNumber = ui.PhoneNumber;
                editId.homeAddress = ui.homeAddress;

                _content.SaveChanges();     //To save update of teller details
                TempData["Success"] = "Update Successful";
                return RedirectToAction("Index", "Tellers");
            }
        }
    }
}