using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Server
{
    internal class SQL:SingleInstance<SQL>,IClose
    {
        static public bool IsOpen => s_Instance != null && s_Instance.connection.State == System.Data.ConnectionState.Open;
        
        SqlConnectionStringBuilder sqlConnectionString = new SqlConnectionStringBuilder();
        SqlConnection connection;
        public SQL() 
        {
            sqlConnectionString.DataSource = "127.0.0.1";
            sqlConnectionString.UserID = "sa";
            sqlConnectionString.Password = "2694565268";
            sqlConnectionString.InitialCatalog = "student";

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
