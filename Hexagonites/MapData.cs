using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hexagonites
{
    public struct MapData
    {
        //public List<Polygon> polygons; //we can TRY and save it....and FAILLL
        public List<Hex> hexes;
        public Graph graph;
        public TranslateTransform initTransform; //we can TRY
        public Point mouseInit; //we can TRY try tryyyy
    }
}
