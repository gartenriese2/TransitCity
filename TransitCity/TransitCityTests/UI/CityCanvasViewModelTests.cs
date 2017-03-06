namespace TransitCityTests.UI
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TransitCity.Models;
    using TransitCity.UI;
    using TransitCity.Utility.Coordinates;

    [TestClass]
    public class CityCanvasViewModelTests
    {
        [TestMethod]
        public void GetViewModelFromModelTestNullArgument()
        {
            var vm = new CityCanvasViewModel(null, null);
            var privateObj = new PrivateObject(vm);
            var ret = privateObj.Invoke("GetViewModelFromModel", null);
            Assert.AreEqual(ret, null);
        }

        [TestMethod]
        public void GetViewModelFromModelTestNullCityModel()
        {
            var privateObj = new PrivateObject(new CityCanvasViewModel(null, null));
            var ret = privateObj.Invoke("GetViewModelFromModel", new MetroStationModel(new ModelPosition(0, 0)));
            Assert.AreEqual(ret, null);
        }
    }
}