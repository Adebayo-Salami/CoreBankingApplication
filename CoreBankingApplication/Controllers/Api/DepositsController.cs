using System;
using System.Collections.Generic;
using System.Linq;
using CoreBankingApplication.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CoreBankingApplication.Controllers.Api
{
    public class DepositsController : ApiController
    {
        private ApplicationDbContext _content;

        public DepositsController()
        {
            _content = new ApplicationDbContext();
        }

        [HttpDelete]
        public void DeleteLog(int id)
        {
            var log = _content.postD.SingleOrDefault(m => m.id == id);

            if (log == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.postD.Remove(log);
            }
            _content.SaveChanges();
        }
    }
}
