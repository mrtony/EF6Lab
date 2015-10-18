using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pratice01
{
    public class Course2
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public string DepartmentName { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Practice06();
        }

        //---------------Day2--------------
        static private void Practice06()
        {
            using (var db = new ContosoUniversityEntities())
            {
                var course = db.Course.Find(1);

                course.Credits = 100;

                var entry = db.Entry(course);

                Console.WriteLine("Original Value=" + entry.OriginalValues.GetValue<int>("Credits"));
                Console.WriteLine("Current Value=" + entry.CurrentValues.GetValue<int>("Credits"));

                Console.WriteLine("State:" + "\t" + entry.State);
            }

        }


        //---------------Day1--------------

        /// <summary>
        /// 使用 db.Database.SqlQuery 執行任意查詢語法，並透過自訂類別取得資料
        /// 1. 使用SQL Management Tool的 "在編輯器中設計查詢", 直接有圖表來點選要輸出的資料,下方會輸出SQL command
        /// 2. 設計一個新的class以做為回傳資料的對應繫結
        /// 3. 將1的command複製後, 以db.Database.SqlQuery<T>以SQL command query來取得資料
        /// </summary>
        static private void Practice05()
        {
            var sql = @"SELECT Course.CourseID, Course.Title, Department.Name AS DepartmentName FROM Course INNER JOIN Department ON Course.DepartmentID = Department.DepartmentID";
            using (var db = new ContosoUniversityEntities())
            {
                var result = db.Database.SqlQuery<Course2>(sql);

                foreach (var item in result)
                {
                    Console.WriteLine(item.CourseID + "\t" + item.Title + "\t" + item.DepartmentName);
                }
            }
            
        }

        /// <summary>
        /// 練習多對多的新增方式
        /// Requirement: Course=7的課程沒有Instructor, 需要新增Person=5當成它的instructor
        /// Todo
        /// 1: 修改course導覽屬性的Person為instructors, 可讀性較高
        /// 2. 對course做新增instructor
        /// </summary>
        static private void Practice04()
        {
            using (var db = new ContosoUniversityEntities())
            {
                var course = db.Course.Find(7);
                course.Instructors.Add(db.Person.Find(5));

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 實驗StoreGeratedPattern (Identity, Computed, None)
        /// </summary>
        static private void Practice03()
        {
            using (var db = new ContosoUniversityEntities())
            {
                var c1 = new Course() 
                { 
                    Title = "Test",
                    DepartmentID = 1,
                    Credits = 6,
                    ModifyOn = DateTime.Now
                };

                db.Course.Add(c1);
                db.SaveChanges();

            }
        }

        /// <summary>
        /// 練習導覽屬性
        /// </summary>
        static private void Practice02()
        {
            using (var db = new ContosoUniversityEntities())
            {
                var result = from item in db.Department
                             select item;

                foreach (var department in result)
                {
                    Console.WriteLine(department.DepartmentID + "\t" + department.Name);
                    foreach (var course in department.Course)
                    {
                        Console.WriteLine("\t" + course.CourseID + "\t" + course.Title);
                    }
                }

            }
        }

        static private void Practice01()
        {
            using (var db = new ContosoUniversityEntities())
            {
                var result = from item in db.Course
                             where item.Title.Contains("Git")
                             select item;

                foreach (var item in result)
                {
                    Console.WriteLine(item.CourseID + "\t" + item.Title);
                }

            }
        }
    }
}
