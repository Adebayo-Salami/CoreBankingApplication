using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using CoreBankingApplication.Models;
using System.Web.Http;

namespace CoreBankingApplication.Controllers.Api
{
    public class BanklogsController : ApiController
    {
        private ApplicationDbContext _content;

        public BanklogsController()
        {
            _content = new ApplicationDbContext();
        }

        [HttpDelete]
        public void DeleteRecord(int id)
        {
            var record = _content.bankL.SingleOrDefault(m => m.id == id);

            if (record == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.bankL.Remove(record);
            }
            _content.SaveChanges();
        }
    }
}
