using System;
using System.Collections.Generic;
using System.Windows;

namespace Hexagonites
{
    /// <summary>
    /// Class designed to represent the graph. The outermost structure of this graph
    /// is a List<list<Point>>, "a list of lists of Point objects"
    /// The Vertices are meant to be saved in a separate indexed data structure
    /// 
    /// Using the index of a particular vertex in this graph class
    /// will give the list of Point objects that relates to the Hex/Polygon that is saved on the SAME
    /// index in their own lists.
    /// 
    /// The list<Point> "list of points" will always contain 6 points where the index of that list
    /// represents the specific point matching a specific direction:
    /// index numbers 0 to 5 represents the directions E, SE, SW, W, NW, NE
    /// 
    /// The Points represent an edge of the graph, where 
    /// X is the index of the destination vertex in terms of this (graph)list
    /// Y is the movement cost going from "index" to the index given from "X" of this point
    /// 
    /// This obtained X-index is then applicable to the 
    /// List<Polygon> for visual modifications,
    /// List<Hex> for any data/logic in the game and the map, as well as
    /// this very graph, to gain the edges and costs for the NEW vertex
    /// 
    /// EXAMPLE:
    /// say you read this.list[11][4].X and you get the integer 7
    /// this means that the "11th" entry of the graph has a neighbor in the direction 4(NW)
    /// that very X being positive means that the neighbor exists.
    /// X == 7 means that the neighbor can be found by looking it up using index 7
    /// 
    /// reading this.list[7][2].X would then give you the integer 11 since
    /// list[7][2] means the "7th" entry in direction 2[SE], and the X at that value
    /// must show 7, since going NW from 11 and getting to 7, means you can
    /// go SE from 7 and get to 11
    /// </summary>
    [Serializable]
    public class Graph
    {
        public List<List<Point>> graph; //the list of points to add
        public List<Vertex> graph2;
        public int Count //shows how many entries there are in the (graph)list
        {
            get
            {
                return graph2.Count;
            }
        }

        public Graph()
        {
            graph = new List<List<Point>>();
            graph2 = new List<Vertex>();
        }

        //This method creates a new entry in the (graph)list and adds 6 invalid points to it
        public void AddLoose()
        {
            graph.Add(new List<Point>()); //the new entry
            graph2.Add(new Vertex(true));
            for (int i = 1; i <= 6; i++) //6 step loop, for 6 neighbor spots
            {
                graph[graph.Count - 1].Add(new Point(-1, -1)); //(-1,-1) means that all neighbors are unknown yet
            }
        }

        //Method for generating new neighbors for a particular hex
        internal void generateNeighborEdges(int index, out List<int> updatedDirs)
        {
            List<Point> oldSet, newSet; 
            oldSet = new List<Point>(); //Represent neighbors that already were made
            newSet = new List<Point>(); //represent neighbors that was made + all the new neighbors
            
            //will represent the difference between oldSet and newSet, but now
            //without the Y part of the point, since that only represents travelcost
            updatedDirs = new List<int>(); 

            int newIndex; //used to represent the index of potentially new entries in the graph
            double oldY; //will later be used to save travelcost info and transfer it to new Point objects
            
            //This method here is only responsible for creating the neighbors as far as graph-relations go
            //This object (Graph) will make new Lists of points for traversal
            //it WILL NOT make new Polygon- or Hex objects anywhere.

            //There will likely already be SOME neighbors defined for the entry at "index"

            //The current list of Points at "index" is what will later be the "oldSet"
            foreach(Point p in graph[index])
            {
                oldSet.Add(new Point(p.X,p.Y)); //Just a copy of what we have now, saved for later reference
            }
            foreach(Point p in graph2[index].neighbors)
            {
                oldSet.Add(new Point(p.X, p.Y)); //Just a copy of what we have now, saved for later reference
            }

            //Here we actually create neighbors:
            //The first loop will iterate over all 6 potential neighbors and check for an already existing neighbor
            //and if there is no neighbor (point.X is -1), a new one is created
            //Here the loop will also establish relationship to/from the graph[index] and the new neighbors
            //[index] represents a given entry in the (graph)list
            //[i] represents 1 of the 6 neighbor directions: E, SE, SW, W, NW, NE as numbers 0 to 5
            for (int i = 0; i < 6; i++)
            {
                if(graph2[index].neighbors[i].X==-1) //The hexagon at [index] has no neighbor in direction [i]
                {
                    AddLoose(); // creates an actual new element in the (graph)list
                    newIndex = graph2.Count - 1; //the index of this new element
                    oldY = graph[index][i].Y; //travelcost for [index][i] is saved here
                    oldY = graph2[index].neighbors[i].Y; //travelcost for [index].neighbors[i] is saved here

                    //reusing oldY, the direction [i] is then set with a new X value, namely the newIndex gotten
                    //after calling AddLoose()
                    graph[index][i] = new Point(newIndex, oldY);
                    graph2[index].neighbors[i] = new Point(newIndex, oldY);

                    //now we do the same thing, but for the new entry:

                    oldY = graph[newIndex][oposDir(i)].Y; //save the travelcost
                    oldY = graph2[newIndex].neighbors[oposDir(i)].Y; //save the travelcost

                    //reusing oldY again, the entry at newIndex should also have a neighbor BACK
                    //to the one gotten from [index]..
                    //oposDir gives the opposite direction from i. This should be "backwards" to [index]
                    graph[newIndex][oposDir(i)] = new Point(index, oldY);
                    graph2[newIndex].neighbors[oposDir(i)] = new Point(index, oldY);
                }
                else //[index][i].X is NOT -1
                {
                    //"else" there IS a neighbor, and we should not try and modify it. NO action!
                }
            }

            //Second for-loop is responsible for making all neighbors (THAT NOW EXIST),
            //of graph[index] establish correct neighbor-relations to each other
            //since we need 2 neighbors to establish a connection, a "previous" neighbor will be used
            //around the hex at [index], there will be 6 PAIRS of neighbors, that also neighbor each other!
            //the [dir] value is used to represent the "current" neighbor.
            //This "previousDir" will start as being the last of the 6 neighbors a hex has:
            int previousDir = 5; //5 is last because 6 neighbors have the indexes 0,1,2,3,4,5
            for (int dir = 0; dir < 6; dir++)
            {
                

                //to assign the LEFT relation, going from i to previousI, we must do some pretty obnoxious stuff
                //here is the breakdown:
                //all the pairs are AROUND the entry at [index], and are therefore NOT = index
                //the entry curNeighborI that gets a new value is found FROM the [index] by graph[index][dir].X
                int curNeighborI = (int)graph[index][dir].X; //this is the index of the "current" neighbor in direction [dir]
                curNeighborI = (int)graph2[index].neighbors[dir].X; //this is the index of the "current" neighbor in direction [dir]

                //We also need the index of the "previous" neighbor of [index] in direction [dir], except
                //the direction of the "previous" neighbor is given from previousDir:
                int prevNeighborI = (int)graph[index][previousDir].X; //this is the index of the "previous" neighbor in direction [previousDir]
                prevNeighborI = (int)graph2[index].neighbors[previousDir].X; //this is the index of the "previous" neighbor in direction [previousDir]

                //need to save the oldY value so it can be reapplied later
                oldY = graph[curNeighborI][leftDir(dir)].Y; //save the old Y value of the current-neighbor's left Point
                oldY = graph2[curNeighborI].neighbors[leftDir(dir)].Y; //save the old Y value of the current-neighbor's left Point

                //reusing oldY, we are ready to create the left-relation, using both the 2 indexes of the 2 neighbors to [index]
                //leftDir(dir) delivers the correct direction for the left-relation we are making right now
                graph[curNeighborI][leftDir(dir)] = new Point(prevNeighborI, oldY); //left-relation now established!
                graph2[curNeighborI].neighbors[leftDir(dir)] = new Point(prevNeighborI, oldY); //left-relation now established!

                //we need to save oldY once more, before making the right-relation
                oldY = graph[prevNeighborI][rightDir(previousDir)].Y; //Save the old Y value of the previous-neighbor's right Point
                oldY = graph2[prevNeighborI].neighbors[rightDir(previousDir)].Y; //Save the old Y value of the previous-neighbor's right Point

                //reusing oldY we are ready to create the right-relation, using both the 2 indexes of the 2 neighbors to [index]
                //rightDir(previousDir) delivers the correct direction for the right-relation we are making right now
                graph[prevNeighborI][rightDir(previousDir)] = new Point(curNeighborI,oldY); //right-relation now established!
                graph2[prevNeighborI].neighbors[rightDir(previousDir)] = new Point(curNeighborI, oldY); //right-relation now established!

                previousDir = dir; //set previousDir as dir (current) just before dir is incremented by dir++;
            }

            //After the work, the newSize is set
            foreach (Point p in graph[index])
            {
                newSet.Add(new Point(p.X, p.Y)); //Just a copy
            }
            foreach (Point p in graph2[index].neighbors)
            {
                newSet.Add(new Point(p.X, p.Y)); //Just a copy
            }

            //From the oldSet and the newSet, create a set of differences
            for (int i = 0; i < 6; i++)
            {
                if (oldSet[i].X != newSet[i].X) //means a new hex should be made
                { 
                    updatedDirs.Add(i); //add index of the soon new hex
                }
            }

            //ALL NEW RELATIONS from [index] are now established!
            //ALL NEW RELATIONS between ALL neighbors of [index] are now established!
            //updatedIndexes contains all the directions from [index] that has changed
        }

        //method for giving the diretion of the neighbor 1 step clockwise
        //  integer list: 0,  1,  2,  3,  4,  5
        //direction list: E, SE, SW,  W, NW, NE
        //EXAMPLE: 
        //asking for direction E, will give the direction from neighbor E, 
        //to clockwise neighbor SE. This will then be SW
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

        //method for giving the diretion of the neighbor 1 step counterclockwise
        //  integer list: 0,  1,  2,  3,  4,  5
        //direction list: E, SE, SW,  W, NW, NE
        //EXAMPLE: 
        //asking for direction E, will give the direction from neighbor E, 
        //to counterclockwise neighbor NE. This will then be NW
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

        //This method takes a direction, and gives the opposite direction
        //  integer list: 0,  1,  2,  3,  4,  5
        //direction list: E, SE, SW,  W, NW, NE
        private int oposDir(int i)
        {
            switch (i) //6 cases for 6 directions
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