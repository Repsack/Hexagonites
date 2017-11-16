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

        //AddInitial means "add the very first entry in the (graph)list"
        //THIS SHOULD PROBABLY NOT BE USED
        public void AddInitial()
        {
            graph.Add(new List<Point>()); //First, a new entry into the (graph)list of neighbor-lists

            //This loop creates 6 new entries at the last position of the (graph)list
            //Each entry will be a point with the coordinates i,-1 representing the next 6 elements,
            //namely index 1 through 6 (total of 7 hexes)
            for (int i = 1; i <= 6; i++)
            {
                //we can say [0] because the initial point is first in this list
                graph[0].Add(new Point(i, -1)); //the count from 1 to 6 also represent the 6 new entries

                //since this is INITIAL, we can safely add the 6 new entries, 
                //since there were NO graph parts before this
                graph.Add(new List<Point>()); 
            }
        }

        //This method creates a new entry in the (graph)list and adds 6 invalid points to it
        public void AddLoose()
        {
            graph.Add(new List<Point>()); //the new entry
            for (int i = 1; i <= 6; i++)
            {
                graph[graph.Count - 1].Add(new Point(-1, -1)); //(-1,-1) means that all neighbors are unknown yet
            }
        }

        internal void generateNeighborEdges(int index, out List<Point> oldSet, out List<Point> newSet)
        {
            int newIndex;
            double oldY;
            //This part here is only responsible for creating the neighbors as far as graph-relations go
            //This object will make new Lists of points for traversal
            //it WILL NOT make new Polygon- or Hex objects anywhere.

            //There will likely already be SOME neighbors written down for the entry at "index",


            //the current list of Points at "index" is what will later be the "oldSet"
            oldSet = new List<Point>();
            foreach(Point p in graph[index])
            {
                oldSet.Add(new Point(p.X,p.Y)); //Just a copy
            }

            //Here we actually create neighbors:
            //The first loop will iterate over all 6 potential neighbors and check for an already existing neighbor
            //and if there is no neighbor (the point.X is -1), a new one is created
            //Here the loop will also establish relationship to/from the graph[index] and the new neighbors
            for (int i = 0; i < 6; i++)
            {

                if(graph[index][i].X==-1)
                {
                    AddLoose();
                    newIndex = graph.Count - 1;
                    oldY = graph[index][i].Y;
                    graph[index][i] = new Point(newIndex, oldY);
                    oldY = graph[newIndex][oposDir(index)].Y;
                    graph[newIndex][oposDir(i)] = new Point(index, oldY);
                }
                else
                {
                    //"else" there IS a neighbor, and we should not try and modify it. NO action!
                }
            }

            //Second for-loop is responsible for making all neighbors (THAT NOW EXIST),
            //of graph[index] establish correct neighbor-relations to each other
            //since we need 2 neighbors to establish a connection, a "previous" neighbor will be used
            //This "previous" will start as being the last of them:
            Double previousI = 5;
            for (int i = 0; i < 6; i++)
            {
                oldY = graph[(int)(graph[index][i].X)][leftDir(i)].Y; //save the old Y value of the index-neighbor's left Point
                graph[(int)graph[index][i].X][leftDir(i)] = new Point(graph[index][(int)previousI].X, oldY); //assign the LEFT relation
                oldY = graph[(int)graph[index][(int)previousI].X][rightDir(previousI)].Y; //Save the old Y value of the previous-neighbor's right Point
                graph[(int)graph[index][(int)previousI].X][rightDir(previousI)] = new Point(graph[index][i].X,oldY); //assign the RIGHT relation
                previousI = i; //set previousI as i (current) just before i is incremented by i++;
            }

            //After the work, the newSize is set
            newSet = new List<Point>();
            foreach (Point p in graph[index])
            {
                newSet.Add(new Point(p.X, p.Y)); //Just a copy
            }
        }

        private int rightDir(double previousI)
        {
            switch (previousI)
            {
                case 0:
                    return 2;
                case 1:
                    return 3;
                case 2:
                    return 4;
                case 3:
                    return 5;
                case 4:
                    return 0;
                case 5:
                    return 1;
                default:
                    return -1;
            }
        }

        private int leftDir(int v)
        {
            switch (v)
            {
                case 0:
                    return 4;
                case 1:
                    return 5;
                case 2:
                    return 0;
                case 3:
                    return 1;
                case 4:
                    return 2;
                case 5:
                    return 3;
                default:
                    return -1;
            }
        }

        private int oposDir(int i)
        {
            switch (i)
            {
                case 0:
                    return 3;
                case 1:
                    return 4;
                case 2:
                    return 5;
                case 3:
                    return 0;
                case 4:
                    return 1;
                case 5:
                    return 2;
                default:
                    return -1;
                
            }
        }
    }
}