using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hexagonites
{
    //Class representing all the data that goes into the graph
    //Before this, the graph only contained information about neighbors for any given entry
    //-this lead to the problem where the mapmaker could generate multiple entries where
    //the matching Hex objects all had the same center value. And these would then
    //overlap completely when drawn.
    //
    //The solution is then to create a coordinate system so that the next graph entry should
    //only be created if no other graph entry matches the coordinates
    [Serializable]
    public class Vertex
    {
        public List<Point> neighbors; //Like before, these represent the neighbor direction and the travelcost
        public List<int> coords; //3 ints represent *that* cube coordinate system

        public Vertex()
        {
            
        }
        public Vertex(bool notNew)
        {
            neighbors = new List<Point>();
            coords = new List<int>();
            for (int i = 1; i <= 6; i++) //6 step loop, for 6 neighbor spots
            {
                neighbors.Add(new Point(-1, -1)); //(-1,-1) means that all neighbors are unknown yet
                if (i <= 3) //only 3 coords
                {
                    coords.Add(-30000); //default error value
                }
            }
        }
    }
}
