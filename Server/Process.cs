using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Server
{
    //业务处理
    internal class Process
    {
        //查询学生信息
        static public Student? InqueryStudent(
            string examinee_number, //准考证号
            string id_number        //身份证号
            )
        {
            return SQL.InqueryStudent(examinee_number, id_number);
        }

        //查询账号
        static public Account? InqueryAccount(string id, string password)
        {
            return SQL.InqueryAccount(id, password);
        }

        //插入学生信息
        static public bool InsertStudent(Student stu)
        {
            return SQL.InsertStudent(stu);
        }

        //修改学生信息
        static public bool ModifyStudent(string id_number, Student new_info)
        {
            return SQL.ModifyStudent(id_number, new_info);
        }

        //注册账号
        static public bool RegisterAccount(Account acc)
        {
            return SQL.RegisterAccount(acc);
        }

        //查找录取学生
        static public List<Student> FindRecruitStudents()
        {
            return SQL.FindRecruitStudents();
        }
    }
}
