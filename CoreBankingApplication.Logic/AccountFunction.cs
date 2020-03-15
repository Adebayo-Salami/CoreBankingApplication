using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Logic
{
    public class AccountFunction
    {
        public string AccountNumber(string code, int id)
        {
            var form1 = code;
            Random rn = new Random();
            int form2 = rn.Next(00000, 99999);
            var form3 = id;
            var accountN = form1 + "" + form2 + "" + form3;
            return accountN.ToString();
        }

        public double ConvertNegativeNumber(double number)
        {
            var positiveNumber = number * -1;
            return positiveNumber;
        }
    }
}