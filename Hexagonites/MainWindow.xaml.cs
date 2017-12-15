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
using System.Xml.Serialization;

namespace Hexagonites
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// hehe
    /// </summary>
    public partial class MainWindow : Window
    {
        Map map;
        private bool firstPlaced;
        double scale;
        private Brush whiteBrush, greenBrush, blueBrush, greyBrush, currentBrush, abyssBrush;
        private Point lastMousePos;
        private bool isDragged;
        

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
            initialCanvasPos();
        }


        //Used for placing the canvas in a different position
        //The canvas has a height and a width of 1 million. 
        //By moving it half that size up and left, it seems to stretch forever long!
        //In XAML, the canvas is placed BEFORE the other things inside the parent grid
        //This is so the canvas is "at the bottom" and will stay "below" the other gridrows
        //when you pan it along
        private void initialCanvasPos()
        {
            var matrix = mt.Matrix; // it's a struct
            matrix.Translate(-500000, -500000);
            mt.Matrix = matrix;
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

        private void CanvasRightDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            theCanvas.CaptureMouse();
            //_last = e.GetPosition(canvas);
            lastMousePos = e.GetPosition(this);
            isDragged = true;
        }

        private void CanvasRightUp(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            theCanvas.ReleaseMouseCapture();
            isDragged = false;
            Console.WriteLine("UP");
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged == false)
                return;

            base.OnMouseMove(e);
            if (e.RightButton == MouseButtonState.Pressed && theCanvas.IsMouseCaptured)
            {

                var pos = e.GetPosition(this);
                var matrix = mt.Matrix; // it's a struct
                matrix.Translate(pos.X - lastMousePos.X, pos.Y - lastMousePos.Y);
                mt.Matrix = matrix;
                lastMousePos = pos;
            }
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

        

        private void OpenCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            
            // Create an open file dialog box and only show XAML files.
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "XML files |*.xml";
            // Did they click on the OK button?
            if (true == openDlg.ShowDialog())
            {
                //1st attempt
                // Load all text of selected file.
                //string dataFromFile = File.ReadAllText(openDlg.FileName);
                // Show string in TextBox.
                //map = new Map(theCanvas, scale, highlightHexagon, unhighlightHexagon, dataFromFile);

                //2nd attempt
                //map = ReadFromBinaryFile<Map>(openDlg.FileName);
                //MAYBE REFRESH CANVAS??

                //3rd attempt

            }
            
        }

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "XML files |*.xml";
            // Did they click on the OK button?
            if (true == saveDlg.ShowDialog())
            {
                //1st attempt
                // Save data in the TextBox to the named file.
                //string mapString = map.ToString();
                //File.WriteAllText(saveDlg.FileName, mapString);

                //2nd attempt
                //WriteToBinaryFile<Map>(saveDlg.FileName, map, false);

                //3rd attempt
                MapData md = map.CreateMapData();
                try
                { 
                    XmlSerializer xs = new XmlSerializer(typeof(MapData));
                    TextWriter tw = new StreamWriter(saveDlg.FileName);
                    xs.Serialize(tw, md);
                }
                catch (Exception eeee)
                {
                    Console.WriteLine(eeee.StackTrace);
                }
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
