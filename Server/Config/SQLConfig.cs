using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Config
{
    //后面会改成从配置文件中读取
    static internal class SQLConfig
    {
        static public string DataSource = "127.0.0.1";
        static public string UserID = "sa";
        static public string Password = "2694565268";
        static public string InitialCatalog = "student";
    }
}
