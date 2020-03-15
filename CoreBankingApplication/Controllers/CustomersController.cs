using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using CoreBankingApplication.Core.Models;
using CoreBankingApplication.Logic;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using CoreBankingApplication.Data;

namespace CoreBankingApplication.Controllers
{
    [Authorize(Roles = RoleName.teller + "," + RoleName.admin)]
    public class CustomersController : Controller
    {
        private CoreBankingApplication.Data.ApplicationDbContext _content;  //Creating an instance of the database connection.

        //Creating an instance of the database connection.
        public CustomersController()
        {
            _content = new CoreBankingApplication.Data.ApplicationDbContext();
        }

        //Disposing of connnection to the database
        protected override void Dispose(bool disposing)
        {
            _content.Dispose();
        }


        // GET: Customers
        public ActionResult Index()
        {
            var customers = _content.custom.ToList();
            return View(customers);
        }

        //GET: To Create Customer Profile
        public ActionResult CreateProfile()
        {
            var gender = _content.stat.ToList();    //To Load gender list from the database.

            var vm = new AccIndexer //Using View Model
            {
                status = gender,
                Customer = new Customer()
            };

            return View(vm);    //Returning view to the profile create page
        }

        //POST: To Create Customer Profile
        [HttpPost]
        public ActionResult CreateProfilee(AccIndexer form)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Error Creating Customer Profile,pls check inputs and try again.";  //Displaying Error message to the user.
                return RedirectToAction("CreateProfile", "Customers");
            }
            else
            {
                var checkName = _content.custom.SingleOrDefault(m => m.customerName == form.Customer.customerName);
                if(checkName != null)
                {
                    TempData["Error"] = "Customer Name Already exists in the database.";    //Displaying Error message to the user.
                    return View("CreateProfile", form);
                }
                var checkEmail = _content.custom.SingleOrDefault(m => m.customerEmail == form.Customer.customerEmail);  //To check that customer email doesn't already exist in the database.
                if(checkEmail != null)
                {
                    TempData["Error"] = "Customer Email Already exists in the database, pls use another email.";    //Displaying Error message to the user.
                    return View("CreateProfile", form);
                }
                //To add new customer profile to the database
                _content.custom.Add(form.Customer);
                _content.SaveChanges();
                TempData["Success"] = "Customer Profile Created Successfully";   //Displaying Success message to the user.
                return RedirectToAction("Index", "Customers");
            }
        }

        //GET: To Edit Customer Profile
        public ActionResult EditProfile(int id)
        {
            var customer = _content.custom.SingleOrDefault(m => m.id == id);
            if(customer == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(customer);
            }
        }

        //POST: To Edit Customer Profile
        [HttpPost]
        public ActionResult InsertProfile(Customer form)
        {
            if(!ModelState.IsValid)
            {
                return View("EditProfile", form);
            }
            else
            {
                var customerInDB = _content.custom.SingleOrDefault(m => m.id == form.id);   //To load selected customer profile from the database.

                //To save customer profile updates to the database.
                var customerAccts = _content.customAccts.ToList();
                foreach(var item in customerAccts)
                {
                    if(item.CustomerID == customerInDB.id)
                    {
                        item.accountName = customerInDB.customerName;
                        _content.SaveChanges();
                    }
                }
                customerInDB.customerName = form.customerName;
                customerInDB.customerPhone = form.customerPhone;
                customerInDB.customerEmail = form.customerEmail;
                customerInDB.customerLocation = form.customerLocation;
                customerInDB.customerNationalId = form.customerNationalId;
                customerInDB.customerVoterId = form.customerVoterId;
                customerInDB.customerElectricityId = form.customerElectricityId;

                _content.SaveChanges();
                TempData["Success"] = "Update Successful";  //To display success message to the user.
                return RedirectToAction("Index", "Customers");
            }
        }

        //GET: To display all Customer Accounts
        public ActionResult CustomerAccounts()
        {
            var accountDetails = _content.customAccts.ToList();
         
            return View(accountDetails);
        }

        //GET: To assign account to customer profile
        public ActionResult assignAccount()
        {
            var acctType = _content.acctIndex.ToList();     //To get all available account from the database.
            var customType = _content.custom.ToList();      //To get all customer profiles from the database.
            var userT = _content.Users.ToList();            //To get all users from the database.
            var statT = _content.stat.ToList();             //To get currency choice from the database.

            var vm = new AccIndexer //Using view model
            {
                CustomerAccounts = new CustomerAccounts(),
                AccountT = acctType,
                CustomerT = customType,
                userT = userT,
                status = statT
            };

            return View(vm);
        }

        //POST: To assign account to customer profile
        [HttpPost]
        public ActionResult CreateAccount(AccIndexer form)
        {
            if(!ModelState.IsValid)
            {
                return View("assignAccount", form);
            }
            if(form.CustomerAccounts.accountBalance < 0)
            {
                TempData["Error"] = "Account Balance can't be below zero";      //To Display Error message to the user.
                return View("assignAccount", form);
            }
            if(form.CustomerAccounts.interestRate < 1 || form.CustomerAccounts.interestRate > 100)
            {
                TempData["Error"] = "Interest Rate Must Be between 1 to 100";      //To Display Error message to the user.
                return View("assignAccount", form);
            }
            form.CustomerAccounts.cot = 5;        //Assign cot percentage o customer current account configuration
            var verufyProfile = _content.custom.SingleOrDefault(m => m.id == form.CustomerAccounts.CustomerID);      //To get customer profile from the database.
            if(verufyProfile == null)   //To verify that customer profile exists.
            {
                TempData["Error"] = "Customer Profile Does not exist in the database";      //To Display Error message to the user.
                return View("assignAccount", form);
            }
            else
            {
                form.CustomerAccounts.accountName = verufyProfile.customerName;
                if (form.CustomerAccounts.linkAcctType != null || form.CustomerAccounts.linkedAcctNumber != null)       //To check if account is linked to the current account being created.
                {
                    var verifyLink = _content.customAccts.SingleOrDefault(m => m.customerAccountType == form.CustomerAccounts.linkAcctType && m.customerAccountNumber == form.CustomerAccounts.linkedAcctNumber && m.CustomerID == form.CustomerAccounts.CustomerID);       //Get Linked Account details from the database.
                    if (verifyLink == null)     //To verify that linked account exist
                    {
                        TempData["Error"] = "Customer Linked Account Does not exist.";      //To Display Error message to the user.
                        return View("CreateLoanAccount", form);
                    }
                    else
                    {
                        var checkForLoan = _content.customAccts.SingleOrDefault(m => m.CustomerID == form.CustomerAccounts.CustomerID && m.accountName == form.CustomerAccounts.accountName && m.customerAccountType == "Phoenix Loan Account");    //To check if customer already has an active loan account.
                        if(checkForLoan != null)       //To verify customer does not have an active loan account.
                        {
                            TempData["Error"] = "Customer Already has a loan account";      //To Display Error message to the user.
                            return View("assignAccount", form);
                        }
                        if (form.CustomerAccounts.customerAccountType == "Phoenix Loan Account")
                        {
                            if(form.CustomerAccounts.months > 0)
                            {
                                var bankAcct = _content.acctIndex.SingleOrDefault(m => m.id == 13);     //To load bank's account details from the database.
                                if(bankAcct.amountInAcct <= form.CustomerAccounts.accountBalance)       //To check if bank has enough money to give out as loan.
                                {
                                    TempData["Error"] = "Bank's Account Too Low to process Loan";       //To Display Error message to the user.
                                    return View("assignAccount", form);
                                }
                                if (form.CustomerAccounts.accountBalance < 2000)
                                {
                                    TempData["Error"] = "Not allowed to give loan lower than 2000 nair";      //To Display Error message to the user.
                                    return View("assignAccount", form);
                                }

                                //Getting Account Configuration
                                var interestRate = (form.CustomerAccounts.interestRate / 100);
                                var amount = form.CustomerAccounts.accountBalance;
                                var duration = form.CustomerAccounts.months;
                                var realAmt = form.CustomerAccounts.accountBalance;

                                //Calculating Interest Rate / Income from customer loan account
                                var calcIncome = (interestRate * realAmt) * duration;
                                var totalSum = calcIncome + realAmt;

                                verifyLink.accountBalance = verifyLink.accountBalance + realAmt;    //Crediting customer's linked account with the amount loaned
                                bankAcct.amountInAcct = bankAcct.amountInAcct - realAmt;        //Debiting bank's account with the amount loaned

                                var acctype = _content.acctIndex.SingleOrDefault(m => m.id == 12);
                                var accountN = new AccountFunction().AccountNumber(acctype.accountCode, verufyProfile.id);    //Generating account number for customer's loan account

                                //Saving Customers loan account and configurations to database.
                                form.CustomerAccounts.customerAccountNumber = accountN;
                                form.CustomerAccounts.accountBalance = (double)totalSum;
                                form.CustomerAccounts.dateCreated = DateTime.Now;
                                form.CustomerAccounts.balanceStatus = "NAIRA";
                                form.CustomerAccounts.loanStatus = true;
                                _content.customAccts.Add(form.CustomerAccounts);

                                //Creating Customer loan interest account to keep track of the interest accrual 
                                LoanInterestAccrual loanAccured = new LoanInterestAccrual();
                                loanAccured.customerAcctName = form.CustomerAccounts.accountName;
                                loanAccured.customerAcctNo = form.CustomerAccounts.customerAccountNumber;
                                loanAccured.linkedAcctName = form.CustomerAccounts.linkAcctType;
                                loanAccured.linkedAcctNo = form.CustomerAccounts.linkedAcctNumber;
                                loanAccured.accruedAmt = 0.0;
                                loanAccured.duration = form.CustomerAccounts.months;
                                loanAccured.dateOfLoan = DateTime.Now;
                                loanAccured.interestRate = interestRate / 30;
                                loanAccured.loanAmount = totalSum;
                                _content.loanAccrued.Add(loanAccured);

                                //Creating Customer loan income account to keep track of the amount gained as ncome from customer loan
                                loanIncome loan = new loanIncome();
                                loan.accountName = form.CustomerAccounts.accountName;
                                loan.acctNo = form.CustomerAccounts.customerAccountNumber;
                                loan.amountBorrowed = realAmt.ToString();
                                loan.incomeG = calcIncome.ToString();
                                loan.accruedInc = 0.0;
                                loan.date = DateTime.Now;
                                loan.duration = form.CustomerAccounts.months.ToString();
                                loan.rate = interestRate.ToString();
                                _content.loanI.Add(loan);

                                var acctLoan = _content.acctIndex.SingleOrDefault(m => m.id == 9);
                                acctLoan.amountInAcct = acctLoan.amountInAcct + totalSum;     //Crediting Customer Loan Account.

                                var linkedAcct = _content.acctIndex.SingleOrDefault(m => m.accountName == verifyLink.accountName);
                                if(linkedAcct != null)
                                {
                                    linkedAcct.amountInAcct = linkedAcct.amountInAcct + realAmt;
                                }

                                _content.SaveChanges();    //Saving all changes to the database
                            }
                            else
                            {
                                TempData["Error"] = "Pls Specify Loan Duration";        //To Display Error message to the user.
                                return View("CreateLoanAccount", form);
                            }
                        }
                    }
                }
                else if(form.CustomerAccounts.linkAcctType == null && form.CustomerAccounts.linkedAcctNumber == null)
                {
                    if(form.CustomerAccounts.customerAccountType == "Phoenix Loan Account")
                    {
                        TempData["Error"] = "Pls Specify Customer Linked Accounts";     //To Display error message to the user.
                        return View("CreateLoanAccount", form); 
                    }
                    var acctype = _content.acctIndex.SingleOrDefault(m => m.accountName == form.CustomerAccounts.customerAccountType);
                    //Generating Account Number for th customer account
                    var accountN = new AccountFunction().AccountNumber(acctype.accountCode, verufyProfile.id);     //Assigning Account Number to the customer account

                    //Saving Customer account and configurations to the database
                    form.CustomerAccounts.customerAccountNumber = accountN;
                    form.CustomerAccounts.accountBalance = (double)form.CustomerAccounts.accountBalance;
                    form.CustomerAccounts.dateCreated = DateTime.Now;
                    form.CustomerAccounts.balanceStatus = "NAIRA";
                    acctype.amountInAcct = acctype.amountInAcct + form.CustomerAccounts.accountBalance;
                    form.CustomerAccounts.loanStatus = true;
                    form.CustomerAccounts.interestRate = form.CustomerAccounts.interestRate / 100;
                    _content.customAccts.Add(form.CustomerAccounts);

                    _content.SaveChanges();     //Saving Changes to the database.
                }
               
            }

            TempData["Success"] = "Account Created Successfully";       //To Display Success message to the user.
            return RedirectToAction("CustomerAccounts", "Customers");
        }

        //GET: To display customer account details
        public ActionResult AcctDetails(int id)
        {
            var acct = _content.customAccts.SingleOrDefault(m => m.id == id);
            if(acct == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(acct);
            }
        }

        //To close customer account 
        public ActionResult closeAcct(int id)
        {
            var info = _content.customAccts.SingleOrDefault(m => m.id == id);   //Load customer account from the database
            if(info == null)        //Verify that customer account exists.
            {
                return HttpNotFound();
            }
            else
            {
                var checkForLoan = _content.closedAcct.SingleOrDefault(m => m.CustomerID == info.CustomerID && m.accountName == info.accountName && m.customerAccountType == "Phoenix Loan Account");  //if its a loan account, verify the customer doesn't have another loan account in the databse.
                if (checkForLoan != null)   //Verify the customer doesn't have anothe loan account in the database if its a loan account.
                {
                    TempData["Error"] = "Customer Already has a closed loan account, pls delete one";       //Display error message to the user.
                    return RedirectToAction("CustomerAccounts", "Customers");
                }

                //To CLose customer account
                CloseAcct info1 = new CloseAcct();
                info1.customerAccountNumber = info.customerAccountNumber;
                info1.customerAccountType = info.customerAccountType;
                info1.accountName = info.accountName;
                info1.accountBalance = info.accountBalance;
                info1.balanceStatus = info.balanceStatus;
                info1.branchCreated = info.branchCreated;
                info1.dateCreated = info.dateCreated;
                info1.interestRate = info.interestRate;
                info1.lien = info.lien;
                info1.cot = info.cot;
                info1.CustomerID = info.CustomerID;
                info1.linkedAcctNumber = info.linkedAcctNumber;
                info1.linkAcctType = info.linkAcctType;
                info1.months = info.months;
                info1.loanStatus = info.loanStatus; 

                _content.closedAcct.Add(info1);
                _content.customAccts.Remove(info);
            }

            _content.SaveChanges();     //Saving changes to the database.
            TempData["Success"] = "Account Closed Successfully";     //To display a success message to the user
            return RedirectToAction("CustomerAccounts", "Customers");
        }

        //GET: To view closed accounts
        public ActionResult viewClosed()
        {   
            var closed = _content.closedAcct.ToList();      //To load all closed accounts from the database.
            return View(closed);
        }

        //GET: To view closed account details
        public ActionResult viewClosedAcct(int id)
        {
            var acct = _content.closedAcct.SingleOrDefault(m => m.id == id);    //To load selected closed account details from the database.
            if (acct == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(acct);
            }
        }

        //To open closed customer account
        public ActionResult openAcct(int id)
        {
            var info = _content.closedAcct.SingleOrDefault(m => m.id == id);   //To load closed customer account from the database.
            if (info == null)   //Vreify the account is actually closed
            {
                return HttpNotFound();
            }
            else
            {
                if(info.customerAccountType == "Phoenix Loan Account")      //To check if its a loan account 
                {
                    var checkForLoan = _content.customAccts.SingleOrDefault(m => m.CustomerID == info.CustomerID && m.accountName == info.accountName && m.customerAccountType == "Phoenix Loan Account");  //To check if its a loan account
                    if(checkForLoan != null)    //To verify the custimer doesn't have another loan account before opening up the closed loan account
                    {
                        TempData["Error"] = "Customer Already has an open loan account, pls delete one";    //To display an error message to the user
                        return RedirectToAction("viewClosed", "Customers");
                    }
                }
                //To open up customer closed account
                CustomerAccounts info1 = new CustomerAccounts();
                info1.customerAccountNumber = info.customerAccountNumber;
                info1.customerAccountType = info.customerAccountType;
                info1.accountName = info.accountName;
                info1.accountBalance = info.accountBalance;
                info1.balanceStatus = info.balanceStatus;
                info1.branchCreated = info.branchCreated;
                info1.dateCreated = info.dateCreated;
                info1.interestRate = info.interestRate;
                info1.lien = info.lien;
                info1.cot = info.cot;
                info1.CustomerID = info.CustomerID;
                info1.linkAcctType = info.linkAcctType;
                info1.linkedAcctNumber = info.linkedAcctNumber;
                info1.months = info.months;
                info1.loanStatus = info.loanStatus;
                

                _content.customAccts.Add(info1);
                _content.closedAcct.Remove(info);
            }
            _content.SaveChanges();     //To save channges to the database.
                        
            TempData["Success"] = "Account Opened Successfully";    //To display success message to the user
            return RedirectToAction("viewClosed", "Customers");
        }

        //GET: To Edit Customer Accounts
        public ActionResult editAcct(int id)
        {
            var findAcct = _content.customAccts.SingleOrDefault(m => m.id == id);   //To load selected account details from the database.
            findAcct.interestRate = findAcct.interestRate * 100;
            if(findAcct == null)    //Verify the account exists
            {
                return HttpNotFound();
            }
            else
            {
                var vm = new AccIndexer     //Usintg view model
                {
                    acctIndexN = findAcct.customerAccountType,
                    CustomerAccounts = findAcct
                };

                return View(vm);
            }
        }

        //POST: To Edit Customer Accounts
        [HttpPost]
        public ActionResult saveEdit(AccIndexer form)
        {
            var acctEdit = _content.customAccts.SingleOrDefault(m => m.id == form.CustomerAccounts.id);     //To load selected account details from the database.
            if(!ModelState.IsValid)
            {
                return View("editAcct", form);
            }
            if(form.CustomerAccounts.lien < 0)
            {
                TempData["Error"] = "Lien Amount must not be negative";
                return View("editAcct", form);
            }
            if(form.CustomerAccounts.interestRate < 1 || form.CustomerAccounts.interestRate > 100) 
            {
                TempData["Error"] = "Interest Rate must be between the range 1 to 100";
                return View("editAcct", form);
            }
            if(acctEdit == null)    //To verify account exists
            {
                return View("editAcct", form);
            }
            else
            {
                form.CustomerAccounts.interestRate = form.CustomerAccounts.interestRate / 100;
                //Save updates to the customer account details
                acctEdit.branchCreated = form.CustomerAccounts.branchCreated;
                if(form.CustomerAccounts.customerAccountType != "Phoenix Loan Account")
                {
                    acctEdit.lien = form.CustomerAccounts.lien;
                }             
                acctEdit.interestRate = form.CustomerAccounts.interestRate;

                _content.SaveChanges();     //To save changes to the database.
            }

            TempData["Success"] = "Update Successful";      //To display success message.
            return RedirectToAction("CustomerAccounts", "Customers");
        }

        //GET: To assign account to customer profile
        public ActionResult profileAssign(int id)
        {
            var key = id;   //To store customer profile id
            var name = _content.custom.SingleOrDefault(m => m.id == id);    //To store customer profile name 
            var keyName = name.customerName;    //To store customer profile name
            var acctType = _content.acctIndex.ToList();     //To load all the available accounts from the database
            var customType = _content.custom.ToList();      //To load all the customer profiles in the database
            var userT = _content.Users.ToList();            //To load all the users from the database
            var statT = _content.stat.ToList();             //To load currency name from the database
            

            var vm = new AccIndexer     //Using view model
            {
                CustomerAccounts = new CustomerAccounts(),
                AccountT = acctType,
                CustomerT = customType,
                userT = userT,
                profileKey = key,
                profileName = keyName,
                status = statT
            };

            return View(vm);
        }

        //GET: To get all configurations
        public ActionResult setAcctConfig()
        {
            var configurations = _content.accountConfig.ToList();

            return View(configurations);
        }

        //GET: To get specific Account Type Configuration
        public ActionResult editConfig(int id)
        {
            var selectedConfiguration = _content.accountConfig.SingleOrDefault(m => m.id == id);
            selectedConfiguration.interestRate = selectedConfiguration.interestRate * 100;
            if(selectedConfiguration != null)
            {
                return View(selectedConfiguration);
            }
            else
            {
                return HttpNotFound();
            }
        }

        //POST: To set account configurations to all customer account
        [HttpPost]
        public ActionResult setConfigurations(AccountTypeConfig form)
        {
            if(form.lien < 0)
            {
                TempData["Error"] = "Lien Amount Must not be negative";
                return View("editConfig", form);
            }
            if(form.interestRate < 1 || form.interestRate > 100)
            {
                TempData["Error"] = "Interest Rate must be between 1 to 100";
                return View("editConfig", form);
            }
            if(!ModelState.IsValid)
            {
                return View("editConfig", form);
            }
            var selectedConfiguration = _content.accountConfig.SingleOrDefault(m => m.id == form.id);
            if(selectedConfiguration !=null)
            {
                var customerAccounts = _content.customAccts.ToList();
                foreach(var item in customerAccounts)
                {
                    if(item.customerAccountType == selectedConfiguration.accountType)
                    {
                        item.interestRate = form.interestRate / 100;
                        item.lien = form.lien;
                        _content.SaveChanges();
                    }
                }

                selectedConfiguration.interestRate = form.interestRate / 100;
                selectedConfiguration.lien = form.lien;
                _content.SaveChanges();

                TempData["Success"] = "Customer account Configurations Successfully Updated";
                return RedirectToAction("setAcctConfig", "Customers");
            }
            else
            {
                return HttpNotFound();
            }
        }

        //To set account configurations to all customer account
        public ActionResult setConfig(int id)
        {
            var configuration = _content.accountConfig.SingleOrDefault(m => m.id == id);
            if(configuration != null)
            {
                var customerAccounts = _content.customAccts.ToList();
                foreach (var item in customerAccounts)
                {
                    if (item.customerAccountType == configuration.accountType)
                    {
                        item.interestRate = configuration.interestRate;
                        item.lien = configuration.lien;
                        _content.SaveChanges();
                    }
                }
            }

            _content.SaveChanges();
            TempData["Success"] = "Customer account Configurations Successfully Updated";
            return RedirectToAction("setAcctConfig", "Customers");
        }
    }
}