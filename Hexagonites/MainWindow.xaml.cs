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

        List<Hex> hexes;
        List<Polygon> polygons;
        Hex curHex;
        Polygon curPol;
        private bool firstPlaced;
        double scale;

        public MainWindow()
        {
            hexes = new List<Hex>();
            polygons = new List<Polygon>();
            InitializeComponent();
            scale = 30;
        }

        private void highlightHex(object sender, MouseEventArgs e)
        {
            /*
            //double the stroke value of the currently highlighted hex            
            curPol = (Polygon)sender;
            curPol.StrokeThickness *= 2;
            if (curPol.Name == "myPol1") //WROOOOOOOOONG!!!
            {
                curHex = myTestHex1;
            }
            else if(curPol.Name == "myPol2")
            {
                curHex = myTestHex2;
            }
            else
            {
                curHex = myTestHex3;
            }
            curHex.highlighted = true;
            */
        }

        private void unhighlightHex(object sender, MouseEventArgs e)
        {
            /*
            curPol.StrokeThickness /= 2;
            curHex.highlighted=false;
            /*
            THIS curHex = null; is BAAD!! there is no way of telling if this fires AFTER another hex just said
            curHex = myTestHex.. might unselect it all the time
            */
            //curHex = null;
            //curPol = null;
            
        }

        private void selectHex(object sender, MouseButtonEventArgs e)
        {
            if(curHex.highlighted)
            {
                centerLabel.Text = "Center: " + curHex.center.ToString();
            }
            else
            {
                centerLabel.Text = "center: ";
            }
        }

        private void CanvasLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (!firstPlaced)
            {

                Polygon p = new Polygon();
                Hex h = new Hex(new Point(0,0),scale,"0");
                p.Name = "s0";
                p.Fill = Brushes.White;
                p.Stroke = Brushes.Black;
                p.StrokeThickness = 2;
                p.RenderTransform = new TranslateTransform(e.GetPosition(theCanvas).X,e.GetPosition(theCanvas).Y);
                p.HorizontalAlignment = 0; //means Left as defined in the enum of HorizontalAlignment
                p.VerticalAlignment = 0; //means Top as defined in the enum of VerticalAlignment
                p.Points = h.corners;
                theCanvas.Children.Add(p);
                firstPlaced = true;
            }
        }
    }
}
