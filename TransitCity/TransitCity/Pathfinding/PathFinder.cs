namespace TransitCity.Pathfinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Utility.Units;

    internal static class PathFinder
    {
        internal static Tuple<Path, Time> FindQuickestPath(Node from, Node to, Traveller traveller)
        {
            if (!traveller.GetType().IsValueType)
            {
                throw new ArgumentException("traveller needs to be a copyable struct");
            }

            var allowedNodes = traveller.AllTypes;
            if (!from.Info.AllowedTypes.Intersect(allowedNodes).Any() || !to.Info.AllowedTypes.Intersect(allowedNodes).Any())
            {
                Console.WriteLine(@"Warning: this kind of traveller can't use the start or end node.");
                return null;
            }

            var closedSet = new HashSet<Node>();
            var openSet = new List<Tuple<Node, Traveller>> { Tuple.Create(from, traveller) };
            var cameFrom = new Dictionary<Node, Tuple<Node, PathInfo>>();
            var gScore = new Dictionary<Node, Time> { { from, Time.Zero } };
            var fScore = new Dictionary<Node, Time> { { from, HeuristicTimeEstimate(from, to) } };

            while (openSet.Count != 0)
            {
                var currentNode = fScore.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;

                if (currentNode == to)
                {
                    return Tuple.Create(ReconstructPath(cameFrom, to), gScore[currentNode]);
                }

                var currentGScore = gScore[currentNode];
                var currentTraveller = openSet.Find(x => x.Item1 == currentNode).Item2;
                openSet.RemoveAll(x => x.Item1 == currentNode);
                gScore.Remove(currentNode);
                fScore.Remove(currentNode);
                closedSet.Add(currentNode);
                foreach (var nextNodePair in currentNode.NextNodes)
                {
                    var nextNode = nextNodePair.Key;
                    if (!nextNode.Info.Public && !currentTraveller.Keys.Contains(nextNode))
                    {
                        continue;
                    }

                    if (!nextNode.Info.AllowedTypes.Intersect(currentTraveller.AllTypes).Any() && !nextNode.Info.AllowedTypes.Contains(currentTraveller.CurrentType))
                    {
                        continue;
                    }

                    if (closedSet.Contains(nextNode))
                    {
                        continue;
                    }

                    foreach (var pathInfo in nextNodePair.Value)
                    {
                        var type = pathInfo.Type;
                        if (!currentTraveller.AllTypes.Contains(type) && currentTraveller.CurrentType != type)
                        {
                            continue;
                        }

                        var nextTraveller = Traveller.Copy(currentTraveller);
                        if (type != nextTraveller.CurrentType)
                        {
                            nextTraveller.CurrentType = type;
                            if (!nextTraveller.ReusableTypes.Contains(type))
                            {
                                nextTraveller.NonReusableTypes.Remove(type);
                            }
                        }

                        var tentativeGScore = currentGScore + (currentNode.GetDistanceTo(nextNode) / pathInfo.Speed);
                        if (currentNode.Info.TimePenalties.ContainsKey(nextTraveller.CurrentType))
                        {
                            tentativeGScore += currentNode.Info.TimePenalties[nextTraveller.CurrentType];
                        }

                        if (openSet.All(x => x.Item1 != nextNode))
                        {
                            openSet.Add(Tuple.Create(nextNode, nextTraveller));
                        }
                        else if (tentativeGScore >= gScore[nextNode])
                        {
                            continue;
                        }

                        cameFrom[nextNode] = Tuple.Create(currentNode, pathInfo);
                        gScore[nextNode] = tentativeGScore;
                        fScore[nextNode] = gScore[nextNode] + HeuristicTimeEstimate(nextNode, to);
                    }
                }
            }

            return null;
        }

        private static Time HeuristicTimeEstimate(Node from, Node to)
        {
            var dist = from.GetDistanceTo(to);
            var speed = new Speed { Kmh = 30f };
            return dist / speed;
        }

        private static Path ReconstructPath(IReadOnlyDictionary<Node, Tuple<Node, PathInfo>> cameFrom, Node current)
        {
            var nodes = new List<Node> { current };
            var infos = new List<PathInfo>();
            var tmp = current;
            while (cameFrom.ContainsKey(tmp))
            {
                var tuple = cameFrom[tmp];
                tmp = tuple.Item1;
                nodes.Add(tuple.Item1);
                infos.Add(tuple.Item2);
            }

            nodes.Reverse();
            infos.Reverse();
            return new Path(nodes, infos);
        }
    }
}
