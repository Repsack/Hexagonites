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
/// The list of Hexes to be handled in terms of gamelogic
/// The graph that saves information about edges and travel-cost of between hexagons
/// </summary>
namespace Hexagonites
{
    class Map
    {
        private List<Polygon> polygons;
        private List<Hex> hexes;
        private Graph graph;
        private Canvas theCanvas;
        private double scale;
        private double strokeThickness;

        public Map(Canvas theCanvas, double scale)
        {
            this.theCanvas = theCanvas;
            this.scale = scale;
            strokeThickness = 2; //arbitrary choice
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

            int oldSize, newSize;
            //The graph need no information from the other lists to correctly create new entries
            graph.generateNeighbors(index, out oldSize, out newSize);

        }

        public void placeFirst(object sender, MouseEventArgs mouse)
        {
            Polygon p = new Polygon();

            //using "0" as name because "placeFirst" really is supposed to be the first index in the list
            Hex h = new Hex(new Point(0, 0), scale, "0"); //needs a point for the makeCorners method
            h.empty = false; //Means the hex is an actual hex and not just a potential hex
            p.Name = "s0"; //using "s0" instead of "0" because just "0" is forbidden 
            p.Fill = Brushes.White; //subject to change, depending on hexagon type
            p.Stroke = Brushes.Black; //subject to change, depending on hexagon type
            p.StrokeThickness = strokeThickness; //arbitrary, but should probably be the same for ALL hexes
            p.RenderTransform = new TranslateTransform(mouse.GetPosition(theCanvas).X, mouse.GetPosition(theCanvas).Y);
            p.HorizontalAlignment = 0; //means Left as defined in the enum of HorizontalAlignment
            p.VerticalAlignment = 0; //means Top as defined in the enum of VerticalAlignment
            p.Points = h.corners; //The polygon can now receive the corner points generated in the hex object
            theCanvas.Children.Add(p); //give the polygon to the canvas for rendering

            //These next entries all happen at the same index: 0
            polygons.Add(p); //we must keep the same reference here, in case we later must change the visuals
            hexes.Add(h); //the Hex data is added to the list
            graph.Add(); //This adds the first entry into the graph
            generateNeighbors(0); //in a separate operation, the neighbors for the first entry are made

            //Here is where the rest of the Hex+Polygons must be created and made, after the neighbors are created
        }
    }
}
