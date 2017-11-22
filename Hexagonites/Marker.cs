using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hexagonites
{
    internal class Marker
    {
        private Canvas theCanvas;
        private double scale;
        private Action<object, MouseEventArgs> highlightHexagon;
        private Action<object, MouseEventArgs> unhighlightHexagon;

        public Marker(Canvas theCanvas, double scale, Action<object, MouseEventArgs> highlightHexagon, Action<object, MouseEventArgs> unhighlightHexagon)
        {
            this.theCanvas = theCanvas;
            this.scale = scale;
            this.highlightHexagon = highlightHexagon;
            this.unhighlightHexagon = unhighlightHexagon;
        }


    }
}