using System.Linq;
using Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseUnitTest
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        public void RemoveAndAdd()
        {
            using (var db = new TransitDatabase())
            {
                db.Persons.RemoveRange(db.Persons.Where(x => true));

                var person = new Person { Name = "John" };
                db.Persons.Add(person);
                db.SaveChanges();

                var persons =
                    from a in db.Persons
                    where a.Name == "John"
                    select a.Id;
                var list = persons.ToList();
                Assert.AreEqual(list.Count, 1);
            }
        }
    }
}
