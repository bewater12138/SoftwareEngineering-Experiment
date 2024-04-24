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
    internal partial class SQL
    {
        //查询学生信息
        static public Student? InqueryStudent(
            string examinee_number, //准考证号
            string id_number        //身份证号
            )
        {
            return null;
        }

        //查询账号
        static public Account? InqueryAccount(string id,string password)
        {
            return null;
        }

        //插入学生信息
        static public bool InsertStudent(Student stu)
        {
            return false;
        }

        //修改学生信息
        static public bool ModifyStudent(string id_number, Student new_info)
        {
            return false;
        }

        //注册账号
        static public bool RegisterAccount(Account acc)
        {
            return false;
        }

        //查找录取学生
        static public List<Student> FindRecruitStudents()
        {
            return new List<Student>();
        }
    }

    internal partial class SQL:SingleInstance<SQL>,IClose
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
                Log.LogError("sqlserver 连接失败");
            }

        }

        void IClose.Close()
        {
            s_Instance?.connection.Close();
        }
    }
}
