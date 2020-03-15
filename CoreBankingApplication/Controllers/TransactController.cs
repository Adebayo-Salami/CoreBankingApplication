using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using CoreBankingApplication.Core.Models;
using CoreBankingApplication.Logic;
using System.Web;
using System.Web.Mvc;
using CoreBankingApplication.Data;

namespace CoreBankingApplication.Controllers
{
    [Authorize(Roles = RoleName.admin + "," + RoleName.teller)]
    public class TransactController : Controller
    {
        private CoreBankingApplication.Data.ApplicationDbContext _content;  //Creating an instance of the database connection.

        //Creating an instance of the database connection.
        public TransactController()
        {
            _content = new CoreBankingApplication.Data.ApplicationDbContext();
        }

        //Disposing of connnection to the database
        protected override void Dispose(bool disposing)
        {
            _content.Dispose();
        }

        // GET: Transact
        public ActionResult Index()
        {
            var vm = new AccIndexer
            {

            };

            return View(vm);
        }

        //GET: To post withdraw transaction
        public ActionResult postWithdraw()
        {
            var eodStatus = _content.eod.SingleOrDefault(m => m.id == 1);
            var acctDetails1 = _content.acctIndex.ToList();
            var acctDetails2 = _content.custom.ToList();
            var acctDetails3 = _content.customAccts.ToList();

            var vm = new AccIndexer
            {
                CustomerAT = acctDetails3,
                AccountT = acctDetails1,
                CustomerT = acctDetails2,
                PostWithdrawal = new PostWithdrawal()
            };

            if(eodStatus.status == true)
            {
                return View(vm);
            }
            else
            {
                TempData["Error"] = "User Not Allowed To Post Transactions during close of business.";
                return RedirectToAction("Index", "Transact");
            }
            
        }

        //POST: To post withdraw transaction
        [HttpPost]
        public ActionResult Withdraw(AccIndexer form)
        {
            //To load customer account details from the database. 
            var verify = _content.customAccts.SingleOrDefault(m => m.customerAccountNumber == form.PostWithdrawal.accountNumber && m.customerAccountType == form.PostWithdrawal.accountType);

            if(verify == null)      //Verify that customer account exist
            {
                TempData["Error"] = "Customer Account Doesn't Exist.";  //To display an error message to the user.
                return View("postWithdraw", form);
            }
            else
            {
                if(form.PostWithdrawal.amountW <= 0)        //To check the amount requested for withdrawal that it's not a negative number or zero
                {
                    TempData["Error"] = "Amount Must not be negative or zero."; //To display an error message to the user.s
                    return View("postWithdraw", form);
                }
                if(form.PostWithdrawal.accountType == "Phoenix Loan Account")
                {
                    TempData["Error"] = "Error, can't withdraw from a loan account."; //To display an error message to the user.s
                    return View("postWithdraw", form);
                }
                if (verify.accountBalance > (form.PostWithdrawal.amountW + verify.lien))    //To check that the balance after transaction will not be lower than the lien imposed on selected customer account
                {
                    var entryAcct = _content.acctIndex.SingleOrDefault(m => m.accountName == form.PostWithdrawal.accountType);      //To load account with the corresponding entry
                    if (entryAcct == null)
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        if(Session["tillAcountNumber"].ToString() != null)
                        {
                            form.PostWithdrawal.tillAccount = Session["tillAcountNumber"].ToString();
                        }
                        else
                        {
                            form.PostWithdrawal.tillAccount = "12324353531";
                        }
                        
                        form.PostWithdrawal.accountToCredit = entryAcct.accountName;        //To load account with the corresponding entry
                        form.PostWithdrawal.accountName = verify.accountName;
                        var tillAcct = _content.Users.SingleOrDefault(m => m.tillAccount == form.PostWithdrawal.tillAccount);   //To load till account details from the database
                        if (tillAcct == null)   //To verify that till account exist in the database
                        {
                            TempData["Error"] = "Current User does not have a till acount."; //To display an error message to the user.
                            return View("postWithdraw", form);
                        }
                        else
                        {
                            var tillBalance = _content.tellerDetails.Single(m => m.tillAccountNumber.ToString() == tillAcct.tillAccount);   //To load till account details
                            if (tillBalance.tillBalance > form.PostWithdrawal.amountW)      //To check that till account balance can perform requested transaction
                            {
                                var entry = _content.acctIndex.SingleOrDefault(m => m.accountName == form.PostWithdrawal.accountToCredit);  //To load account for corresponding entry
                                if (entry == null)
                                {
                                    TempData["Error"] = "Invalid Account To Credit";    //To display an error message to the user.
                                    return View("postWithdraw", form);
                                }
                                else
                                {
                                    //To check if customer has a loan account
                                    var checkForLoan = _content.customAccts.SingleOrDefault(m => m.accountName == verify.accountName && m.CustomerID == verify.CustomerID && m.customerAccountType == "Phoenix Loan Account");
                                    if(checkForLoan != null)
                                    {
                                        if(checkForLoan.loanStatus == false)    //If customer is owing bank on loan, transaction denied
                                        {
                                            if (checkForLoan.accountBalance > 0)
                                            {
                                                TempData["Error"] = "Customer can't withdraw from accout till loan is paid off";    //To display an error message to the user.
                                                return View("postWithdraw", form);
                                            }
                                            if (checkForLoan.accountBalance == 0)
                                            {
                                                TempData["Error"] = "Pls close or delete customer loan account before transaction can be allowed on customer account";  //To display an error message to the user.
                                                return View("postWithdraw", form);
                                            }
                                        }
                                    }
                                    //Post withdrawal transaction to thedatabase
                                    entry.amountInAcct = entry.amountInAcct - form.PostWithdrawal.amountW;
                                    form.PostWithdrawal.date = DateTime.Now;
                                    _content.postW.Add(form.PostWithdrawal);
                                    verify.accountBalance = (verify.accountBalance - form.PostWithdrawal.amountW);
                                    var bal = tillBalance.tillBalance - form.PostWithdrawal.amountW;
                                    tillBalance.tillBalance = bal;      //Credit till account
                                    Session["tillBal"] = bal;
                                    var cashAssetGL = _content.acctIndex.SingleOrDefault(m => m.id == 20);
                                    cashAssetGL.amountInAcct = cashAssetGL.amountInAcct - form.PostWithdrawal.amountW;
                                }
                            }
                            else
                            {
                                TempData["Error"] = "Till Account Balance too low to complete transaction";      //To display an error message to the user.
                                return View("postWithdraw", form);
                            }
                        }
                    } 
                }
                else
                {
                    TempData["Error"] = "Insufficient Funds";     //To display an error message to the user.
                    return View("postWithdraw", form);
                }

            }
            _content.SaveChanges();

            TempData["Success"] = "Transaction Successful";     //To display a success message to the user.
            return RedirectToAction("Index", "Transact");
        }

        //GET: To get withdawals records
        public ActionResult postWithdrawLog()
        {
            var viewLog = _content.postW.ToList();

            return View(viewLog);
        }

        //GET: To post deposit transactions
        public ActionResult postDeposit()
        {
            var eodStatus = _content.eod.SingleOrDefault(m => m.id == 1);     //To load EOD Status from the database
            var acctDetails1 = _content.acctIndex.ToList();     //To load accont details from the database
            var acctDetails2 = _content.custom.ToList();        //To load customer details from the database
            var acctDetails3 = _content.customAccts.ToList();   //To load customer account details from the database

            var vm = new AccIndexer     //Using view model
            {
                CustomerAT = acctDetails3,
                AccountT = acctDetails1,
                CustomerT = acctDetails2,
                PostDeposit = new PostDeposit()
            };

            if (eodStatus.status == true)       //Check if business is closed for the day or opened
            {
                return View(vm);
            }
            else
            {
                TempData["Error"] = "User Not Allowed To Post Transactions during close of business.";      //To display an error message to the user
                return RedirectToAction("Index", "Transact");
            }

        }

        //POST: To post deposit transactions
        [HttpPost]
        public ActionResult Deposit(AccIndexer form)
        {
            if(form.PostDeposit.amountD <= 0)       //To verify that amount is not less than zero or zero
            {
                TempData["Error"] = "Amount must never be negative or zero";        //To display an error message to the user.
                return View("postDeposit", form);
            }
            if (form.PostDeposit.accountType == "Phoenix Loan Account")
            {
                TempData["Error"] = "Error, can't deposit to a loan account."; //To display an error message to the user.s
                return View("postDeposit", form);
            }
            //To load customer account from the database
            var verifyAcct = _content.customAccts.SingleOrDefault(m => m.customerAccountNumber == form.PostDeposit.accountNumber && m.customerAccountType == form.PostDeposit.accountType);
            if(verifyAcct == null)      //To verify that customer account exists.
            {
                TempData["Error"] = "Customer Account Doesn't exist, pls create new customer profile for customer";     //To display an error message to the user.
                return View("postDeposit", form);
            }
            else
            {
                var entryAcct = _content.acctIndex.SingleOrDefault(m => m.accountName == form.PostDeposit.accountType);
                if(entryAcct == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    form.PostDeposit.accountToCredit = entryAcct.accountName;
                    form.PostDeposit.accountName = verifyAcct.accountName;
                    if (Session["tillAcountNumber"].ToString() != null)
                    {
                        form.PostDeposit.tillAccount = Session["tillAcountNumber"].ToString();
                    }
                    else
                    {
                        form.PostDeposit.tillAccount = "12324353531";
                    }
                    var tillAcct = _content.tellerDetails.SingleOrDefault(m => m.tillAccountNumber.ToString() == form.PostDeposit.tillAccount);     //To load till account details from the database
                    if(tillAcct == null)    //To verify that till account exists.s
                    {
                        TempData["Error"] = "Current User does not have a till Account";     //To display an error message to the user.
                        return View("postDeposit", form);
                    }
                    else
                    {
                        //To post deposit transaction to the database
                        entryAcct.amountInAcct = entryAcct.amountInAcct + form.PostDeposit.amountD;
                        verifyAcct.accountBalance = verifyAcct.accountBalance + form.PostDeposit.amountD;       //Credit customer account
                        tillAcct.tillBalance = tillAcct.tillBalance + form.PostDeposit.amountD;         //Credit till account
                        Session["tillBal"] = tillAcct.tillBalance;
                        var cashAssetGL = _content.acctIndex.SingleOrDefault(m => m.id == 20);
                        cashAssetGL.amountInAcct = cashAssetGL.amountInAcct + form.PostDeposit.amountD;
                        form.PostDeposit.accountToCredit = entryAcct.accountName;
                        form.PostDeposit.date = DateTime.Now;
                        _content.postD.Add(form.PostDeposit);
                        _content.SaveChanges();     //Saving changes to the database
                    }
                }
            }
            TempData["Success"] = "Transaction Successful";     //To display a success message to the user.
            return RedirectToAction("Index", "Transact");
        }

        //GET: To get deposits records
        public ActionResult postDepositLog()
        {
            var viewLog = _content.postD.ToList();

            return View(viewLog);
        }

        //GET: To post loan payment through cash
        public ActionResult postRepayment()
        {
            var eodStatus = _content.eod.SingleOrDefault(m => m.id == 1);       //To load EOD Status from the database
            var acctDetails1 = _content.acctIndex.ToList();         //To load accont details from the database
            var acctDetails2 = _content.custom.ToList();            //To load customer details from the database
            var acctDetails3 = _content.customAccts.ToList();       //To load customer account details from the database

            var vm = new AccIndexer     //Using view model
            {
                CustomerAT = acctDetails3,
                AccountT = acctDetails1,
                CustomerT = acctDetails2,
                loanPayment = new loanPayment()
            };

            if (eodStatus.status == true)       //Check if business is closed for the day or opened
            {
                return View(vm);
            }
            else
            {
                TempData["Error"] = "User Not Allowed To Post Transactions during close of business.";       //To display an error message to the user
                return RedirectToAction("Index", "Transact");
            }

        }

        //POST: To post loan payment through cash
        [HttpPost]
        public ActionResult loanPay(AccIndexer form)
        {
            if(!ModelState.IsValid)
            {
                return View("postRepayment", form);
            }
            if(form.loanPayment.amountP <= 0)
            {
                TempData["Error"] = "Amount must not be negative or less than zero";         //To display an error message to the user
                return View("postRepayment", form);
            }
            form.loanPayment.accountToCredit = "Phoenix Loan Account";
            //To fetch customer loan account details from the database
            var loanExists = _content.customAccts.SingleOrDefault(m => m.customerAccountNumber == form.loanPayment.accountNumber && m.customerAccountType == form.loanPayment.accountToCredit);
            if(loanExists == null)      //Verify that customer loan account exists.
            {
                TempData["Error"] = "Customer Does not have a loan Account.";        //To display an error message to the user
                return View("postRepayment", form);
            }
            else
            {
                if(form.loanPayment.amountP > loanExists.accountBalance)        //Verify that customer does not pay above loan amount
                {
                    var amt = loanExists.accountBalance;
                    TempData["Error"] = "Customer Should not pay more than he/she is meant to, loan balance remains " + amt + ", pls collect that amount";       //To display an error message to the user
                    return View("postRepayment", form);
                }
                else
                {
                    if (Session["tillAcountNumber"].ToString() != null)
                    {
                        form.loanPayment.tillAccount = Session["tillAcountNumber"].ToString();
                    }
                    else
                    {
                        form.loanPayment.tillAccount = "12324353531";
                    }
                    var verifyTill = _content.tellerDetails.SingleOrDefault(m => m.tillAccountNumber.ToString() == form.loanPayment.tillAccount);   //To load til details from the database
                    if(verifyTill == null)  //Verify that till account exist
                    {
                        TempData["Error"] = "Current User does not have a till Account";         //To display an error message to the user
                        return View("postRepayment", form);
                    }
                    else
                    {
                        var correspondingEntry = _content.acctIndex.SingleOrDefault(m => m.id == 13);
                        if(correspondingEntry == null)
                        {
                            return HttpNotFound();
                        }
                        else
                        {
                            //Post loan payment to database
                            correspondingEntry.amountInAcct = correspondingEntry.amountInAcct + form.loanPayment.amountP;
                            verifyTill.tillBalance = verifyTill.tillBalance + form.loanPayment.amountP;
                            Session["tillBal"] = verifyTill.tillBalance;
                            var cashAssetGL = _content.acctIndex.SingleOrDefault(m => m.id == 20);
                            cashAssetGL.amountInAcct = cashAssetGL.amountInAcct + form.loanPayment.amountP;
                            form.loanPayment.date = DateTime.Now;
                            loanExists.accountBalance = loanExists.accountBalance - form.loanPayment.amountP;
                            form.loanPayment.accountToCredit = correspondingEntry.accountName;
                            _content.loanPay.Add(form.loanPayment);

                            BankLog b3 = new BankLog();
                            b3.acctEntry = "Credit";
                            b3.correspondEntry = "Payment By Cash";
                            b3.amt = form.loanPayment.amountP;
                            b3.Entrydesc = "Payment of loan from customer By Cash";
                            b3.dateTr = DateTime.Now;
                            _content.bankL.Add(b3);
                        }
                    }
                }
            }
            _content.SaveChanges();

            TempData["Success"] = "Transaction Successful";      //To display a succes message to the user
            return RedirectToAction("Index", "Transact");
        }

        //GET: To post loan payment through bank account
        public ActionResult postRepaymentayA()
        {
            var eodStatus = _content.eod.SingleOrDefault(m => m.id == 1);  //To load EOD Status from the database 
            var acctDetails1 = _content.acctIndex.ToList();     //To load accont details from the database
            var acctDetails2 = _content.custom.ToList();        //To load customer details from the database
            var acctDetails3 = _content.customAccts.ToList();   //To load customer account details from the database

            var vm = new AccIndexer     //Using view model
            {
                CustomerAT = acctDetails3,
                AccountT = acctDetails1,
                CustomerT = acctDetails2,
                loanPayment = new loanPayment()
            };

            if (eodStatus.status == true)       //Check if business is closed for the day or opened
            {
                return View(vm);
            }
            else
            {
                TempData["Error"] = "User Not Allowed To Post Transactions during close of business.";      //To display error message to user
                return RedirectToAction("Index", "Transact");
            }
        }

        //POST: To post loan payment through bank account
        [HttpPost]
        public ActionResult loanPayA(AccIndexer form)
        {
            if(!ModelState.IsValid)
            {
                return View("postRepaymentayA", form);
            }
            if(form.loanPayment.amountP <= 0 )      //To check that amount is not less than zero or zero
            {
                TempData["Error"] = "Amount must not be negative or less than zero";        //To display an error message to the user
                return View("postRepaymentayA", form);
            }
            //To load customer account details from the database
            var verifyAcct = _content.customAccts.SingleOrDefault(m => m.customerAccountNumber == form.loanPayment.customerAccountNumber && m.customerAccountType == form.loanPayment.accountType);
            if(verifyAcct == null)      //To verify that customer account exist
            {
                TempData["Error"] = "Customer Account does not exist.";     //To display an error message to the user
                return View("postRepaymentayA", form);
            }
            else
            {
                if (Session["tillAcountNumber"].ToString() != null)
                {
                    form.loanPayment.tillAccount = Session["tillAcountNumber"].ToString();
                }
                else
                {
                    form.loanPayment.tillAccount = "12324353531";
                }
                var verifyTill = _content.tellerDetails.SingleOrDefault(m => m.tillAccountNumber.ToString() == form.loanPayment.tillAccount);
                if(verifyTill == null)
                {
                    TempData["Error"] = "Current User does not have a till Account";     //To display an error message to the user
                    return View("postRepaymentayA", form);
                }
                else
                {
                    form.loanPayment.accountToCredit = "Phoenix Loan Account";
                    //To load customer loan account from the database
                    var verifyLoan = _content.customAccts.SingleOrDefault(m => m.customerAccountNumber == form.loanPayment.accountNumber && m.customerAccountType == form.loanPayment.accountToCredit);
                    if(verifyLoan == null)      //To verify that customer loan account exists.
                    {
                        TempData["Error"] = "Customer Does not have any loan account.";   //To display an error message to the user  
                        return View("postRepaymentayA", form);
                    }
                    else
                    {
                        var entryLoan = _content.acctIndex.SingleOrDefault(m => m.id == 13);
                        if(entryLoan == null)
                        {
                            return HttpNotFound();
                        } 
                        else
                        {
                            var entryGL = _content.acctIndex.SingleOrDefault(m => m.accountName == form.loanPayment.accountType);
                            if(entryGL == null)
                            {
                                return HttpNotFound();
                            }
                            else
                            {
                                if(verifyAcct.accountBalance >= form.loanPayment.amountP)       //To verify that customer as enough funds in account to pay the amount
                                {
                                    if(form.loanPayment.amountP <= verifyLoan.accountBalance)   //To verify that customer is not charged more than the amount owing
                                    {
                                        if(form.loanPayment.accountType == "Phoenix Loan Account")      //To verify that its not a loan account used to pay loan
                                        {
                                            TempData["Error"] = "You can not pay loan from this account, pls make use of your other accounts if you have any or make cash payment.";        //To display an error message to the user
                                            return View("postRepaymentayA", form);
                                        }
                                        else
                                        {
                                            //To post loan payment transactiom to the database
                                            form.loanPayment.date = DateTime.Now;
                                            verifyAcct.accountBalance = verifyAcct.accountBalance - form.loanPayment.amountP;
                                            verifyLoan.accountBalance = verifyLoan.accountBalance - form.loanPayment.amountP;
                                            entryGL.amountInAcct = entryGL.amountInAcct - form.loanPayment.amountP;
                                            entryLoan.amountInAcct = entryLoan.amountInAcct + form.loanPayment.amountP;
                                            verifyTill.tillBalance = verifyTill.tillBalance + form.loanPayment.amountP;
                                            Session["tillBal"] = verifyTill.tillBalance;
                                            var cashAssetGL = _content.acctIndex.SingleOrDefault(m => m.id == 20);
                                            cashAssetGL.amountInAcct = cashAssetGL.amountInAcct + form.loanPayment.amountP;
                                            form.loanPayment.accountToCredit = entryLoan.accountName;
                                            _content.loanPay.Add(form.loanPayment);


                                            BankLog b3 = new BankLog();
                                            b3.acctEntry = "Credit";
                                            b3.correspondEntry = verifyAcct.accountName + "-" + verifyAcct.customerAccountNumber;
                                            b3.amt = form.loanPayment.amountP;
                                            b3.Entrydesc = "Payment of loan from customer";
                                            b3.dateTr = DateTime.Now;
                                            _content.bankL.Add(b3);
                                        }
                                    }
                                    else
                                    {
                                        TempData["Error"] = "Customer should not pay more than he/she is meant to, the customer's current debt sums up to " + verifyLoan.accountBalance + ", that amount should be collected";      //To display an error message to the user
                                        return View("postRepaymentayA", form);
                                    }
                                }
                                else
                                {
                                    TempData["Error"] = "Insufficient Funds in Customer Account.";      //To display an error message to the user
                                    return View("postRepaymentayA", form);
                                }
                            }
                        }
                    }
                }
            }
            _content.SaveChanges();
            TempData["Success"] = "Transaction Successful";     //To display a success message to the user
            return RedirectToAction("Index", "Transact");
        }

        //GET: To get loan payments records
        public ActionResult postRepaymentLog()
        {
            var viewLog = _content.loanPay.ToList();

            return View(viewLog);
        }

        //GET: To generate profit and loass financial report
        public ActionResult viewPL()
        {
            var vieww = _content.acctIndex.ToList();
            var inc = 0.0;
            var exp = 0.0;
            var balPL = 0.0;
            var negativeBal = 0.0;
            foreach(var item in vieww)
            {
                if(item.mainGLCategory == 4)    //To load all income GL Accounts
                {
                    inc += item.amountInAcct;
                }
                else if(item.mainGLCategory == 5)   //To load all expense GL Acounts
                {
                    exp += item.amountInAcct;
                }
                
            };
            balPL = inc - exp;
            if(balPL < 0)
            {
                negativeBal = new AccountFunction().ConvertNegativeNumber(balPL);
                balPL = -1;
            }
            var vm = new AccIndexer     //Using view model
            {
                AccountT = vieww,
                incomeB = inc,
                expenseB = exp,
                PLBal = balPL,
                PLMinusBal = negativeBal
            };

            return View(vm);
        }

        //GET: To generate balance sheet finanacial report
        public ActionResult viewBS()
        {
            var vieww = _content.acctIndex.ToList();
            var assetB = 0.0;
            var liabilityBal = 0.0;
            var totalA = 0.0;
            var netAset = 0.0;
            var interestPayable = 0.0;
            var negativeNetAsset = 0.0;
            var capital = 0.0;
            var netInc = 0.0;
            var balInc = 0.0;
            var negativeNetInc = 0.0;

            foreach (var item in vieww)
            {
                if (item.mainGLCategory == 1)   //To load all asset GL Accounts
                {
                    assetB += item.amountInAcct;
                }
                else if (item.mainGLCategory == 2)  //To load all liability GL Accounts
                {
                    liabilityBal += item.amountInAcct;
                }
                else if(item.mainGLCategory == 3)   //To load all capital GL Accounts
                {
                    assetB += item.amountInAcct;
                    capital += item.amountInAcct;
                }
                else if(item.mainGLCategory == 5)
                {
                    liabilityBal += item.amountInAcct;
                    interestPayable += item.amountInAcct;
                }
            };
            netAset = assetB - liabilityBal;
            totalA = assetB;
            netInc = netAset - capital;
            balInc = capital + netInc;
            if(netInc < 0)
            {
                negativeNetInc = new AccountFunction().ConvertNegativeNumber(netInc);
                netInc = -1;
            }

            if(netAset < 0)
            {
                negativeNetAsset = new AccountFunction().ConvertNegativeNumber(netAset);
                netAset = -1;
            }

            var vm = new AccIndexer     //Using view model
            {
                assetB = totalA,
                liabilityB = liabilityBal,
                netAsset = netAset,
                netMinusAsset = negativeNetAsset,
                AccountT = vieww,
                interestPay = interestPayable,
                capitalBal = capital,
                netBal = netInc,
                incBal = balInc,
                netMinusBal = negativeNetInc
            };

            return View(vm);
        }

        //GET: To generate trial balance financial report
        public ActionResult viewTB()
        {
            var allGLAccounts = _content.acctIndex.ToList();
            var assetBal = 0.0;
            var liabilityBal = 0.0;
            var capitalBal = 0.0;
            var incomeBal = 0.0;
            var expenseBal = 0.0;
            var debitAmount = 0.0;
            var creditAmount = 0.0;
            var balanceFigure = 0.0;
            var negativeFigure = 0.0;

            foreach(var item in allGLAccounts)
            {
                if(item.mainGLCategory == 1)
                {
                    assetBal = assetBal + item.amountInAcct;
                    debitAmount = debitAmount + item.amountInAcct;
                }
                if (item.mainGLCategory == 2)
                {
                    liabilityBal = liabilityBal + item.amountInAcct;
                    debitAmount = debitAmount + item.amountInAcct;
                }
                if (item.mainGLCategory == 3)
                {
                    capitalBal = capitalBal + item.amountInAcct;
                    creditAmount = creditAmount + item.amountInAcct;
                }
                if (item.mainGLCategory == 4)
                {
                    incomeBal = incomeBal + item.amountInAcct;
                    creditAmount = creditAmount + item.amountInAcct;
                }
                if (item.mainGLCategory == 5)
                {
                    expenseBal = expenseBal + item.amountInAcct;
                    creditAmount = creditAmount + item.amountInAcct;
                }
            }
            balanceFigure = creditAmount - debitAmount;
            if(balanceFigure  < 0)
            {
                negativeFigure = new AccountFunction().ConvertNegativeNumber(balanceFigure);
                debitAmount = debitAmount - negativeFigure;
                assetBal = assetBal - negativeFigure;
                balanceFigure = -1;
            }
            else
            {
                debitAmount = debitAmount + balanceFigure;
                assetBal = assetBal + balanceFigure;
            }
            var vm = new AccIndexer
            {
                AccountT = allGLAccounts,
                assetB = assetBal,
                liabilityB = liabilityBal,
                capitalBal = capitalBal,
                incomeB = incomeBal,
                expenseB = expenseBal,
                credit = creditAmount,
                debit = debitAmount,
                balanceFig = balanceFigure,
                negativeFig = negativeFigure
            };

            return View(vm);
        }
        
        //GET: To run EOD
        [Authorize(Roles = RoleName.admin)]
        public ActionResult EOD()
        {
            var vm = new AccIndexer //Using view model
            {

            };

            return View(vm);
        }

        //GET: To run EOD (open business)
        [Authorize(Roles = RoleName.admin)]
        public ActionResult openB()
        {
            var status = _content.eod.SingleOrDefault(m => m.id == 1);      //Fetch eod status from the database
            if(status.status == true)
            {
                TempData["Error"] = "Business is already Opened For The Day";   //To display an error message to the user
                return RedirectToAction("EOD", "Transact");
            }
            status.status = true;   //Change EOD status to open
            _content.SaveChanges();

            TempData["Success"] = "Business Opened For The day";       //To display a success message to the user
            return RedirectToAction("EOD", "Transact");
        }

        //GET: To run EOD (close business)
        [Authorize(Roles = RoleName.admin)]
        public ActionResult closeB()
        {
            var status = _content.eod.SingleOrDefault(m => m.id == 1);      //To fetch current eod status from the database
            var savingsAcct = _content.acctIndex.SingleOrDefault(m => m.id == 1);
            var currentAcct = _content.acctIndex.SingleOrDefault(m => m.id == 3);
            var loanAcct = _content.acctIndex.SingleOrDefault(m => m.id == 12);
            if (status.status == false)      //Verify that business is not already closed
            {
                TempData["Error"] = "Business already closed for the day";      //To display an error message to the user.
                return RedirectToAction("EOD", "Transact");
            }
            if (status.countDays < 30)      //To count the number of EOD ran befor it reaches EOM
            {
                var customA = _content.customAccts.ToList();    //To load customer accounts from the database
                var savingsInteres = _content.SIlog.ToList();   //To load savings interest GL Accounts from the database
                var currentInterest = _content.CIlog.ToList();  //To load current interest GL Accounts from the database
                var loanInterestAccrual = _content.loanAccrued.ToList();    //To load loan interest recievable GL Account from the database


                foreach (var item in customA)       //To check through all customer accounts in the database
                {                  
                    var acctName = item.customerAccountType;    //To store customer account type
                    var interestRate = item.interestRate;       //To store customer interest rate
                    var commisionOnTurnover = item.cot;         //To store customer cot percent
                    double acctB = item.accountBalance;         //To store customer account balance
                    double balD = acctB;                        //To store customer account balance
                    double calcI = interestRate;                //To store customer interest rate
                    double cotD = commisionOnTurnover;          //To store customer cot rate
                    double cc = calcI / 30;                     //To calculate customer's interest rate per day
                    double calcDay = cc * balD;                 //To calculate customer's interest amount per day

                    if (acctName == savingsAcct.accountName)      //To run savings configuration for on customer account
                    {
                        if(savingsInteres.Count == 0)       //Check if customer has a savings interest Account
                        {
                            var CEntry = _content.acctIndex.SingleOrDefault(m => m.id == 13);   //To load account with corresponding entry
                            //To create a savings interest account for the customer savings account
                            SavingsInterestLog logS = new SavingsInterestLog();     
                            logS.acctEntry = "CREDIT";
                            logS.correspondEntry = CEntry.accountName;
                            logS.entryAccN = CEntry.accountCode;
                            logS.customerAccNumb = item.customerAccountNumber;
                            logS.customerAcctype = item.customerAccountType;
                            logS.amt = calcDay;
                            logS.Entrydesc = "Amount paid as Interest Rate on customer savings account";
                            logS.dateTr = DateTime.Now;

                            _content.SIlog.Add(logS);   //Save customer savings interest account
                            var indexAcct = _content.acctIndex.SingleOrDefault(m => m.id == 7);     //To load savings interest GL Account details
                            indexAcct.amountInAcct = indexAcct.amountInAcct + calcDay;      //Credit the expense account
                        }
                        else
                        {
                            foreach (var itemS in savingsInteres)       //Check if customer has a savings interest Account
                            {
                                //To fetch customer savings interest account
                                var checkAccc = _content.SIlog.SingleOrDefault(m => m.customerAccNumb == item.customerAccountNumber && m.customerAcctype == item.customerAccountType);
                                if (checkAccc != null)  //To verikfy that the interest account exist
                                {
                                    checkAccc.amt = checkAccc.amt + calcDay;    //Increase the interest accrued                        
                                }
                                else
                                {
                                    var CEntry = _content.acctIndex.SingleOrDefault(m => m.id == 13);   //To load account with corresponding entry
                                    //To create a savings interest account for the customer savings account
                                    SavingsInterestLog logS = new SavingsInterestLog();
                                    logS.acctEntry = "CREDIT";
                                    logS.correspondEntry = CEntry.accountName;
                                    logS.entryAccN = CEntry.accountCode;
                                    logS.customerAccNumb = item.customerAccountNumber;
                                    logS.customerAcctype = item.customerAccountType;
                                    logS.amt = calcDay;
                                    logS.Entrydesc = "Amount paid as Interest Rate on customer savings account";
                                    logS.dateTr = DateTime.Now;

                                    _content.SIlog.Add(logS);   //Save customer savings interest account
                                }

                                var indexAcct = _content.acctIndex.SingleOrDefault(m => m.id == 7);     //To load savings interest GL Account details
                                indexAcct.amountInAcct = indexAcct.amountInAcct + calcDay;      //Credit the expense account
                            }
                        }
                        
                    }

                    if (acctName == currentAcct.accountName)      //To run current account configuration  on customer account
                    {
                        if(currentInterest.Count == 0)       //Check if customer has a current interest Account
                        {
                            var CEntry = _content.acctIndex.SingleOrDefault(m => m.id == 13);   //To load account with corresponding entry
                            //To create a current interest account for the customer savings account
                            CurrentInterestLog logS = new CurrentInterestLog();
                            logS.acctEntry = "CREDIT";
                            logS.correspondEntry = CEntry.accountName;
                            logS.entryAccN = CEntry.accountCode;
                            logS.customerAccNumb = item.customerAccountNumber;
                            logS.customerAcctype = item.customerAccountType;
                            logS.amt = calcDay;
                            logS.Entrydesc = "Amount paid as Interest Rate on customer savings account";
                            logS.dateTr = DateTime.Now;

                            _content.CIlog.Add(logS);       //Save customer current interest account

                            var indexAcct = _content.acctIndex.SingleOrDefault(m => m.id == 8);     //To load current interest GL Account details
                            indexAcct.amountInAcct = indexAcct.amountInAcct + calcDay;      //Credit the expense account
                        }
                        else
                        {                            
                            foreach (var itemS in currentInterest)      //Check if customer has a current interest Account
                            {
                                //To fetch customer curent interest account
                                var checkAccc = _content.CIlog.SingleOrDefault(m => m.customerAccNumb == item.customerAccountNumber && m.customerAcctype == item.customerAccountType);
                                if (checkAccc != null)      //To verikfy that the interest account exist
                                {
                                    checkAccc.amt = checkAccc.amt + calcDay;    //Increase the interest accrued
                                }
                                else
                                {
                                    var CEntry = _content.acctIndex.SingleOrDefault(m => m.id == 13);       //To load account with corresponding entry
                                    //To create a current interest account for the customer savings account
                                    CurrentInterestLog logS = new CurrentInterestLog();
                                    logS.acctEntry = "CREDIT";
                                    logS.correspondEntry = CEntry.accountName;
                                    logS.entryAccN = CEntry.accountCode;
                                    logS.customerAccNumb = item.customerAccountNumber;
                                    logS.customerAcctype = item.customerAccountType;
                                    logS.amt = calcDay;
                                    logS.Entrydesc = "Amount paid as Interest Rate on customer savings account";
                                    logS.dateTr = DateTime.Now;

                                    _content.CIlog.Add(logS);        //Save customer current interest account
                                }

                                var indexAcct = _content.acctIndex.SingleOrDefault(m => m.id == 8);     //To load current interest GL Account details
                                indexAcct.amountInAcct = indexAcct.amountInAcct + calcDay;          //Credit the expense account    
                            }
                        }
                        
                    }

                    if(acctName != savingsAcct.accountName && acctName != currentAcct.accountName)      //To run loan account configuration  on customer account
                    {
                        if(loanInterestAccrual.Count != 0)      //Check if loan interest receivable account exist
                        {
                            foreach(var itemL in loanInterestAccrual)
                            {
                                //To get customer loam account details
                                var loanAccounExist = _content.customAccts.SingleOrDefault(m => m.accountName == itemL.customerAcctName && m.customerAccountNumber == itemL.customerAcctNo && m.customerAccountType == "Phoenix Loan Account");
                                if(loanAccounExist != null)
                                {
                                    if (itemL.duration > 0)      //Verify loan duration is not yet over
                                    {
                                        var accrued = itemL.interestRate * itemL.loanAmount;    //To calculate the interest accrued amount
                                                                                                
                                        if (loanAccounExist.accountBalance > accrued)       //Verify that customer loan account exist
                                        {
                                            itemL.accruedAmt = itemL.accruedAmt + accrued;  //Credit(increase) accrued amount

                                            //To get customer loam account details
                                            var loanIncome = _content.loanI.SingleOrDefault(m => m.accountName == itemL.customerAcctName && m.acctNo == itemL.customerAcctNo);
                                            loanIncome.accruedInc = loanIncome.accruedInc + accrued;   

                                            var indexAccount = _content.acctIndex.SingleOrDefault(m => m.id == 23);     //Get Loan Interest Account GL
                                            indexAccount.amountInAcct = indexAccount.amountInAcct + accrued;            //Update Loan Interest Account GL
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                status.countDays += 1;      //Increase financial date by one day
            }
            else
            {
                var customerAccts = _content.customAccts.ToList();      //To load all customer accounts from the database
                var loanInterestAccrual = _content.loanAccrued.ToList();    //To load all loan interest receivable

                var incomeCOT = 0.0;        //Seting variable
                var interestPayS = 0.0;     //Seting variable
                var interestPayC = 0.0;     //Seting variable
                var cotIncome = 0.0;
                var accruedTotal = 0.0;     //Seting variable
                var accruedSingle = 0.0;    //Setting Variable

                //Running configurations for EOM
                foreach (var item in customerAccts)     //To check through all customer accounts
                {
                    var acctN = item.customerAccountType;     //To store customer account type  
                    var interestRate = item.interestRate;     //To store customer interest rate
                    var commisionOnTurnover = item.cot;       //To store customer cot rate
                    var acctB = item.accountBalance;          //To store custome account balance
                    var lastDate = DateTime.Now;             //To set date and time of transactions

                    double total_withdraw = 0.0;     //Seting variable
                    var cotCalc = _content.postW.ToList();  //Fetching customer withdrawals
                    foreach (var itemC in cotCalc)      //check through all cot GL Account
                    {
                        if (itemC.accountNumber == item.customerAccountNumber)      //Check all customers withdrawal
                        {
                            total_withdraw = total_withdraw + itemC.amountW;        //Sum Up all customer withdrawals
                        }
                    }

                    double balD = (double)acctB;        //Get Customer account balance
                    double calcI = (double)interestRate;    //Get Customer account interest rate configurations
                    double cotD = (double)commisionOnTurnover;  //Get Customer account cot configuration
                    double calcDay = calcI * balD;      //Calculate customer interest rate
                    double commisionDay;
                    if (total_withdraw > 1000)
                    {
                        total_withdraw = total_withdraw / 1000;
                        total_withdraw = Math.Ceiling(total_withdraw);
                        commisionDay = total_withdraw * cotD;    //Calculate customer cot
                    }
                    else
                    {
                        commisionDay = 0;
                    }
                    


                    if (acctN == savingsAcct.accountName)     //Running Savings account configuration on EOM
                    {
                        //To load customer savings interest account
                        var checkInterest = _content.SIlog.SingleOrDefault(m => m.customerAccNumb == item.customerAccountNumber && m.customerAcctype == item.customerAccountType);
                        if(checkInterest == null)      //Verify interest account exist
                        {
                            var CEntry = _content.acctIndex.SingleOrDefault(m => m.id == 13);  //To load corresponding entry GL Account
                            //To create customer interest account
                            SavingsInterestLog logS = new SavingsInterestLog();
                            logS.acctEntry = "CREDIT";
                            logS.correspondEntry = CEntry.accountName;
                            logS.entryAccN = CEntry.accountCode;
                            logS.customerAccNumb = item.customerAccountNumber;
                            logS.customerAcctype = item.customerAccountType;
                            logS.amt = calcDay;
                            logS.paidAmount = calcDay;
                            logS.Entrydesc = "Amount paid as Interest Rate on customer savings account";
                            logS.dateTr = DateTime.Now;

                            _content.SIlog.Add(logS);
                        }
                        else
                        {
                            item.accountBalance = item.accountBalance + checkInterest.amt;  //Add interest amount to customer account 
                            interestPayS = interestPayS + checkInterest.amt;
                            checkInterest.paidAmount = checkInterest.paidAmount + checkInterest.amt;
                            checkInterest.amt = 0;
                        }
                    }

                    if (acctN == currentAcct.accountName)     //Running Current account configuration on EOM
                    {
                        //To load customer current interest account
                        var checkInterest = _content.CIlog.SingleOrDefault(m => m.customerAccNumb == item.customerAccountNumber && m.customerAcctype == item.customerAccountType);
                        //To load customer cot account
                        var checkCOT = _content.cotI.SingleOrDefault(m => m.customerAccNumb == item.customerAccountNumber && m.customerAcctype == item.customerAccountType);

                        if(checkCOT == null)    //Verify customer cot account
                        {
                            //To create customer cot account
                            CotIncomeGL cot = new CotIncomeGL();
                            cot.acctEntry = "Credit";
                            cot.customerAccNumb = item.customerAccountNumber;
                            cot.customerAcctype = item.customerAccountType;
                            cot.amt = commisionDay;
                            cot.previousAmt = cot.amt;
                            cot.Entrydesc = "Income Calaculated from customer's current account";
                            cot.dateTr = DateTime.Now;
                            cotIncome = cotIncome + cot.amt;
                            _content.cotI.Add(cot);
                        }
                        else
                        {
                            checkCOT.amt = commisionDay - checkCOT.previousAmt;     
                            item.accountBalance = item.accountBalance - checkCOT.amt;       //Debit cot charge from the customer account
                            incomeCOT = incomeCOT + checkCOT.amt;       //Corresponding GL Account entry
                            checkCOT.previousAmt = checkCOT.previousAmt + checkCOT.amt;
                            cotIncome = cotIncome + checkCOT.amt;
                            checkCOT.amt = 0.0;
                        }

                        if (checkInterest == null)  //Verify customer interest account
                        {
                            var CEntry = _content.acctIndex.SingleOrDefault(m => m.id == 13);   //Load corresponding entry account
                            //Create cusomer interest account
                            CurrentInterestLog logS = new CurrentInterestLog();
                            logS.acctEntry = "CREDIT";
                            logS.correspondEntry = CEntry.accountName;
                            logS.entryAccN = CEntry.accountCode;
                            logS.customerAccNumb = item.customerAccountNumber;
                            logS.customerAcctype = item.customerAccountType;
                            logS.amt = calcDay;
                            logS.paidAmount = calcDay;
                            logS.Entrydesc = "Amount paid as Interest Rate on customer savings account";
                            logS.dateTr = DateTime.Now;
                            interestPayC = interestPayC + calcDay;

                            _content.CIlog.Add(logS);
                        }
                        else
                        {                       
                            checkInterest.amt = checkInterest.amt + calcDay;       //Credit customer interest account               
                            item.accountBalance = item.accountBalance + checkInterest.amt;
                            interestPayC = interestPayC + checkInterest.amt;
                            checkInterest.paidAmount = checkInterest.paidAmount + checkInterest.amt;                 
                            checkInterest.amt = 0;
                        }
                    }
                    if (acctN != savingsAcct.accountName && acctN != currentAcct.accountName)    //Load loan configurations for EOM
                    {
                        if (loanInterestAccrual.Count != 0)     //check for customer loan interest accrual account
                        {
                            foreach (var itemL in loanInterestAccrual)
                            {
                                //To get customer loan account
                                var loanCheck = _content.customAccts.SingleOrDefault(m => m.accountName == itemL.customerAcctName && m.customerAccountNumber == itemL.customerAcctNo && m.customerAccountType == "Phoenix Loan Account");
                                if(loanCheck != null)
                                {
                                    if (itemL.duration > 0)      //Check if loan is still active
                                    {
                                        var accrued = itemL.interestRate * itemL.loanAmount;    //Get total accrued amount
                                        itemL.accruedAmt = itemL.accruedAmt + accrued;      //Get total accrued amount
                                        accruedSingle = accruedSingle + accrued;
                                        if (loanCheck.accountBalance < itemL.accruedAmt) //Verify that loan amount is above accued amount
                                        {
                                            itemL.accruedAmt = loanCheck.accountBalance;
                                            accrued = loanCheck.accountBalance;
                                            itemL.duration = 0;
                                        }
                                        //To load loan income GL Account 
                                        var loanIncome = _content.loanI.SingleOrDefault(m => m.accountName == itemL.customerAcctName && m.acctNo == itemL.customerAcctNo);
                                        loanIncome.accruedInc = loanIncome.accruedInc + accrued;    //Credit loan interest income account

                                        //To get customer linked accounts
                                        var customerLinkedAcct = _content.customAccts.SingleOrDefault(m => m.customerAccountType == itemL.linkedAcctName && m.customerAccountNumber == itemL.linkedAcctNo);
                                        if (customerLinkedAcct.accountBalance >= itemL.accruedAmt)  //Verify customer has enough funds in the linked account to pay of accrued amount
                                        {
                                            //To debit customer linked account
                                            customerLinkedAcct.accountBalance = customerLinkedAcct.accountBalance - itemL.accruedAmt;
                                            accruedTotal = accruedTotal + itemL.accruedAmt;
                                            //Get customer loan account
                                            var checkForLoan = _content.customAccts.SingleOrDefault(m => m.accountName == itemL.customerAcctName && m.customerAccountNumber == itemL.customerAcctNo && m.customerAccountType == "Phoenix Loan Account");
                                            checkForLoan.accountBalance = checkForLoan.accountBalance - itemL.accruedAmt;       //Debit customer loan account
                                            if (itemL.duration != 0)
                                            {
                                                itemL.duration = itemL.duration - 1;    //Decrease loan fee duration
                                            }
                                            itemL.accruedAmt = 0.0;
                                        }
                                        else
                                        {
                                            //Bloack all customer accounts if, customer is unable to pay off loan installments
                                            var checkForLoan = _content.customAccts.SingleOrDefault(m => m.accountName == itemL.customerAcctName && m.customerAccountNumber == itemL.customerAcctNo && m.customerAccountType == "Phoenix Loan Account");
                                            if (checkForLoan != null)
                                            {
                                                checkForLoan.loanStatus = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //Update all affected GL Accounts       
                var updateS = _content.acctIndex.SingleOrDefault(m => m.id == 1);                                                   //Get Savings Account GL
                updateS.amountInAcct = updateS.amountInAcct + interestPayS;                                                         //Update Savings Account GL
                var updateC = _content.acctIndex.SingleOrDefault(m => m.id == 3);                                                   //Get Current Account GL
                updateC.amountInAcct = updateC.amountInAcct + interestPayC;                                                        //Update Current Account GL
                var updateL = _content.acctIndex.SingleOrDefault(m => m.id == 12);                                                 //Get Loan Account GL
                updateL.amountInAcct = updateL.amountInAcct - accruedTotal;                                                        //Update Loan Account GL
                var updateLI = _content.acctIndex.SingleOrDefault(m => m.id == 9);                                                 //Get Loan Income Account GL
                updateLI.amountInAcct = updateLI.amountInAcct + accruedTotal;                                                      //Update Loan Income Account GL
                var updateLoanInterest = _content.acctIndex.SingleOrDefault(m => m.id == 23);                                      //Get Loan Interest Account GL
                updateLoanInterest.amountInAcct = (updateLoanInterest.amountInAcct + accruedSingle) - accruedTotal;                //Update Loan Interest Account GL
                var updateCOT = _content.acctIndex.SingleOrDefault(m => m.id == 15);                                               //Get COT Income Account GL
                updateCOT.amountInAcct = updateCOT.amountInAcct + cotIncome;                                                      //To Update COT Income Account GL

                //Log records to the dataabase
                double interestPay = interestPayS + interestPayC;
                BankLog bl = new BankLog();
                bl.acctEntry = "Debit";
                bl.correspondEntry = "S & C Interest Account";
                bl.amt = interestPay;
                bl.Entrydesc = "Payment For Customers Interest Account";
                bl.dateTr = DateTime.Now;

                _content.bankL.Add(bl);

                var updateBank = _content.acctIndex.SingleOrDefault(m => m.id == 13);
                updateBank.amountInAcct = updateBank.amountInAcct - interestPay;

                TempData["EOM"] = "CLOSE OF BUSINESS FOR THE MONTH HAS BEEN EXECUTED SUCCESSFULLY(EOM)";
                status.countDays = 0;
            }

            status.status = false;      //Close business for the day
            _content.SaveChanges();

            TempData["Success"] = "Business Successfully Closed For The Day";       //To display a success message to the user.
            return RedirectToAction("EOD", "Transact");
        }

        //GET: To post GL Transactions
        public ActionResult postGLTransactions()
        {
            var vm = new AccIndexer     //Using view model
            {
                PostGLTransactions = new PostGLTransactions()
            };

            var eodStatus = _content.eod.SingleOrDefault(m => m.id == 1);
            if(eodStatus.status == true)    //Verify that business is already opened for the day
            {
                return View(vm);
            }
            else
            {
                TempData["Error"] = "User Not Allowed To Post Transactions during close of business.";      //Display an error message to the user
                return RedirectToAction("Index", "Transact");
            }
        }

        //POST: To post GL transactions
        [HttpPost]
        public ActionResult saveGLPosting(AccIndexer form)
        {
            form.PostGLTransactions.dateOfTransaction = DateTime.Now;
            if(!ModelState.IsValid)
            {
                return View("postGLTransactions", form);
            }
            else
            {
                if(form.PostGLTransactions.amountDebit <= 0 || form.PostGLTransactions.amountCredit <= 0)       //Check that the amount is not negative or less than zero
                {
                    TempData["Error"] = "Amount must never be negative or zero";        //Display an error message to the user
                    return View("postGLTransactions", form);
                }
                if(form.PostGLTransactions.glReceiving == form.PostGLTransactions.glSending)        //Verify that GL Sending and GL Receiving are not the same GL Account
                {
                    TempData["Error"] = "GL Account Sending and GL Account Recieving must not be the same";     //Display an error message to the user
                    return View("postGLTransactions", form);
                }

                var verifyGLSending = _content.acctIndex.SingleOrDefault(m => m.accountName == form.PostGLTransactions.glSending);
                var veriyGLReceiving = _content.acctIndex.SingleOrDefault(m => m.accountName == form.PostGLTransactions.glReceiving);

                if(form.PostGLTransactions.amountCredit == form.PostGLTransactions.amountDebit)     //Verify that the debit amount and credit amount are the same
                {
                    //Check that none of the gl is a till account
                    if (verifyGLSending != null && veriyGLReceiving != null)
                    {
                        var accountToDebit = _content.acctIndex.SingleOrDefault(m => m.accountName == form.PostGLTransactions.glSending);
                        if (accountToDebit.amountInAcct >= form.PostGLTransactions.amountDebit)
                        {
                            //Posting GL Transactions
                            var accountToCredit = _content.acctIndex.SingleOrDefault(m => m.accountName == form.PostGLTransactions.glReceiving);
                            accountToDebit.amountInAcct = accountToDebit.amountInAcct - form.PostGLTransactions.amountDebit;
                            accountToCredit.amountInAcct = accountToCredit.amountInAcct + form.PostGLTransactions.amountCredit;
                            form.PostGLTransactions.glReceivingAccountCode = veriyGLReceiving.accountCode;
                            form.PostGLTransactions.glSendingAccountCode = verifyGLSending.accountCode;
                            _content.postGL.Add(form.PostGLTransactions);
                            _content.SaveChanges();
                        }
                        else
                        {
                            TempData["Error"] = "Insufficient funds in the GL Account To Debit";        //To display an error message to the user
                            return View("postGLTransactions", form);
                        }
                    }
                    var tillSending = _content.tellerDetails.SingleOrDefault(m => m.tellerUsername == form.PostGLTransactions.glSending);
                    var tillReceiving = _content.tellerDetails.SingleOrDefault(m => m.tellerUsername == form.PostGLTransactions.glReceiving);
                    if(tillSending != null && tillReceiving != null)
                    {
                        if(tillSending.tillBalance >= form.PostGLTransactions.amountDebit)
                        {
                            tillReceiving.tillBalance = tillReceiving.tillBalance + form.PostGLTransactions.amountCredit;
                            tillSending.tillBalance = tillSending.tillBalance - form.PostGLTransactions.amountDebit;
                            form.PostGLTransactions.glReceivingAccountCode = tillReceiving.tillAccountNumber;
                            form.PostGLTransactions.glSendingAccountCode = tillSending.tillAccountNumber;
                            _content.postGL.Add(form.PostGLTransactions);
                            _content.SaveChanges();
                        }
                        else
                        {
                            TempData["Error"] = "Insufficient funds in the GL Account To Debit";        //To display an error message to the user
                            return View("postGLTransactions", form);
                        }
                    }
                    else if(tillSending != null && tillReceiving == null)
                    {
                        if (tillSending.tillBalance >= form.PostGLTransactions.amountDebit)
                        {
                            tillSending.tillBalance = tillSending.tillBalance - form.PostGLTransactions.amountDebit;
                            veriyGLReceiving.amountInAcct = veriyGLReceiving.amountInAcct + form.PostGLTransactions.amountCredit;
                            form.PostGLTransactions.glReceivingAccountCode = veriyGLReceiving.accountCode;
                            form.PostGLTransactions.glSendingAccountCode = tillSending.tillAccountNumber;
                            _content.postGL.Add(form.PostGLTransactions);
                            var tillAcconts = _content.acctIndex.SingleOrDefault(m => m.id == 20);
                            tillAcconts.amountInAcct = tillAcconts.amountInAcct - form.PostGLTransactions.amountDebit;
                            _content.SaveChanges();
                        }
                        else
                        {
                            TempData["Error"] = "Insufficient funds in the GL Account To Debit";        //To display an error message to the user
                            return View("postGLTransactions", form);
                        }
                    }
                    else if(tillSending == null && tillReceiving != null)
                    {
                        verifyGLSending.amountInAcct = verifyGLSending.amountInAcct - form.PostGLTransactions.amountDebit;
                        tillReceiving.tillBalance = tillReceiving.tillBalance + form.PostGLTransactions.amountCredit;
                        form.PostGLTransactions.glReceivingAccountCode = tillReceiving.tillAccountNumber;
                        form.PostGLTransactions.glSendingAccountCode = verifyGLSending.accountCode;
                        _content.postGL.Add(form.PostGLTransactions);
                        var tillAcconts = _content.acctIndex.SingleOrDefault(m => m.id == 20);
                        tillAcconts.amountInAcct = tillAcconts.amountInAcct + form.PostGLTransactions.amountCredit;
                        _content.SaveChanges();
                    }
                }
                else
                {
                    TempData["Error"] = "Amount to debit must be equals to amount to credit";       //Display an error message to the user
                    return View("postGLTransactions", form);
                }
            }
            TempData["Success"] = "GL Posting Transaction Successful";      //Display a success message to the user
            return RedirectToAction("postGLTransactions", "Transact");
        }

        //GET: To get GL Transaction records
        public ActionResult glPostingsLog()
        {
            var glPostings = _content.postGL.ToList();      //Load all GL Posting records from the database
            var fundGL = _content.fundGL.ToList();

            var vm = new AccIndexer
            {
                postGLT = glPostings,
                fundGLT = fundGL
            };

            return View(vm);
        }

        //GET: To fund capital GL Account
        public ActionResult fundCapitalGL()
        {
            var allGLs = _content.acctIndex.Where(g=>g.mainGLCategory == 3).ToList();
            var vm = new AccIndexer     //Using view model
            {
                AccountTF = allGLs,
               FundCapitalGL = new FundCapitalGL()
            };

            var eodStatus = _content.eod.SingleOrDefault(m => m.id == 1);
            if (eodStatus.status == true)    //Verify that business is already opened for the day
            {
                return View(vm);
            }
            else
            {
                TempData["Error"] = "User Not Allowed To Post Transactions during close of business.";      //Display an error message to the user
                return RedirectToAction("Index", "Transact");
            }
        }

        //POST: To fund capital GL Account
        [HttpPost]
        public ActionResult fundGLPosting(AccIndexer form)
        {
            form.FundCapitalGL.date = DateTime.Now;
            if(!ModelState.IsValid)
            {
                return View("fundCapitalGL", form);
            }
            else
            {
                if (form.FundCapitalGL.amountCredit <= 0)       //Check that the amount is not negative or less than zero
                {
                    TempData["Error"] = "Amount must never be negative or zero";        //Display an error message to the user
                    return View("fundCapitalGL", form);
                }
                //To verify that its a capital account that is being funded
                var verifyCapitalGL = _content.acctIndex.SingleOrDefault(m => m.accountName == form.FundCapitalGL.glReceiving);     
                if (verifyCapitalGL.mainGLCategory != 3)
                {
                    TempData["Error"] = "Sorry this option is only available for a capital account";        //Display an error message to the user
                    return View("fundCapitalGL", form);
                }
                form.FundCapitalGL.amountDebit = form.FundCapitalGL.amountCredit;
                if (form.FundCapitalGL.amountCredit == form.FundCapitalGL.amountDebit)     //Verify that the debit amount and credit amount are the same
                {
                    //Posting GL Transactions
                    form.FundCapitalGL.glSending = "Cash At Hand";
                    form.FundCapitalGL.glReceivingAccountCode = verifyCapitalGL.accountCode;
                    form.FundCapitalGL.glSendingAccountCode = "-";
                    var accountToCredit = _content.acctIndex.SingleOrDefault(m => m.accountName == form.FundCapitalGL.glReceiving);
                    accountToCredit.amountInAcct = accountToCredit.amountInAcct + form.FundCapitalGL.amountCredit;
                    _content.fundGL.Add(form.FundCapitalGL);
                    _content.SaveChanges();
                    TempData["Success"] = "Capital GL Acconut funded Successful";      //Display a success message to the user
                    return RedirectToAction("fundCapitalGL", "Transact");
                }
                else
                {
                    TempData["Error"] = "Amount to debit must be equals to amount to credit";       //Display an error message to the user
                    return View("fundCapitalGL", form);
                }
            }
        }
    }
}