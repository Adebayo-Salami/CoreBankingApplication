﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CoreBankingApplication.Controllers.SwitchApi
{
    public class BalanceInquiryController : ApiController
    {
        public string[] Get()
        {
            return new string[]
            {
                "Hello",
                "World"
            };
        }
    }
}
