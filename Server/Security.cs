using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Security
{
    class Security : SingleInstance<Security>, IClose
    {
        private Account? m_FindAccount(string id, string password)
        {
            Thread.Sleep(1000);
            if (id == "111" && password == "222")
            {
                return new Account("111", "lyl", Privilege.Admin);
            }
            return null;
        }

        static public Account? FindAccount(string id, string password)
        {
            return s_Instance.m_FindAccount(id, password);
        }

        void IClose.Close()
        {

        }
    }
}
