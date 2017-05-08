using System;
using System.Collections.Generic;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeometryUnitTest
{
    [TestClass]
    public class PathUnitTests
    {
        [TestMethod]
        public void TestLerp1()
        {
            var path = new Path(new List<Position2d> {new Position2d(0, 0), new Position2d(1, 0)});
            var pos1 = path.Lerp(0.5);
            Assert.IsTrue(Math.Abs(0.5 - pos1.X) < double.Epsilon);
            Assert.IsTrue(Math.Abs(0.0 - pos1.Y) < double.Epsilon);
            var pos2 = path.Lerp(0.01);
            Assert.IsTrue(Math.Abs(0.01 - pos2.X) < double.Epsilon);
            Assert.IsTrue(Math.Abs(0.0 - pos2.Y) < double.Epsilon);
            var pos3 = path.Lerp(0.99999);
            Assert.IsTrue(Math.Abs(0.99999 - pos3.X) < double.Epsilon);
            Assert.IsTrue(Math.Abs(0.0 - pos3.Y) < double.Epsilon);
        }

        [TestMethod]
        public void TestLerp2()
        {
            var path = new Path(new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(2, 0) });
            var pos1 = path.Lerp(0.5);
            Assert.IsTrue(Math.Abs(1.0 - pos1.X) < double.Epsilon);
            var pos2 = path.Lerp(0.01);
            Assert.IsTrue(Math.Abs(0.02 - pos2.X) < double.Epsilon);
            var pos3 = path.Lerp(0.99999);
            Assert.IsTrue(Math.Abs(1.99998 - pos3.X) < double.Epsilon);
        }

        [TestMethod]
        public void TestLerp3()
        {
            var path = new Path(new List<Position2d> { new Position2d(0, 0), new Position2d(0.5, 0), new Position2d(2, 0) });
            var pos1 = path.Lerp(0.5);
            Assert.IsTrue(Math.Abs(1.0 - pos1.X) < double.Epsilon);
            var pos2 = path.Lerp(0.01);
            Assert.IsTrue(Math.Abs(0.02 - pos2.X) < double.Epsilon);
            var pos3 = path.Lerp(0.99999);
            Assert.IsTrue(Math.Abs(1.99998 - pos3.X) < double.Epsilon);
        }

        [TestMethod]
        public void TestLerp4()
        {
            var path = new Path(new List<Position2d> { new Position2d(0, 0), new Position2d(0.5, 0), new Position2d(0.6, 0), new Position2d(0.601, 0), new Position2d(2, 0) });
            var pos1 = path.Lerp(0.5);
            Assert.IsTrue(Math.Abs(1.0 - pos1.X) < double.Epsilon);
            var pos2 = path.Lerp(0.01);
            Assert.IsTrue(Math.Abs(0.02 - pos2.X) < double.Epsilon);
            var pos3 = path.Lerp(0.99999);
            Assert.IsTrue(Math.Abs(1.99998 - pos3.X) < double.Epsilon);
        }

        [TestMethod]
        public void TestLerp5()
        {
            var path = new Path(new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(1, 1) });
            var pos1 = path.Lerp(0.5);
            Assert.IsTrue(Math.Abs(1.0 - pos1.X) < double.Epsilon);
            Assert.IsTrue(Math.Abs(0.0 - pos1.Y) < double.Epsilon);
            var pos2 = path.Lerp(0.01);
            Assert.IsTrue(Math.Abs(0.02 - pos2.X) < double.Epsilon);
            Assert.AreEqual(0.0, pos2.Y, double.Epsilon);
            var pos3 = path.Lerp(0.9999);
            Assert.AreEqual(1.0, pos3.X, double.Epsilon);
            Assert.AreEqual(0.9998, pos3.Y, double.Epsilon);
        }
    }
}
