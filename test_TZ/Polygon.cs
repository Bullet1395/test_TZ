using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_TZ
{
    public class Polygon
    {
        public List<Point> points = new List<Point>();

        public Polygon()
        {
        }

        public Polygon(List<Point> points)
        {
            this.points = points;
        }

        public Polygon(Point point)
        {
            this.points.Add(point);
        }
    }
}
