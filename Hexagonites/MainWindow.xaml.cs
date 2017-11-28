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
using Microsoft.Win32;
using System.IO;

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
        private Brush whiteBrush, greenBrush, blueBrush, greyBrush, currentBrush, abyssBrush;


        public MainWindow()
        {
            InitializeComponent();
            scale = 30;
            map = new Map(theCanvas,scale, highlightHexagon, unhighlightHexagon);
            whiteBrush = (Brush)new BrushConverter().ConvertFromString("#FFF");
            greenBrush = (Brush)new BrushConverter().ConvertFromString("#0D0");
            blueBrush = (Brush)new BrushConverter().ConvertFromString("#006");
            greyBrush = (Brush)new BrushConverter().ConvertFromString("#333");
            abyssBrush = (Brush)new BrushConverter().ConvertFromString("#777");
            currentBrush = whiteBrush;
        }

        private void highlightHexagon(object sender, MouseEventArgs e)
        {
            map.curHighlightPol = (Polygon)sender;
            //Console.WriteLine("Name of polygon: " + map.curPol.Name);
            //Console.WriteLine("Color of polygon: " + map.curPol.Fill.ToString());
            int index;
            int.TryParse(map.curHighlightPol.Name.Substring(1), out index);
            map.curHighlightHex = map.hexes[index];
            if (!map.curHighlightHex.uninitialized)
            {
                map.curHighlightPol.Fill = Brushes.Aqua;
            }
            else
            {
                map.curHighlightPol.Fill = (Brush)new BrushConverter().ConvertFromString("#555");
            }
            map.polHighlighted = true;
        }

        private void unhighlightHexagon(object sender, MouseEventArgs e)
        {
            //map.polHighlighted = false; //DANGEROUS, might be called after "= true" in the highlight method
            Polygon p = (Polygon)sender;
            if (p != map.curSelectedPol || !map.PolSelected)
            {
                if (!map.curHighlightHex.uninitialized)
                {
                    int index;
                    int.TryParse(((Polygon)sender).Name.Substring(1), out index);
                    map.curHighlightPol.Fill = map.hexes[index].curBrush;
                }
                else
                {
                    map.curHighlightPol.Fill = (Brush)new BrushConverter().ConvertFromString("#777");
                }
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
                int index;
                int.TryParse(map.curHighlightHex.name, out index);
                if (map.hexes[index].uninitialized)
                {
                    map.hexes[index].uninitialized = false;
                    map.hexes[index].abyss = false;
                    map.polygons[index].Fill = whiteBrush;
                    map.polygons[index].Stroke = greyBrush;
                    map.generateNeighbors(index);
                }
                else
                {
                    map.hexes[index].curBrush = currentBrush;
                    map.polygons[index].Fill = currentBrush;
                    if(currentBrush == abyssBrush)
                    {
                        map.hexes[index].abyss = true;
                        map.polygons[index].Stroke = greyBrush;
                    }
                }
            }
        }

        private void CanvasLeftUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void typeSelect(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "whiteType":
                    currentBrush = whiteBrush;
                    break;
                case "greenType":
                    currentBrush = greenBrush;
                    break;
                case "blueType":
                    currentBrush = blueBrush;
                    break;
                case "greyType":
                    currentBrush = greyBrush;
                    break;
                case "abyssType":
                    currentBrush = abyssBrush;
                    break;
                default:
                    break;
            }
        }

        private void OpenCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            
            // Create an open file dialog box and only show XAML files.
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Text Files |*.txt";
            // Did they click on the OK button?
            if (true == openDlg.ShowDialog())
            {
                // Load all text of selected file.
                string dataFromFile = File.ReadAllText(openDlg.FileName);
                // Show string in TextBox.
                map = new Map(theCanvas, scale, highlightHexagon, unhighlightHexagon, dataFromFile);
            }
            
        }

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "Text Files |*.txt";
            // Did they click on the OK button?
            if (true == saveDlg.ShowDialog())
            {
                // Save data in the TextBox to the named file.
                string mapString = map.ToString();
                File.WriteAllText(saveDlg.FileName, mapString);
            }
            
        }

        private void SaveCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
