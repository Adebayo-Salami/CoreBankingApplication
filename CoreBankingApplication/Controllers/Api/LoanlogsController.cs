using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CoreBankingApplication.Models;
using System.Net.Http;
using System.Web.Http;

namespace CoreBankingApplication.Controllers.Api
{
    public class LoanlogsController : ApiController
    {
        private ApplicationDbContext _content;

        public LoanlogsController()
        {
            _content = new ApplicationDbContext();
        }

        [HttpDelete]
        public void DeleteRecord(int id)
        {
            var record = _content.loanI.SingleOrDefault(m => m.id == id);

            if (record == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.loanI.Remove(record);
            }
            _content.SaveChanges();
        }
    }
}
