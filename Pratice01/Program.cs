using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pratice01
{
    /// <summary>
    /// 配合"使用 db.ChangeTracker.Entities() 實作在修改資料後 SaveChanges() 時自動填入 ModifiedOn 欄位的內容"新增的partial class, 對SaveChanges作擴充
    /// </summary>
    public partial class ContosoUniversityEntities : DbContext
    {
        public override int SaveChanges()
        {
            var entries = this.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                Console.WriteLine("Entry name : {0}", entry.Entity.GetType().FullName);
                Console.WriteLine(entry.Entity.GetType().FullName.Contains("Course")? "This is a course table" : "This is not course table");

                if (entry.State == System.Data.Entity.EntityState.Modified)
                {
                    entry.CurrentValues.SetValues(new { ModifyOn = DateTime.Now});
                }
            }

            return base.SaveChanges();
        }
    }

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
            Practice12();
        }

        /// <summary>
        /// 練習 Entity Framework 使用 檢視表 的各種注意事項
        /// </summary>
        static private void Practice12()
        {
            using (var db = new ContosoUniversityEntities())
            {
                foreach (var course in db.vwCourse)
                {
                    Console.WriteLine(course.Title);
                }
            }
        }

        //---------------Day2--------------
        /// <summary>
        /// 目的: 練習將int轉成enum
        /// </summary>
        static private void Practice11()
        {
            using (var db = new ContosoUniversityEntities())
            {
                var courses = db.Course;

                foreach (var course in courses)
                {
                    Console.WriteLine(course.Credits + "\t" + course.Title);
                }
            }
        }

        /// <summary>
        /// 以store procedure取得要查尋的title的course - 練習 Entity Framework 的 Stored Procedure 匯入與設定
        /// </summary>
        static private void Practice10()
        {
            using (var db = new ContosoUniversityEntities())
            {
                var courses = db.GetCourse("%GIT%");

                foreach (var course in courses)
                {
                    Console.WriteLine(course.DepartmentName + "\t" + course.Title);
                }
            }
        }

        /// <summary>
        /// 目的: 測試使用timestamp的並行模式 - 練習 Entity Framework 的並行模式 (Concurrency Mode) 使用方式
        /// 準備:
        /// 1. DB中新增type為timestamp的RowVersion欄位
        /// 2. 更新資料庫模型
        /// 3. 將RowVersion的並行模式設為Fixed
        /// 測試
        /// 1. 將course某一筆的credit改為777, build成功後,將執行環境copy到某資料夾, 然後執行 -> exe1
        /// 2. 將course某一筆的credit改為666, build成功後執行 -> exe2
        /// 3. 在exe1的window按任一鍵, 先更新好credit
        /// 4. 在exe2的window按任一鍵, 會發生DbupdateConcurrency exception
        /// </summary>
        static private void Practice09()
        {
            using (var db = new ContosoUniversityEntities())
            {
                var course = db.Course.Find(1);
                course.Credits = CreditType.Low;

                Console.ReadKey();
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 目的: 使用離線模式修改資料
        /// </summary>
        static private void Practice08()
        {
            Course course;
            using (var db = new ContosoUniversityEntities())
            {
                course = db.Course.Find(1);
                course.Credits = CreditType.Low;
            }

            //即使attach了, 但沒有修改state, EF還是不會對DB作Update
            using (var db = new ContosoUniversityEntities())
            {
                db.Course.Attach(course);

                db.SaveChanges();
                Console.WriteLine("Before attached and change state, Credits={0}", db.Course.Find(1).Credits);
            }

            using (var db = new ContosoUniversityEntities())
            {
                db.Entry(course).State = System.Data.Entity.EntityState.Modified; //=db.Course.Attach(course);
                db.SaveChanges();

                Console.WriteLine("After attached, Credits={0}", db.Course.Find(1).Credits);
            }
        }

        /// <summary>
        /// 目的: 使用 db.ChangeTracker.Entities() 實作在修改資料後 SaveChanges() 時自動填入 ModifiedOn 欄位的內容
        /// </summary>
        static private void Practice07()
        {
            using (var db = new ContosoUniversityEntities())
            {
                var course = db.Course.Find(1);

                course.Credits = CreditType.Low;

                var entry = db.Entry(course);

                Console.WriteLine("Original Value=" + entry.OriginalValues.GetValue<int>("Credits"));
                Console.WriteLine("Current Value=" + entry.CurrentValues.GetValue<int>("Credits"));

                Console.WriteLine("State:" + "\t" + entry.State);

                db.SaveChanges();
            }

        }

        /// <summary>
        /// 目的: 使用 db.Entry 取得實體物件(Entity)的內部資訊，包含實體狀態(State)與原始值(OriginalValue)
        /// </summary>
        static private void Practice06()
        {
            using (var db = new ContosoUniversityEntities())
            {
                var course = db.Course.Find(1);

                course.Credits = CreditType.Low;

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
                    Credits = CreditType.Low,
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
