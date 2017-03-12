using System;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathFinding.Network;
using Transit;

namespace TransitUnitTest
{
    [TestClass]
    public class LineUnitTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Func<Node<Position2f>, Node<Position2f>, TimeEdgeCost> costFuncWalking = (a, b) => new TimeEdgeCost(a.Position.DistanceTo(b.Position) / 2.2f); // 8km/h

            var station1A = new Station<Position2f>(new Position2f(1500, 1000));
            var station2A = new Station<Position2f>(new Position2f(2500, 1000));
            var station3A = new Station<Position2f>(new Position2f(3500, 1500));
            var station4A = new Station<Position2f>(new Position2f(4000, 2500));
            var station5A = new Station<Position2f>(new Position2f(4500, 3500));
            var station6A = new Station<Position2f>(new Position2f(5500, 4000));
            var station7A = new Station<Position2f>(new Position2f(6500, 4000));
            var station8A = new Station<Position2f>(new Position2f(7500, 4000));
            var station9A = new Station<Position2f>(new Position2f(8000, 5000));
            var station10A = new Station<Position2f>(new Position2f(8000, 6000));
            var station11A = new Station<Position2f>(new Position2f(7500, 7000));
            var station12A = new Station<Position2f>(new Position2f(7500, 8000));
            var station13A = new Station<Position2f>(new Position2f(8000, 9000));

            var station1B = new Station<Position2f>(new Position2f(1500, 1020));
            var station2B = new Station<Position2f>(new Position2f(2500, 1020));
            var station3B = new Station<Position2f>(new Position2f(3500, 1520));
            var station4B = new Station<Position2f>(new Position2f(4000, 2520));
            var station5B = new Station<Position2f>(new Position2f(4500, 3520));
            var station6B = new Station<Position2f>(new Position2f(5500, 4020));
            var station7B = new Station<Position2f>(new Position2f(6500, 4020));
            var station8B = new Station<Position2f>(new Position2f(7500, 4020));
            var station9B = new Station<Position2f>(new Position2f(8000, 5020));
            var station10B = new Station<Position2f>(new Position2f(8000, 6020));
            var station11B = new Station<Position2f>(new Position2f(7500, 7020));
            var station12B = new Station<Position2f>(new Position2f(7500, 8020));
            var station13B = new Station<Position2f>(new Position2f(8000, 9020));

            var route1A = new Route<Position2f>(new[]{ station1A, station2A, station3A, station4A, station5A, station6A, station7A, station8A, station9A, station10A, station11A, station12A, station13A }, 300f);
            var route1B = new Route<Position2f>(new[]{ station1B, station2B, station3B, station4B, station5B, station6B, station7B, station8B, station9B, station10B, station11B, station12B, station13B }, 300f);
            var line1 = new Line<Position2f>("1", route1A, route1B);

            var network = new TransitNetwork<Position2f>();
            network.ConnectLine(line1, SubwayTravelTimeFunc);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_1", station1A, station1B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_2", station2A, station2B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_3", station3A, station3B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_4", station4A, station4B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_5", station5A, station5B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_6", station6A, station6B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_7", station7A, station7B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_8", station8A, station8B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_9", station9A, station9B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_10", station10A, station10B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_11", station11A, station11B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_12", station12A, station12B), costFuncWalking);
            network.ConnectTransferStation(new TransferStation<Position2f>("1_13", station13A, station13B), costFuncWalking);
        }

        private TimeEdgeCost SubwayTravelTimeFunc(Node<Position2f> a, Node<Position2f> b)
        {
            const float meanAcceleration = 0.6f;
            const float maximalSpeed = 70f / 3.6f; // 70 km/h
            const float timeToReachMaximalSpeed = maximalSpeed / meanAcceleration;
            const float neededDistanceToReachMaximalSpeed = meanAcceleration / 2 * timeToReachMaximalSpeed * timeToReachMaximalSpeed;
            var distance = a.Position.DistanceTo(b.Position);
            var baseTime = 30f; // waiting time at station
            if (distance < 2 * neededDistanceToReachMaximalSpeed) // distance is too small to reach maximalSpeed
            {
                baseTime += 2 * (float)Math.Sqrt(distance / meanAcceleration);
            }
            else
            {
                var remainingDistance = distance - 2 * neededDistanceToReachMaximalSpeed;
                baseTime += 2 * timeToReachMaximalSpeed + remainingDistance / maximalSpeed;
            }

            return new TimeEdgeCost(baseTime);
        }
    }
}
