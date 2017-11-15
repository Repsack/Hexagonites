using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuickGraph;

namespace Hexagonites
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// hehe
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
        IMutableVertexListGraph defines methods to add and remove vertices,
        IMutableEdgeListGraph defines methods to add and remove edges
        IMutableVertexAndEdgeListGraph merges the two above concepts, 
        */
        //BidirectionalGraph<Hex,HexEdge> myHexGraph; //NOT SETUP CORRECLY JUST YET

        Map map;
        private bool firstPlaced;
        double scale;

        public MainWindow()
        {
            InitializeComponent();
            scale = 30;
            map = new Map(theCanvas,scale, highlightHexagon, unhighlightHexagon);
        }

        private void highlightHexagon(object sender, MouseEventArgs e)
        {
            map.curHighlightPol = (Polygon)sender;
            //Console.WriteLine("Name of polygon: " + map.curPol.Name);
            //Console.WriteLine("Color of polygon: " + map.curPol.Fill.ToString());
            int index;
            int.TryParse(map.curHighlightPol.Name.Substring(1), out index);
            map.curHighlightHex = map.hexes[index];
            map.curHighlightPol.Fill = Brushes.Aqua;
            map.polHighlighted = true;
        }

        private void unhighlightHexagon(object sender, MouseEventArgs e)
        {
            map.polHighlighted = false;
            Polygon p = (Polygon)sender;
            if (p != map.curSelectedPol || !map.PolSelected)
            {
                //Console.WriteLine("Name of polygon: " + map.curPol.Name);
                //Console.WriteLine("Color of polygon: " + map.curPol.Fill.ToString());
                map.curHighlightPol.Fill = Brushes.White;
            }
            else
            {
                map.curHighlightPol.Fill = Brushes.Blue;
            }
        }

        private void CanvasLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (!firstPlaced)
            {
                map.placeFirst(sender,e);
                firstPlaced = true;
            }
            else
            {
                Console.WriteLine("CanvasLeftDown called not-first time");
                if (map.polHighlighted)
                {
                    map.curSelectedPol = map.curHighlightPol;
                    map.curSelectedHex = map.curHighlightHex;
                    map.curSelectedPol.Fill = Brushes.Blue;
                    map.PolSelected = true;
                    if (map.curSelectedPol == map.curHighlightPol)
                    {
                        map.curSelectedPol.Fill = Brushes.Aqua;
                    }
                }
                else
                {
                    if (map.curHighlightPol != null)
                    {
                        map.curHighlightPol.Fill = Brushes.White;
                        map.PolSelected = false;
                    }
                }
            }
        }
    }
}
