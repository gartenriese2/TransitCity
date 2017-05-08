using System;
using System.Collections.Generic;
using Geometry;
using Geometry.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeometryUnitTest
{
    [TestClass]
    public class ShapeUnitTests
    {
        [TestMethod]
        public void TestPolygonConstructor()
        {
            var vertices = new List<Position2d> { new Position2d(0,0), new Position2d(1, 0), new Position2d(0.5f, 1) };
            var polygon = new Polygon(vertices);
            Assert.IsNotNull(polygon);
        }

        [TestMethod]
        public void TestPolygonArea()
        {
            var vertices = new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(0.5f, 1) };
            var polygon = new Polygon(vertices);
            Assert.AreEqual(polygon.Area, 0.5f, float.Epsilon);
        }

        [TestMethod]
        public void TestPolygonArea2()
        {
            var vertices = new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(1, 1), new Position2d(0, 1) };
            var polygon = new Polygon(vertices);
            Assert.AreEqual(polygon.Area, 1, 0.000001);
        }

        [TestMethod]
        public void TestPolygonArea3()
        {
            var vertices = new List<Position2d>
            {
                new Position2d(0, 1),
                new Position2d(1, 0),
                new Position2d(2, -0.5f),
                new Position2d(3, 0),
                new Position2d(4, 1),
                new Position2d(3, 2),
                new Position2d(2, 2.5f),
                new Position2d(1, 2)
            };
            var polygon = new Polygon(vertices);
            Assert.AreEqual(polygon.Area, 7, 0.000001);
        }

        [TestMethod]
        public void TestPolygonBounds()
        {
            var vertices = new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(0.5f, 1) };
            var polygon = new Polygon(vertices);
            var (min, max) = polygon.Bounds;
            Assert.IsTrue(min.EqualPosition(new Position2d(0, 0)));
            Assert.IsTrue(max.EqualPosition(new Position2d(1, 1)));
        }

        [TestMethod]
        public void TestPolygonBounds2()
        {
            var vertices = new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(1, 1), new Position2d(0, 1) };
            var polygon = new Polygon(vertices);
            var (min, max) = polygon.Bounds;
            Assert.IsTrue(min.EqualPosition(new Position2d(0, 0)));
            Assert.IsTrue(max.EqualPosition(new Position2d(1, 1)));
        }

        [TestMethod]
        public void TestPolygonBounds3()
        {
            var vertices = new List<Position2d>
            {
                new Position2d(0, 1),
                new Position2d(1, 0),
                new Position2d(2, -0.5f),
                new Position2d(3, 0),
                new Position2d(4, 1),
                new Position2d(3, 2),
                new Position2d(2, 2.5f),
                new Position2d(1, 2)
            };
            var polygon = new Polygon(vertices);
            var (min, max) = polygon.Bounds;
            Assert.IsTrue(min.EqualPosition(new Position2d(0, -0.5f)));
            Assert.IsTrue(max.EqualPosition(new Position2d(4, 2.5f)));
        }

        [TestMethod]
        public void TestPolygonPointInside()
        {
            var vertices = new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(1, 1), new Position2d(0, 1) };
            var polygon = new Polygon(vertices);
            Assert.IsTrue(polygon.IsPointInside(new Position2d(0.5f, 0.5f)));
        }

        [TestMethod]
        public void TestPolygonPointInside2()
        {
            var vertices = new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(1, 1), new Position2d(0, 1) };
            var polygon = new Polygon(vertices);
            Assert.IsTrue(polygon.IsPointInside(new Position2d(0, 0)));
        }

        [TestMethod]
        public void TestPolygonPointInside3()
        {
            var vertices = new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(1, 1), new Position2d(0, 1) };
            var polygon = new Polygon(vertices);
            Assert.IsTrue(polygon.IsPointInside(new Position2d(0, 0.5f)));
        }

        [TestMethod]
        public void TestPolygonPointInside4()
        {
            var vertices = new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(1, 1), new Position2d(0, 1) };
            var polygon = new Polygon(vertices);
            Assert.IsFalse(polygon.IsPointInside(new Position2d(-1, 0)));
        }

        [TestMethod]
        public void TestPolygonRandomPoint()
        {
            var vertices = new List<Position2d> { new Position2d(0, 0), new Position2d(1, 0), new Position2d(1, 1), new Position2d(0, 1) };
            var polygon = new Polygon(vertices);
            var rnd = new Random();
            for (var i = 0; i < 1000; ++i)
            {
                var point = polygon.CreateRandomPoint(rnd);
                Assert.IsTrue(polygon.IsPointInside(point));
            }
        }
    }
}
