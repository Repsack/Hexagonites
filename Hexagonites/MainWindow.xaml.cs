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

        BidirectionalGraph<Hex,HexEdge> myHexGraph; //NOT SETUP CORRECLY JUST YET

        Hex myTestHex1;
        Hex myTestHex2;
        Hex myTestHex3;
        Hex curHex;
        Polygon curPol;
        public MainWindow()
        {
            InitializeComponent();
            double scale = 30;
            myTestHex1 = new Hex(new Point(0,0),scale);
            myTestHex2 = new Hex(new Point(Math.Sqrt(3)*scale,0), scale);
            myTestHex3 = new Hex(new Point(Math.Sqrt(3) * scale / 2, (scale/2)*3), scale);
            myPol1.Points = myTestHex1.corners;
            myPol2.Points = myTestHex2.corners;
            myPol3.Points = myTestHex3.corners;
            curHex = myTestHex1;
            curPol = myPol1;
        }

        private void highlightHex(object sender, MouseEventArgs e)
        {
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
        }

        private void unhighlightHex(object sender, MouseEventArgs e)
        {
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
    }
}
