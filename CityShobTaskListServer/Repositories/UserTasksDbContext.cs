using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CityShobTaskListServer
{
    public partial class UserTasksDbContext : DbContext
    {
        public UserTasksDbContext()
            : base("name=UserTasksDbContext")
        {
        }

        public virtual DbSet<UserTasks> UserTasks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
