using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Logic
{
    public class TillGenerate
    {
        public string GetTillCode(string code)
        {
            var joinTillId = code;
            Random rn = new Random();
            int generaterTill = rn.Next(000000, 999999);
            var till = joinTillId + "" + generaterTill.ToString();
            return till.ToString();
        }

    }
}