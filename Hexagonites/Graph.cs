using System;
using System.Collections.Generic;
using System.Windows;

namespace Hexagonites
{
    /// <summary>
    /// Class designed to represent the graph. The outermost structure of this graph
    /// is a List<list<Point>>, "a list of lists of Point objects
    /// The Vertices are meant to be saved in a separate indexed data structure
    /// 
    /// Using the index of a particular vertex in this graph class
    /// will give the list of Point objects that relates to the Hex/Polygon that is saved on the SAME
    /// index in their own lists.
    /// 
    /// The Points represent an edge of the graph, where 
    /// X is the index of the destination vertex, and
    /// Y is the movement cost
    /// 
    /// This obtained X-index is then applicable to the 
    /// List<Polygon> for visual modifications,
    /// List<Hex> for any data/logic in the game and the map, as well as
    /// this very graph, to gain the edges and costs for the NEW vertex
    /// </summary>
    public class Graph
    {
        private List<List<Point>> graph; //the list of points to add
        readonly int maxEdges; //represents the maximum number of edges any vertex can have

        public Graph(int maxEdges)
        {
            graph = new List<List<Point>>();
            this.maxEdges = maxEdges;
        }

        //Index-less Add means "add a new entry at the last place, whatever the index may be"
        public void Add()
        {
            graph.Add(new List<Point>());
        }

        internal void generateNeighbors(int index, out int oldSize, out int newSize)
        {
            //This part here is only responsible for creating the neighbors as far as graph-relations go
            //This object will make new Lists of points for traversal
            //it WILL NOT make new Polygon- or Hex objects anywhere.

            //the current size is what will later be the "old" size
            oldSize = graph.Count;

            //Here we actually create neighbors:
            //TODO!!!

            //After the work, the newSize is set
            newSize = graph.Count;
        }
    }
}