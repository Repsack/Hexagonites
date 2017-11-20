using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

/// <summary>
/// Class designed to represent the gamemap made up of imaginary "hexagons"
/// There is no Hexagon class, rather there are 3 lists with matching indexes each handling their own
/// part of what defines each hexagon.
/// This class will contain and handle 3 things:
/// The list of Polygons to be shown on the canvas
/// The list of Hexes to be handled in terms of gamelogic, and some graphics that a Polygon cannot handle
/// The graph that saves information about edges and travel-cost of between hexagons
/// </summary>
namespace Hexagonites
{
    class Map
    {
        private List<Polygon> polygons;
        public List<Hex> hexes;
        private Graph graph;
        TranslateTransform initTransform;
        public Polygon curHighlightPol, curSelectedPol;
        public Hex curHighlightHex,curSelectedHex;
        private Canvas theCanvas;
        private double scale;
        private double strokeThickness;
        private Action<object, MouseEventArgs> highlightHexagon, unhighlightHexagon;
        internal bool polHighlighted;

        public bool PolSelected { get; internal set; }

        public Map(Canvas theCanvas, double scale, 
            Action<object, MouseEventArgs> highlightHexagon, 
            Action<object, MouseEventArgs> unhighlightHexagon)
        {
            this.theCanvas = theCanvas;
            this.scale = scale;
            this.highlightHexagon += highlightHexagon;
            this.unhighlightHexagon += unhighlightHexagon;
            strokeThickness = 2; //arbitrary choice
            polHighlighted = false;
            PolSelected = false;
            polygons = new List<Polygon>();
            hexes = new List<Hex>();
            graph = new Graph(6);
        }

        private void generateNeighbors(int index)
        {
            //This method is responsible for making the List<Polygon>, the List<Hex> and
            //the graph object make their own entries, aswell as passing all the
            //relevant information in between them

            //Each hexagon can have 6 neighbors. Thexe are saved in a list, and are therefore indexed.
            //There is no best way to choose which of them are first on the list, 
            //as long as some convention is honored. The choice will therefore be:
            //The first neighbor is the one directly to the right. Concurrent
            //and index-ordered neighbors are then found going clockwise around the hex
            //In terms of compas directions, here is the directions of the 6 neighbors in order:
            //E, SE, SW, W, NW, NE

            //Unless the this is the first time it is called, the relevant index represents
            //a hexagon that ALREADY have SOME neighbors! This call just generates new empty
            //hexagons and edges between them, aswell as edges to already existing hexes

            //List<Point> oldSet, newSet; //needed to tell which new edges was made and thus hexes
            List<int> updatedSet; //needed to tell which edges was made and thus which hexes must be made
            //The graph need no information from the other lists to correctly create new entries
            graph.generateNeighborEdges(index, out updatedSet);

            //The Hex objects must generate their own points, and so they each need a centerpoint.
            //The centerpoints needed, are made in reference to the centerpoint of the hex at "index".
            //They ALSO depend on which of them already exist aswell as where they are relative to the center.
            //The difference between oldSet and newSet should show which hexes do not exist yet,
            //aswell as their position
            generateHexNeighbors(index, updatedSet);

            //The final task when creating neighbors is to make the Polygon objects,
            //and place them correctly in the polygons list. 
            //Each Polygon needs a set of corners from the corresponding Hex object
            //and possibly also some type information that affects the fill color and more
            //The new Polygons are the same "new" as the new Hex objects, thus
            //it is possible to reuse the updatedSet
            generatePolygonNeighbors(updatedSet);

        }

        private void generatePolygonNeighbors(List<int> updatedSet)
        {
            //Method for creating new Polygon objects in the polygons list.
            //The 2 sets show which new Polygon objects must be made, 
            //and at what index their corresponding Hex objects are.
            //Each polygon must ask the correct hex for the cornerPoints they need to be fully rendered

            Polygon p;
            Console.WriteLine("updatedSet size = " + updatedSet.Count);
            foreach(int i in updatedSet)
            {
                p = new Polygon();
                p.Name = "s"+i; //using "s#" instead of "#" because just a number is forbidden 
                p.Fill = Brushes.Red; //subject to change, depending on hexagon type
                p.Stroke = Brushes.Black; //subject to change, depending on hexagon type
                p.StrokeThickness = strokeThickness; //arbitrary, but should probably be the same for ALL hexes
                p.RenderTransform = initTransform;
                p.HorizontalAlignment = 0; //means Left as defined in the enum of HorizontalAlignment
                p.VerticalAlignment = 0; //means Top as defined in the enum of VerticalAlignment
                p.Points = hexes[i+1].corners; //The polygon can now receive the corner points generated in the hex object
                //even if it works... WHYYY IS IT i+1 ??????????????????????
                p.MouseEnter += new MouseEventHandler(highlightHexagon);
                p.MouseLeave += new MouseEventHandler(unhighlightHexagon);
                theCanvas.Children.Add(p); //give the polygon to the canvas for rendering

                //These next entries all happen at the same index: 0
                polygons.Add(p); //we must keep the same reference here, in case we later must change the visuals

            }
            int n = 0;
            foreach(Hex h in hexes)
            {
                /*
                Console.WriteLine("Points of POL no." + n + " :");
                Console.WriteLine(po.Points.ToString());
                Console.WriteLine("");
                */
                Console.WriteLine("Hex no."+n+" state: " + h.empty);
                n++;
            }
        }

        private void generateHexNeighbors(int index, List<int> updatedIndexes)
        {
            //Method for creating new Hex objects to the hexes list.
            //index is used to point to the hex in the center of the new neighbors to-be
            //the two sets are used to derive which neighbors must be made and where

            //List of indexes for those hexagons that has changed their edges

            Point center = hexes[index].center;
            /*
            List<int> updatedIndexes = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                if(oldSet[i].X != newSet[i].X && oldSet[i].X == -1) //means a new hex should be made
                {
                    updatedIndexes.Add(i); //add index of the soon new hex
                }
            }
            */
            //Now we know exactly which new hex objects must be made next to the one at "index"
            //We also know their positions relative to the center
            //new Hex(center,scale,name);
            for (int i = 0; i < updatedIndexes.Count; i++)
            {
                hexes.Add(new Hex(calcHexCenter(hexes[index].center,updatedIndexes[i]),scale,i.ToString()));
            }
        }

        private Point calcHexCenter(Point center, int dir)
        {
            //Method for calculating the center of the next hex, 
            //depending on the current center and the direction defined by dir.
            //recall direction order: E, SE, SW, W, NW, NE
            double height = 2.0 * scale;
            double width = Math.Sqrt(3) / 2.0 * height;
            switch (dir)
            {
                case 0: //Means E
                    return new Point(center.X+width,center.Y); //USE that hex page to get the proper calculations for any new center
                case 1: //Means SE
                    return new Point(center.X + width / 2.0, center.Y + (height * 3) / 4.0);
                case 2: //Means SW
                    return new Point(center.X - width / 2.0, center.Y + (height * 3) / 4.0);
                case 3: //Means W
                    return new Point(center.X-width, center.Y);
                case 4: //Means NW
                    return new Point(center.X - width / 2.0, center.Y - (height * 3) / 4.0);
                case 5: //Means NE
                    return new Point(center.X + width / 2.0, center.Y - (height * 3) / 4.0);
                default:
                    return new Point(-100,-100);
            }
        }

        public void placeFirst(object sender, MouseEventArgs mouse)
        {
            Polygon p = new Polygon();
            
            //using "0" as name because "placeFirst" really is supposed to be the first index in the list
            Hex h = new Hex(new Point(0, 0), scale, "0"); //needs a point for the makeCorners method
            h.empty = false; //Means the hex is an actual hex and not just a potential hex
            p.Name = "s0"; //using "s0" instead of "0" because just "0" is forbidden 
            p.Fill = Brushes.Red; //subject to change, depending on hexagon type
            p.Stroke = Brushes.Black; //subject to change, depending on hexagon type
            p.StrokeThickness = strokeThickness; //arbitrary, but should probably be the same for ALL hexes
            initTransform = new TranslateTransform(mouse.GetPosition(theCanvas).X, mouse.GetPosition(theCanvas).Y);
            p.RenderTransform = initTransform;
            p.HorizontalAlignment = 0; //means Left as defined in the enum of HorizontalAlignment
            p.VerticalAlignment = 0; //means Top as defined in the enum of VerticalAlignment
            p.Points = h.corners; //The polygon can now receive the corner points generated in the hex object
            p.MouseEnter += new MouseEventHandler(highlightHexagon);
            p.MouseLeave += new MouseEventHandler(unhighlightHexagon);
            theCanvas.Children.Add(p); //give the polygon to the canvas for rendering

            //These next entries all happen at the same index: 0
            polygons.Add(p); //we must keep the same reference here, in case we later must change the visuals
            hexes.Add(h); //the Hex data is added to the list
            graph.AddLoose(); //This adds the first entry into the graph
            generateNeighbors(0); //in a separate operation, the neighbors for the first entry are made
        }
    }
}
