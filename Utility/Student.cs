using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class Scores
    {
        public decimal C1 { get; set;}
        public decimal C2 { get; set;}
        public decimal C3 { get; set;}
        public decimal C4 { get; set;}
        public decimal C5 { get; set;}
        public decimal C6 { get; set;}

        public Scores()
        {

        }
        public Scores(decimal c1, decimal c2, decimal c3, decimal c4, decimal c5, decimal c6)
        {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
            C5 = c5;
            C6 = c6;
        }
    }

    public class Student
    {
        //准考证号
        public string ExamineeNumber { get; set; }

        //姓名
        public string Name { get; set; }

        //身份证号
        public string IDNumber { get; set; }

        //各科成绩
        public Scores Scores { get; set; }

        public Student()
        {
            ExamineeNumber = "unknow";
            Name = "unknow";
            IDNumber = "unknow";
            Scores = new Scores();
        }
        public Student(string examineeNumber, string name, string iDNumber, Scores scores)
        {
            ExamineeNumber = examineeNumber;
            Name = name;
            IDNumber = iDNumber;
            Scores = scores;
        }
    }
}
