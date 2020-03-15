using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using CoreBankingApplication.Models;
using System.Web.Http;

namespace CoreBankingApplication.Controllers.Api
{
    [Authorize]
    public class CinterestsController : ApiController
    {
        private ApplicationDbContext _content;

        public CinterestsController()
        {
            _content = new ApplicationDbContext();
        }

        [HttpDelete]
        public void cinterests(int id)
        {
            var record = _content.CIlog.SingleOrDefault(m => m.id == id);

            if (record == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.CIlog.Remove(record);
            }
            _content.SaveChanges();
        }
    }
}
