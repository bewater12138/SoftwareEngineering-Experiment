using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Server.Config;

namespace Server
{
    internal class SQL:SingleInstance<SQL>,IClose
    {
        static public bool IsOpen => s_Instance != null && s_Instance.connection.State == System.Data.ConnectionState.Open;
        
        SqlConnectionStringBuilder sqlConnectionString = new SqlConnectionStringBuilder();
        SqlConnection connection;
        public SQL() 
        {
            sqlConnectionString.DataSource = SQLConfig.DataSource;
            sqlConnectionString.UserID = SQLConfig.UserID;
            sqlConnectionString.Password = SQLConfig.Password;
            sqlConnectionString.InitialCatalog = SQLConfig.InitialCatalog;

            var constring = sqlConnectionString.ToString();
            connection = new SqlConnection(constring);
            connection.Open();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                Log.LogInfo("sqlserver 连接成功");
            }
            else
            {
                Log.LogInfo("sqlserver 连接失败");
            }

        }

        void IClose.Close()
        {
            s_Instance?.connection.Close();
        }
    }
}
