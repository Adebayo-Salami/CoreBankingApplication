using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreBankingApplication.Logic
{
    public class PasswordGenerate
    {
        public string GetNewPassword()
        {
            var PasswordLenth = 8;
            string allowedChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*>?";
            Random randChars = new Random();
            char[] chars = new char[PasswordLenth];
            int allowedCharC = allowedChars.Length;
            for (int i = 0; i < PasswordLenth; i++)
            {
                chars[i] = allowedChars[(int)((allowedChars.Length) * randChars.NextDouble())];
            }
            var password = new string(chars);
            return password.ToString();
        }
    }
}