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
    class MapData
    {
        public List<Polygon> polygons; //we can TRY and save it....
        public List<Hex> hexes;
        public Graph graph;
        TranslateTransform initTransform; //we can TRY
        Point mouseInit; //we can TRY try tryyyy
    }
}
