using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CoreBankingApplication.Core.Models;

namespace CoreBankingApplication.Controllers.Api
{
    [Authorize]
    public class TellersController : ApiController
    {
        private CoreBankingApplication.Data.ApplicationDbContext _content;

        public TellersController()
        {
            _content = new CoreBankingApplication.Data.ApplicationDbContext();
        }

        //DELETE /api/tellers/id
        [HttpDelete]
        public void DeleteTeller(string id)
        {
            var teller = _content.Users.SingleOrDefault(m => m.tillAccount == id);
            if (teller == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.Users.Remove(teller);
            }

            var tellerDetails = _content.tellerDetails.SingleOrDefault(m => m.tillAccountNumber.ToString() == id);
            if(tellerDetails == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.tellerDetails.Remove(tellerDetails);
            } 

            _content.SaveChanges();
        }
    }
}
