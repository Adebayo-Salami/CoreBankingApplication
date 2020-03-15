using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CoreBankingApplication.Models;
using System.Net.Http;
using System.Web.Http;

namespace CoreBankingApplication.Controllers.Api
{
    public class SinterestsController : ApiController
    {
        private ApplicationDbContext _content;

        public SinterestsController()
        {
            _content = new ApplicationDbContext();
        }

        [HttpDelete]
        public void DeleteLog(int id)
        {
            var record = _content.SIlog.SingleOrDefault(m => m.id == id);

            if (record == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.SIlog.Remove(record);
            }
            _content.SaveChanges();
        }
    }
}
