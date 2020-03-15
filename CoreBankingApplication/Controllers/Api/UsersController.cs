using System;
using System.Collections.Generic;
using System.Linq;
using CoreBankingApplication.Core.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CoreBankingApplication.Controllers.Api
{
    public class UsersController : ApiController
    {
        private CoreBankingApplication.Data.ApplicationDbContext _content;

        public UsersController()
        {
            _content = new CoreBankingApplication.Data.ApplicationDbContext();
        }

        [HttpDelete]
        public void DeleteUser(string id)
        {
            var user = _content.Users.SingleOrDefault(m => m.Id.ToString() == id);
            if (user == null)
            {
                throw new HttpRequestException(HttpStatusCode.NotFound.ToString());
            }
            else
            {
                _content.Users.Remove(user);
            }
            var tillAccount = user.tillAccount;
            var tellerDetails = _content.tellerDetails.SingleOrDefault(m => m.tillAccountNumber == tillAccount);
            if (tellerDetails != null)
            {
                var count = tellerDetails.tellerUsername.Count();
                tellerDetails.tellerUsername = "UNASSIGNED" + (count + 1) + "";
                tellerDetails.tillStatus = false;
            }

            _content.SaveChanges();
        }
    }
}