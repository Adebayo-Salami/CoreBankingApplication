using System;
using System.Collections.Generic;
using System.Linq;
using CoreBankingApplication.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CoreBankingApplication.Controllers.Api
{
    [Authorize]
    public class AccountsController : ApiController
    {
        private ApplicationDbContext _content;

        public AccountsController()
        {
            _content = new ApplicationDbContext();
        }

        [HttpDelete]
        public void DeleteAccount(int id)
        {
            var acct = _content.acctIndex.SingleOrDefault(m => m.id == id);

            if(acct == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.acctIndex.Remove(acct);
            }

            _content.SaveChanges();
        }
    }
}
