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
    public class ClosedAccountsController : ApiController
    {
        private CoreBankingApplication.Data.ApplicationDbContext _content;

        public ClosedAccountsController()
        {
            _content = new CoreBankingApplication.Data.ApplicationDbContext();
        }

        [HttpDelete]
        public void DeleteAccount(int id)
        {
            var acctInDB = _content.closedAcct.SingleOrDefault(m => m.id == id);
            if (acctInDB == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.closedAcct.Remove(acctInDB);
            }

            _content.SaveChanges();
        }
    }
}
