using System.Data.Entity;
using SQLite.CodeFirst;

namespace Database
{
    public class TransitDatabase : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<TransitDatabase>(modelBuilder);
            System.Data.Entity.Database.SetInitializer(sqliteConnectionInitializer);
        }

        public DbSet<Person> Persons { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
