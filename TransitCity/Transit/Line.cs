using System;
using System.Collections.Generic;
using Geometry;

namespace Transit
{
    public class Line<P> where P : IPosition
    {
        private readonly List<Route<P>> _routes = new List<Route<P>>();

        public Line(string name, params Route<P>[] routes)
        {
            if (routes.Length == 0)
            {
                throw new ArgumentException();
            }

            _routes.AddRange(routes);
            Name = name;
        }

        public IEnumerable<Route<P>> Routes => _routes;

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
