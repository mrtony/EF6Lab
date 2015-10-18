using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFirstPractice.Models;

namespace CodeFirstPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            Practice_14();
        }

        /// <summary>
        /// 實作 Entity Framework Code First 專案，並找到被自動建立的資料庫
        /// http://www.entityframeworktutorial.net/code-first/simple-code-first-example.aspx
        /// </summary>
        private static void Practice_14()
        {
            using (var db = new SchoolContext())
            {
                var standard = new Standard()
                {
                    StandardName = "國小",
                };

                standard.Students.Add(new Student()
                {
                    StudentName = "Paul",
                    Height = 170,
                    Weight = 60,
                    DateOfBirth = DateTime.Now
                });

                standard.Students.Add(new Student()
                {
                    StudentName = "Mark",
                    Height = 180,
                    Weight = 80,
                    DateOfBirth = DateTime.Now
                });

                db.Standards.Add(standard);
                db.SaveChanges();

            }
        }
    }
}
