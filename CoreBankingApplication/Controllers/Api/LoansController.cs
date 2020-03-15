using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using CoreBankingApplication.Models;
using System.Web.Http;

namespace CoreBankingApplication.Controllers.Api
{
    public class LoansController : ApiController
    {
        private ApplicationDbContext _content;

        public LoansController()
        {
            _content = new ApplicationDbContext();
        }

        [HttpDelete]
        public void DeleteLog(int id)
        {
            var log = _content.loanPay.SingleOrDefault(m => m.id == id);

            if (log == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.loanPay.Remove(log);
            }
            _content.SaveChanges();
        }
    }
}
