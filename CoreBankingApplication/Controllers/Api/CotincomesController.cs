using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CoreBankingApplication.Models;
using System.Net.Http;
using System.Web.Http;

namespace CoreBankingApplication.Controllers.Api
{
    public class CotincomesController : ApiController
    {
        private ApplicationDbContext _content;

        public CotincomesController()
        {
            _content = new ApplicationDbContext();
        }

        [HttpDelete]
        public void cincome(int id)
        {
            var record = _content.cotI.SingleOrDefault(m => m.id == id);

            if (record == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.cotI.Remove(record);
            }
            _content.SaveChanges();
        }
    }
}
