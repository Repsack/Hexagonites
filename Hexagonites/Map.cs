using System;
using System.Collections.Generic;
using System.IO;
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
        public List<Polygon> polygons;
        public List<Hex> hexes;
        private Graph graph;
        TranslateTransform initTransform;
        Point mouseInit;
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
            //marker = new Marker(theCanvas, scale, highlightHexagon, unhighlightHexagon);
            strokeThickness = 2; //arbitrary choice
            polHighlighted = false;
            PolSelected = false;
            polygons = new List<Polygon>();
            hexes = new List<Hex>();
            graph = new Graph();
            mouseInit = new Point();
        }

        public Map(Canvas theCanvas, double scale, 
            Action<object, MouseEventArgs> highlightHexagon, 
            Action<object, MouseEventArgs> unhighlightHexagon, 
            string dataFromFile) : this(theCanvas, scale, highlightHexagon, unhighlightHexagon)
        {
            fromString(dataFromFile);
            
        }

        

        public void generateNeighbors(int index)
        {
            Console.WriteLine("genNeigh called with index=" + index);
            //This method is responsible for making the List<Polygon>, the List<Hex> and
            //the graph object make their own entries, aswell as passing all the
            //relevant information in between them

            //Each hexagon can have 6 neighbors. These are saved in a list, and are therefore indexed.
            //There is no best way to choose which of them are first on the list, 
            //as long as some convention is honored. The choice will therefore be:
            //The first neighbor is the one directly to the right. Concurrent
            //and index-ordered neighbors are then found going clockwise around the hex.
            //In terms of compas directions, here is the directions of the 6 neighbors in order:
            //E, SE, SW, W, NW, NE

            //Unless this is the first time it is called, the current index represents
            //a hexagon that ALREADY have SOME neighbors! This call just generates new empty
            //hexagons and edges between them, aswell as edges to already existing hexes

            List<int> updatedDirs; //needed to tell which edges was made and thus which hexes must be made
            //The graph need no information from the other lists to correctly create new entries
            graph.generateNeighborEdges(index, out updatedDirs);

            //The Hex objects must generate their own points, and so they each need a centerpoint.
            //The centerpoints needed, are made in reference to the centerpoint of the hex at "index".
            //They ALSO depend on which of them already exist aswell as where they are relative to the center.
            //The updatedSet should show which hexes do not exist yet,
            //aswell as their position relative to the [index] hexagon
            generateHexNeighbors(index, updatedDirs);

            //The final task when creating neighbors is to make the Polygon objects,
            //and place them correctly in the polygons list. 
            //Each Polygon needs a set of corners from the corresponding Hex object
            //and possibly also some type information that affects the fill color and more
            //The new Polygons are the same "new" as the new Hex objects, thus
            //it is possible to reuse the updatedSet
            generatePolygonNeighbors(index, updatedDirs);

        }

        private void generatePolygonNeighbors(int index, List<int> updatedDirs)
        {
            //Method for creating new Polygon objects in the polygons list.
            //updatedDirs show how many new Polygon objects must be made, 
            //and which neighborly-direction they have from [index]
            //index here is ofcourse the existing hex, from which the neighbors must be obtained
            //Each polygon must ask the correct hex for the cornerPoints they need to be fully rendered

            Polygon p; //the polygon object to be added
            string newName="-111"; //the polygons need a name, aswell as the hex objects do
            Console.WriteLine("updatedSet size = " + updatedDirs.Count);
            Console.WriteLine("index = " + index);
            Console.WriteLine("graph entry count: " + graph.Count);
            for(int i = 0; i < updatedDirs.Count; i++) //these are amount of new polygons needed
            {
                p = new Polygon();
                
                //Console.WriteLine("i is " + i);
                double theX = graph.graph2[index].neighbors[updatedDirs[i]].X;
                //Console.WriteLine("theX is " + theX);
                newName = ((int)(graph.graph2[index].neighbors[updatedDirs[i]].X)).ToString();
                
                p.Name = "s"+newName; //using "s#" instead of "#" because just a number is forbidden 
                p.Fill = (Brush)new BrushConverter().ConvertFromString("#777"); //subject to change, depending on hexagon type
                p.Stroke = (Brush)new BrushConverter().ConvertFromString("#333"); //subject to change, depending on hexagon type
                p.StrokeThickness = strokeThickness; //arbitrary, but should probably be the same for ALL hexes
                p.RenderTransform = initTransform; //just like the first polygon was placed with a transformation, so must all the rest.
                p.HorizontalAlignment = 0; //means Left as defined in the enum of HorizontalAlignment
                p.VerticalAlignment = 0; //means Top as defined in the enum of VerticalAlignment

                //The polygon can now receive the corner points generated in the hex object
                //this is obtained from a hex, 
                //but the index is found just like how we found the index for the name
                p.Points = hexes[(int)graph.graph2[index].neighbors[updatedDirs[i]].X].corners;
                p.MouseEnter += new MouseEventHandler(highlightHexagon); //can mouseover
                p.MouseLeave += new MouseEventHandler(unhighlightHexagon); //can STOP mouseover
                theCanvas.Children.Add(p); //give the polygon to the canvas for rendering
                polygons.Add(p); //we must keep the same reference here, in case we later must change the visuals

            }
            //LOOP for debugging
            /*
            for (int i = 0; i < polygons.Count; i++)
            {
                Console.WriteLine("Names of hex and pol: ("+hexes[i].name+", "+polygons[i].Name+")");
            }
            Console.WriteLine("Polygon count: " + polygons.Count);
            Console.WriteLine("graph node count: " + graph.Count);
            */
        }

        private void generateHexNeighbors(int index, List<int> updatedDirs)
        {
            //Method for creating new Hex objects to the hexes list.
            //index is used to point to the hex in the center of the new neighbors to-be
            //the updatedDirs is used to derive which neighbors must be made AND where

            Point center = hexes[index].center; //shorthand for the centerPoint representing WHERE the centerhex is
            Point newCenter; //represents the center of the new hexagon to-be

            //each hex must get a new name.
            string newName; 

            //Through updatedIndexes know exactly which new hex objects must be made next to the one at "index"
            //We also know their positions relative to the center
            //new Hex(center,scale,name);
            for (int i = 0; i < updatedDirs.Count; i++) //amount of new neighbors to be made
            {
                //the new centerPoint is gotten from this function, 
                //using the center and the current neighbor direction of updatedDirs
                newCenter = calcHexCenter(center, updatedDirs[i]);

                //for a list of N hexes, the final index is N-1; Therefore the index of the 
                //next new element is N, and this must be that elements name:

                //using [index], we can ask the graph which index the new neighbors are in
                //this neighbor-index is then used to create a name for this new hex
                //first the point.X is made from double to int, then toString.. Brackets galore!!
                //we use graph.graph because the list<list<Point>> variable called "graph",
                //exist in the (class)Graph object that is ALSO called graph
                newName = ((int)(graph.graph2[index].neighbors[updatedDirs[i]].X)).ToString();

                //Now all the information is there, and we can make the new hex
                hexes.Add(new Hex(newCenter, scale, newName)); //create the new Hex object
            }
        }

        private Point calcHexCenter(Point center, int dir)
        {
            //Method for calculating the center of the next hex, 
            //depending on the current center and the direction defined by dir.
            //recall direction order: E, SE, SW, W, NW, NE

            //All the calculations are obtained from *that* mathpage saved on reddit
            //for a "standing" hex, this height is the distance from any corner, to the opposite
            double height = 2.0 * scale; //scale is from center to corner, so 2 of those..
            double width = Math.Sqrt(3) / 2.0 * height; //from center of any edge, to center of opposite edge
            switch (dir)
            {
                case 0: //Means E
                    return new Point(center.X+width,center.Y); 
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
            h.uninitialized = false; //Means the hex is an actual hex and not just a potential hex
            p.Name = "s0"; //using "s0" instead of "0" because just "0" is forbidden 
            p.Fill = (Brush)new BrushConverter().ConvertFromString("#777"); //subject to change, depending on hexagon type
            p.Stroke = (Brush)new BrushConverter().ConvertFromString("#333"); //subject to change, depending on hexagon type
            p.StrokeThickness = strokeThickness; //arbitrary, but should probably be the same for ALL hexes
            mouseInit = mouse.GetPosition(theCanvas); //This must be saved, since it is easy to write down when saving the map to file
            initTransform = new TranslateTransform(mouseInit.X, mouseInit.Y); //This must be saved, since every additional Polygon needs this as a renderTransformation
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

        //Method for turning all the relevant information from the map into a single string
        //It should then be possible to grab this very string and recreate the map exactly as it was
        public override string ToString()
        {
            StringBuilder mapString = new StringBuilder(); //The string that must consist of the map is generated from this builder.

            //First part of this stringbuilder is to save the graph:
            foreach(Vertex v in graph.graph2)
            {
                foreach(Point p in v.neighbors)
                {
                    mapString.AppendFormat(p.X+";"+p.Y);
                    mapString.AppendFormat("%");
                }
                mapString.Length--; //Sneeeaky way of throwing away the last part ;D
                mapString.AppendFormat("$");
                mapString.AppendLine();
            }
            mapString.Length = mapString.Length - 3;
            mapString.AppendLine();
            mapString.AppendFormat("¤");
            mapString.AppendLine();

            //Second part is to save the hexes:
            foreach(Hex h in hexes)
            {
                mapString.AppendFormat(h.center+"%"+h.name+"%"+h.uninitialized+"%"+h.abyss+"$");
                mapString.AppendLine();
            }
            mapString.Length = mapString.Length - 3;
            mapString.AppendLine();
            mapString.AppendFormat("¤");
            mapString.AppendLine();

            //Third part is to save the mouse coordinates from the initial click
            mapString.Append(mouseInit.X +";"+ mouseInit.Y);
            mapString.AppendLine();
            mapString.AppendFormat("¤");
            mapString.AppendLine();

            //Fourth part is to save the polygons:
            foreach(Polygon p in polygons)
            {
                mapString.AppendFormat(p.Name+"%"+p.Fill.ToString()+"%"+p.Stroke.ToString()+"$");
                mapString.AppendLine();
            }
            mapString.Length = mapString.Length - 3;
            mapString.AppendLine();

            //Now return the whole thing as a string
            return mapString.ToString();
        }

        private void fromString(string dataFromFile) //INCOMPLETE
        {
            string[] listsandmouse = dataFromFile.Split('¤');
            string[] graphS = listsandmouse[0].Split('$');
            string[] hexesS = listsandmouse[1].Split('$');
            string mouseS = listsandmouse[2];
            string[] polygonsS = listsandmouse[3].Split('$');

            //digesting the graph part of the file - NOT CORRECT YET
            string[] points, onepoint;
            double x, y;
            List<List<Point>> newGraph = new List<List<Point>>();
            foreach (String s1 in graphS)
            {
                points = s1.Split('%');
                List<Point> newPoints = new List<Point>();
                foreach (String s2 in points)
                {
                    onepoint = s2.Split(';');
                    double.TryParse(onepoint[0], out x);
                    double.TryParse(onepoint[1], out y);
                    newPoints.Add(new Point(x, y));
                }
                newGraph.Add(newPoints);
            }
            graph.graph = newGraph;

            //digesting the hexes part of the file
            string[] onehex;
            bool u, a;
            List<Hex> newHexes = new List<Hex>();
            foreach(string s1 in hexesS)
            {
                onehex = s1.Split('%');
                onepoint = onehex[0].Split('.');
                double.TryParse(onepoint[0], out x);
                double.TryParse(onepoint[1], out y);
                bool.TryParse(onehex[2], out u);
                bool.TryParse(onehex[3], out a);
                newHexes.Add(new Hex(new Point(x,y),scale,onehex[1],u,a));
            }
            hexes = newHexes;

            //digesting the mouseInit part of the file
            onepoint = mouseS.Split(';');
            double.TryParse(onepoint[0], out x);
            double.TryParse(onepoint[1], out y);
            mouseInit = new Point(x, y);

            //digesting the polygons part of the file
            //TODO
        }

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }
}
