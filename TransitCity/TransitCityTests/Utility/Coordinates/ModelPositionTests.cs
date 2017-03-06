namespace TransitCityTests.Utility.Coordinates
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TransitCity.Utility.Coordinates;

    [TestClass]
    public class ModelPositionTests
    {
        [TestMethod]
        public void CreateRandomTestXAtLeastZero()
        {
            var pos = ModelPosition.CreateRandom();
            Assert.IsTrue(pos.X >= 0.0);
        }

        [TestMethod]
        public void CreateRandomTestXAtMostOne()
        {
            var pos = ModelPosition.CreateRandom();
            Assert.IsTrue(pos.X <= 1.0);
        }

        [TestMethod]
        public void CreateRandomTestYAtLeastZero()
        {
            var pos = ModelPosition.CreateRandom();
            Assert.IsTrue(pos.Y >= 0.0);
        }

        [TestMethod]
        public void CreateRandomTestYAtMostOne()
        {
            var pos = ModelPosition.CreateRandom();
            Assert.IsTrue(pos.Y <= 1.0);
        }

        [TestMethod]
        public void CreateRandomBetweenTestXAtLeastMin()
        {
            var minX = 0.3;
            var minY = 0.3;
            var maxX = 0.7;
            var maxY = 0.7;
            var pos = ModelPosition.CreateRandomBetween(new ModelPosition(minX, minY), new ModelPosition(maxX, maxY));
            Assert.IsTrue(pos.X >= minX);
        }

        [TestMethod]
        public void CreateRandomBetweenTestXAtMostMax()
        {
            var minX = 0.3;
            var minY = 0.3;
            var maxX = 0.7;
            var maxY = 0.7;
            var pos = ModelPosition.CreateRandomBetween(new ModelPosition(minX, minY), new ModelPosition(maxX, maxY));
            Assert.IsTrue(pos.X <= maxX);
        }

        [TestMethod]
        public void CreateRandomBetweenTestYAtLeastMin()
        {
            var minX = 0.3;
            var minY = 0.3;
            var maxX = 0.7;
            var maxY = 0.7;
            var pos = ModelPosition.CreateRandomBetween(new ModelPosition(minX, minY), new ModelPosition(maxX, maxY));
            Assert.IsTrue(pos.Y >= minY);
        }

        [TestMethod]
        public void CreateRandomBetweenTestYAtMostMax()
        {
            var minX = 0.3;
            var minY = 0.3;
            var maxX = 0.7;
            var maxY = 0.7;
            var pos = ModelPosition.CreateRandomBetween(new ModelPosition(minX, minY), new ModelPosition(maxX, maxY));
            Assert.IsTrue(pos.Y <= maxY);
        }

        [TestMethod]
        public void CreateRandomBetweenFirstPosNull()
        {
            try
            {
                ModelPosition.CreateRandomBetween(null, new ModelPosition(1.0, 1.0));
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void CreateRandomBetweenSecondPosNull()
        {
            try
            {
                ModelPosition.CreateRandomBetween(new ModelPosition(0.0, 0.0), null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void CreateRandomBetweenLowerleftBiggerThanUpperright()
        {
            try
            {
                ModelPosition.CreateRandomBetween(new ModelPosition(1.0, 1.0), new ModelPosition(0.0, 0.0));
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }
    }
}