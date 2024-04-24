using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    static public class AccountFormat
    {
        static public bool CheckName(string name)
        {
            return name.Length > 0 && name.Length <= 10;
        }

        static public bool CheckAccount(string account) 
        {
            if (account.Length < 6 || account.Length > 10) return false;

            foreach (var c in account)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        static public bool CheckPassword(string password)
        {
            if (password.Length < 6 || password.Length > 10) return false;

            foreach(var c in  password)
            {
                if(!char.IsAsciiLetter(c) &&
                    !char.IsPunctuation(c)&&
                    !char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
