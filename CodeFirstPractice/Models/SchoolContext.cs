using CodeFirstPractice.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstPractice
{
    public class SchoolContext : DbContext
    {
        public SchoolContext()
            : base()
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Standard> Standards { get; set; }

    }
}
