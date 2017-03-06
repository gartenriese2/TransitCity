namespace TransitCityTests.Pathfinding
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TransitCity.Pathfinding;

    [TestClass]
    public class TravellerTest
    {
        [TestMethod]
        public void CreateTestReusableTypesNotNull()
        {
            var t = Traveller.Create();
            Assert.AreNotEqual(null, t.ReusableTypes);
        }

        [TestMethod]
        public void CreateTestNonReusableTypesNotNull()
        {
            var t = Traveller.Create();
            Assert.AreNotEqual(null, t.NonReusableTypes);
        }

        [TestMethod]
        public void CreateTestAllTypesNotNull()
        {
            var t = Traveller.Create();
            Assert.AreNotEqual(null, t.AllTypes);
        }

        [TestMethod]
        public void CreateTestKeysNotNull()
        {
            var t = Traveller.Create();
            Assert.AreNotEqual(null, t.Keys);
        }
    }
}
