using System;
using System.Collections.Generic;

namespace Transit
{
    public class Line
    {
        private readonly List<Route> _routes = new List<Route>();

        public Line(string name, params Route[] routes)
        {
            if (routes.Length == 0)
            {
                throw new ArgumentException();
            }

            _routes.AddRange(routes);
            Name = name;
        }

        public IEnumerable<Route> Routes => _routes;

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
