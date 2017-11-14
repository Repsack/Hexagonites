using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Hexagonites
{
    class Hex
    {
        //Can a delegate here grab the event that fires from a Polygon that has the same name..?

        public string name { get; set; }
        public double PI = System.Math.PI;
        public Point center;
        public PointCollection corners;
        private double sscale;
        public bool highlighted { get; set; }

        public double scale
        {
            get { return sscale; }
            set
            {
                sscale = value;
                makeCorners();
            }
        }
        public Hex(Point center, double scale)
        {
            this.center = center;
            this.scale = scale;
            makeCorners();
        }
        private void makeCorners()
        {
            corners = new PointCollection();
            for (int i = 0; i < 6; i++)
            {
                
                double deg = 60 * i + 30;
                double rad = PI / 180 * deg;
                Point ptoadd = new Point(center.X + scale * System.Math.Cos(rad),
                                      center.Y + scale * System.Math.Sin(rad));

                corners.Add(ptoadd);
            }
        }
    }
}
