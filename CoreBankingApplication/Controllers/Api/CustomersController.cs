using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CoreBankingApplication.Core.Models;
using System.Net.Http;
using System.Web.Http;
using CoreBankingApplication.Data;

namespace CoreBankingApplication.Controllers.Api
{ 
    public class CustomersController : ApiController
    {
        private CoreBankingApplication.Data.ApplicationDbContext _content;

        public CustomersController()
        {
            _content = new CoreBankingApplication.Data.ApplicationDbContext();
        }

        [HttpDelete]
        [Authorize(Roles = RoleName.admin)]
        public void DeleteCustomer(int id)
        {
            var customerInDB = _content.custom.SingleOrDefault(m => m.id.ToString() == id.ToString());
            var savingsAcct = _content.acctIndex.SingleOrDefault(m => m.id == 1);
            var currentAcct = _content.acctIndex.SingleOrDefault(m => m.id == 3);

            if (customerInDB == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                var customerAccountsInDB = _content.customAccts.ToList();
                foreach (var item in customerAccountsInDB)
                {
                    if (item.CustomerID == customerInDB.id)
                    {
                        if (item.customerAccountType == savingsAcct.accountName)
                        {
                            var interestAcct = _content.SIlog.SingleOrDefault(m => m.customerAccNumb == item.customerAccountNumber && m.customerAcctype == item.customerAccountType);
                            if (interestAcct != null)
                            {
                                _content.SIlog.Remove(interestAcct);
                            }
                        }

                        if (item.customerAccountType == currentAcct.accountName)
                        {
                            var interestAcct = _content.CIlog.SingleOrDefault(m => m.customerAccNumb == item.customerAccountNumber && m.customerAcctype == item.customerAccountType);
                            if (interestAcct != null)
                            {
                                _content.CIlog.Remove(interestAcct);
                            }
                        }

                        _content.customAccts.Remove(item);
                    }
                }

                _content.custom.Remove(customerInDB);
            }

            _content.SaveChanges();
        }

    }
}
