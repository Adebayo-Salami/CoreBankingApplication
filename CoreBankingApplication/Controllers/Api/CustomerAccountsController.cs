using System;
using System.Collections.Generic;
using System.Linq;
using CoreBankingApplication.Core.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CoreBankingApplication.Data;

namespace CoreBankingApplication.Controllers.Api
{
    [Authorize]
    public class CustomerAccountsController : ApiController
    {
        private CoreBankingApplication.Data.ApplicationDbContext _content;

        public CustomerAccountsController()
        {
            _content = new CoreBankingApplication.Data.ApplicationDbContext();
        }

        [HttpDelete]
        public void  DeleteAccount(int id)
        {
            var acctInDB = _content.customAccts.SingleOrDefault(m => m.id == id);
            var savingsAcct = _content.acctIndex.SingleOrDefault(m => m.id == 1);
            var currentAcct = _content.acctIndex.SingleOrDefault(m => m.id == 3);
            if(acctInDB == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.customAccts.Remove(acctInDB);

                if(acctInDB.customerAccountType == savingsAcct.accountName)
                {
                    var interestAcct = _content.SIlog.SingleOrDefault(m => m.customerAccNumb == acctInDB.customerAccountNumber && m.customerAcctype == acctInDB.customerAccountType);
                    if(interestAcct != null)
                    {
                        _content.SIlog.Remove(interestAcct);
                    }
                }

                if(acctInDB.customerAccountType == currentAcct.accountName)
                {
                    var interestAcct = _content.CIlog.SingleOrDefault(m => m.customerAccNumb == acctInDB.customerAccountNumber && m.customerAcctype == acctInDB.customerAccountType);
                    if (interestAcct != null)
                    {
                        _content.CIlog.Remove(interestAcct);
                    }
                }
                
            }

            _content.SaveChanges();
        }
    }
}
