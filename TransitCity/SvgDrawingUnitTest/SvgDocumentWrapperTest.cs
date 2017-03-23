using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Svg;
using SvgDrawing;
using Transit;

namespace SvgDrawingUnitTest
{
    [TestClass]
    public class SvgDocumentWrapperTest
    {
        [TestMethod]
        public void SaveTest()
        {
            var document = new SvgDocumentWrapper(10000, 10000);
            var circle = new SvgCircle
            {
                CenterX = new SvgUnit(SvgUnitType.Pixel, 200),
                CenterY = new SvgUnit(SvgUnitType.Pixel, 200),
                Radius = new SvgUnit(SvgUnitType.Pixel, 100),
                Fill = new SvgColourServer(Color.Blue)
            };
            document.Add(circle);
            document.Save("test.svg");
        }

        [TestMethod]
        public void DrawLinesTest()
        {
            var document = new SvgDocumentWrapper(10000, 10000);

            var tsd = new Dictionary<string, TransferStation<Position2f>>();
            var t1 = CreateLine1(tsd);
            var t2 = CreateLine2(tsd);
            var t3 = CreateLine3(tsd);
            var t4 = CreateLine4(tsd);
            var lines = new List<Line<Position2f>>
            {
                t1.Item1,
                t2.Item1,
                t3.Item1,
                t4.Item1
            };

            foreach (var station in lines.SelectMany(line => line.Routes).SelectMany(route => route.Stations))
            {
                var c = new SvgCircle
                {
                    CenterX = station.Position.X,
                    CenterY = station.Position.Y,
                    Radius = 8f,
                    Fill = new SvgColourServer(Color.White),
                    Stroke = new SvgColourServer(Color.Black),
                    StrokeWidth = 2f
                };
                document.Add(c);
            }

            var r11 = new SvgPolyline
            {
                StrokeWidth = 4f,
                Stroke = new SvgColourServer(Color.Red),
                Points = new SvgPointCollection(),
                Fill = SvgPaintServer.None
            };
            foreach (var pos in t1.Item2.Item1)
            {
                r11.Points.Add(pos.X);
                r11.Points.Add(pos.Y);
            }
            document.Add(r11);
            var r12 = new SvgPolyline
            {
                StrokeWidth = 4f,
                Stroke = new SvgColourServer(Color.Red),
                Points = new SvgPointCollection(),
                Fill = SvgPaintServer.None
            };
            foreach (var pos in t1.Item2.Item2)
            {
                r12.Points.Add(pos.X);
                r12.Points.Add(pos.Y);
            }
            document.Add(r12);

            var r21 = new SvgPolyline
            {
                StrokeWidth = 4f,
                Stroke = new SvgColourServer(Color.DarkGreen),
                Points = new SvgPointCollection(),
                Fill = SvgPaintServer.None
            };
            foreach (var pos in t2.Item2.Item1)
            {
                r21.Points.Add(pos.X);
                r21.Points.Add(pos.Y);
            }
            document.Add(r21);
            var r22 = new SvgPolyline
            {
                StrokeWidth = 4f,
                Stroke = new SvgColourServer(Color.DarkGreen),
                Points = new SvgPointCollection(),
                Fill = SvgPaintServer.None
            };
            foreach (var pos in t2.Item2.Item2)
            {
                r22.Points.Add(pos.X);
                r22.Points.Add(pos.Y);
            }
            document.Add(r22);

            var r31 = new SvgPolyline
            {
                StrokeWidth = 4f,
                Stroke = new SvgColourServer(Color.DarkBlue),
                Points = new SvgPointCollection(),
                Fill = SvgPaintServer.None
            };
            foreach (var pos in t3.Item2.Item1)
            {
                r31.Points.Add(pos.X);
                r31.Points.Add(pos.Y);
            }
            document.Add(r31);
            var r32 = new SvgPolyline
            {
                StrokeWidth = 4f,
                Stroke = new SvgColourServer(Color.DarkBlue),
                Points = new SvgPointCollection(),
                Fill = SvgPaintServer.None
            };
            foreach (var pos in t3.Item2.Item2)
            {
                r32.Points.Add(pos.X);
                r32.Points.Add(pos.Y);
            }
            document.Add(r32);

            var r41 = new SvgPolyline
            {
                StrokeWidth = 4f,
                Stroke = new SvgColourServer(Color.Orange),
                Points = new SvgPointCollection(),
                Fill = SvgPaintServer.None
            };
            foreach (var pos in t4.Item2.Item1)
            {
                r41.Points.Add(pos.X);
                r41.Points.Add(pos.Y);
            }
            document.Add(r41);
            var r42 = new SvgPolyline
            {
                StrokeWidth = 4f,
                Stroke = new SvgColourServer(Color.Orange),
                Points = new SvgPointCollection(),
                Fill = SvgPaintServer.None
            };
            foreach (var pos in t4.Item2.Item2)
            {
                r42.Points.Add(pos.X);
                r42.Points.Add(pos.Y);
            }
            document.Add(r42);

            document.Save("lines.svg");
        }

        private static Station<Position2f> CreateStation(Position2f pos, string name, IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var station = new Station<Position2f>(pos);
            if (!tsd.ContainsKey(name))
            {
                tsd[name] = new TransferStation<Position2f>(name);
            }

            tsd[name].AddStation(station);
            return station;
        }

        private Tuple<Line<Position2f>, Tuple<List<Position2f>, List<Position2f>>> CreateLine1(Dictionary<string, TransferStation<Position2f>> tsd)
        {
            var (t1, t2) = CreateRoutes(new Dictionary<Position2f, string>
            {
                [new Position2f(1500, 1000)] = "Stanmore",
                [new Position2f(2500, 1000)] = "Canons Park",
                [new Position2f(3500, 1500)] = "Queensbury",
                [new Position2f(4000, 2500)] = "Kingsbury",
                [new Position2f(4500, 3500)] = "Wembley Park",
                [new Position2f(5500, 4000)] = "Neasden",
                [new Position2f(6500, 4000)] = "Clapham South",
                [new Position2f(7500, 4000)] = "Dollis Hill",
                [new Position2f(8000, 5000)] = "Willesden Green",
                [new Position2f(8000, 6000)] = "Kilburn",
                [new Position2f(7500, 7000)] = "West Hampstead",
                [new Position2f(7500, 8000)] = "Finchley Road",
                [new Position2f(8000, 9000)] = "Swiss Cottage",
            }, 240f, tsd);

            return new Tuple<Line<Position2f>, Tuple<List<Position2f>, List<Position2f>>>(new Line<Position2f>("1", t1.Item1, t2.Item1), new Tuple<List<Position2f>, List<Position2f>>(t1.Item2, t2.Item2));
        }

        private Tuple<Line<Position2f>, Tuple<List<Position2f>, List<Position2f>>> CreateLine2(Dictionary<string, TransferStation<Position2f>> tsd)
        {
            var (t1, t2) = CreateRoutes(new Dictionary<Position2f, string>
            {
                [new Position2f(1500, 8000)] = "Morden",
                [new Position2f(2500, 7500)] = "South Wimbledon",
                [new Position2f(3500, 7000)] = "Colliers Wood",
                [new Position2f(4500, 6500)] = "Tooting Broadway",
                [new Position2f(5500, 5500)] = "Tooting Bec",
                [new Position2f(6000, 4750)] = "Balham",
                [new Position2f(6500, 4000)] = "Clapham South",
                [new Position2f(7000, 3000)] = "Clapham Common",
                [new Position2f(7000, 2000)] = "Clapham North",
                [new Position2f(8000, 1500)] = "Stockwell",
                [new Position2f(9000, 1500)] = "Oval",
            }, 240f, tsd);

            return new Tuple<Line<Position2f>, Tuple<List<Position2f>, List<Position2f>>>(new Line<Position2f>("2", t1.Item1, t2.Item1), new Tuple<List<Position2f>, List<Position2f>>(t1.Item2, t2.Item2));
        }

        private Tuple<Line<Position2f>, Tuple<List<Position2f>, List<Position2f>>> CreateLine3(Dictionary<string, TransferStation<Position2f>> tsd)
        {
            var (t1, t2) = CreateRoutes(new Dictionary<Position2f, string>
            {
                [new Position2f(5500, 2000)] = "Chesham",
                [new Position2f(5500, 3000)] = "Amersham",
                [new Position2f(5500, 4000)] = "Neasden",
                [new Position2f(6000, 4750)] = "Balham",
                [new Position2f(6750, 5750)] = "Chalfont",
                [new Position2f(8000, 6000)] = "Latimer",
                [new Position2f(9000, 6250)] = "Watford",
                [new Position2f(9900, 6300)] = "Croxley"
            }, 120f, tsd);


            return new Tuple<Line<Position2f>, Tuple<List<Position2f>, List<Position2f>>>(new Line<Position2f>("3", t1.Item1, t2.Item1), new Tuple<List<Position2f>, List<Position2f>>(t1.Item2, t2.Item2));
        }

        private Tuple<Line<Position2f>, Tuple<List<Position2f>, List<Position2f>>> CreateLine4(Dictionary<string, TransferStation<Position2f>> tsd)
        {
            var (t1, t2) = CreateRoutes(new Dictionary<Position2f, string>
            {
                [new Position2f(500, 5500)] = "Epping",
                [new Position2f(1500, 5000)] = "Theydon Bois",
                [new Position2f(2500, 4500)] = "Debden",
                [new Position2f(3500, 4500)] = "Loughton",
                [new Position2f(4750, 5000)] = "Buckhurst Hill",
                [new Position2f(5900, 4900)] = "Balham",
                [new Position2f(7000, 4500)] = "Woodford",
                [new Position2f(7500, 4100)] = "Dollis Hill",
                [new Position2f(8500, 3500)] = "Roding Valley",
                [new Position2f(9600, 3600)] = "Chigwell"
            }, 180f, tsd);


            return new Tuple<Line<Position2f>, Tuple<List<Position2f>, List<Position2f>>>(new Line<Position2f>("3", t1.Item1, t2.Item1), new Tuple<List<Position2f>, List<Position2f>>(t1.Item2, t2.Item2));
        }

        private Tuple<Tuple<Route<Position2f>, List<Position2f>>, Tuple<Route<Position2f>, List<Position2f>>> CreateRoutes(Dictionary<Position2f, string> dic, float frequency, Dictionary<string, TransferStation<Position2f>> tsd)
        {
            var positions = dic.Keys.ToList();
            var spline = GetSpline(positions, 10);

            var routeAList = new List<Station<Position2f>>();
            var routeBList = new List<Station<Position2f>>();
            foreach (var b in positions)
            {
                var idx = spline.FindIndex(p => p == b);
                var a = idx == 0 ? b : spline[idx - 1];
                var c = idx == spline.Count - 1 ? b : spline[idx + 1];
                var (p1, p2) = GetOffsetPoints(a, b, c, 4f);

                var stationA = CreateStation(p1, dic[b], tsd);
                var stationB = CreateStation(p2, dic[b], tsd);
                routeAList.Add(stationA);
                routeBList.Add(stationB);
            }

            var routeA = new Route<Position2f>(routeAList);
            routeBList.Reverse();
            var routeB = new Route<Position2f>(routeBList);

            var offsetSplines = GetOffsetSplines(spline, 4f);

            return new Tuple<Tuple<Route<Position2f>, List<Position2f>>, Tuple<Route<Position2f>, List<Position2f>>>(new Tuple<Route<Position2f>, List<Position2f>>(routeA, offsetSplines.Item1), new Tuple<Route<Position2f>, List<Position2f>>(routeB, offsetSplines.Item2));
        }

        private static Tuple<Position2f, Position2f> GetOffsetPoints(Position2f a, Position2f b, Position2f c, float offset)
        {
            var vec = c - a;
            var vecRight = vec.RotateRight().Normalize() * offset;
            var vecLeft = vec.RotateLeft().Normalize() * offset;
            return new Tuple<Position2f, Position2f>(b + vecRight, b + vecLeft);

        }

        private static Tuple<List<Position2f>, List<Position2f>> GetOffsetSplines(IReadOnlyList<Position2f> positions, float offset)
        {
            var r1 = new List<Position2f>();
            var r2 = new List<Position2f>();
            for (var i = 0; i < positions.Count; ++i)
            {
                var b = positions[i];
                var a = i == 0 ? b : positions[i - 1];
                var c = i == positions.Count - 1 ? b : positions[i + 1];
                var (p1, p2) = GetOffsetPoints(a, b, c, offset);
                r1.Add(p1);
                r2.Add(p2);
            }

            r2.Reverse();
            return new Tuple<List<Position2f>, List<Position2f>>(r1, r2);
        }

        private static List<Position2f> GetSpline(List<Position2f> controlPoints, int subsegments)
        {
            if (controlPoints.Count < 2)
            {
                return null;
            }

            if (controlPoints.Count == 2)
            {
                return controlPoints;
            }

            var pts = new List<Position2f>
            {
                controlPoints.First()
            };

            var dt = 1f / subsegments;
            for (var i = 1; i < subsegments; ++i)
            {
                pts.Add(PointOnCurve(controlPoints[0], controlPoints[0], controlPoints[1], controlPoints[2], dt * i));
            }

            for (var idx = 1; idx < controlPoints.Count - 2; ++idx)
            {
                pts.Add(controlPoints[idx]);
                for (var i = 1; i < subsegments; ++i)
                {
                    pts.Add(PointOnCurve(controlPoints[idx - 1], controlPoints[idx], controlPoints[idx + 1], controlPoints[idx + 2], dt * i));
                }
            }

            pts.Add(controlPoints[controlPoints.Count - 2]);
            for (var i = 1; i < subsegments; ++i)
            {
                pts.Add(PointOnCurve(controlPoints[controlPoints.Count - 3], controlPoints[controlPoints.Count - 2], controlPoints[controlPoints.Count - 1], controlPoints[controlPoints.Count - 1], dt * i));
            }

            pts.Add(controlPoints.Last());

            return pts;
        }

        private static Position2f PointOnCurve(Position2f p0, Position2f p1, Position2f p2, Position2f p3, float t)
        {
            var t2 = t * t;
            var t3 = t2 * t;

            var x = 0.5f * ((2.0f * p1.X) +
            (-p0.X + p2.X) * t +
            (2.0f * p0.X - 5.0f * p1.X + 4 * p2.X - p3.X) * t2 +
            (-p0.X + 3.0f * p1.X - 3.0f * p2.X + p3.X) * t3);

            var y = 0.5f * ((2.0f * p1.Y) +
            (-p0.Y + p2.Y) * t +
            (2.0f * p0.Y - 5.0f * p1.Y + 4 * p2.Y - p3.Y) * t2 +
            (-p0.Y + 3.0f * p1.Y - 3.0f * p2.Y + p3.Y) * t3);

            return new Position2f(x, y);
        }
    }
}
