using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation
{
    public static class Utils
    {
        public static Point Translate(int second, double yVal, int secondStart, int secondEnd, double yMin, double yMax, double translateToWidth, double translateToHeight)
        {
            var rangeX = secondEnd - secondStart;
            var kX = translateToWidth / rangeX;
            var x = (second - secondStart) * kX;

            var rangeY = yMax - yMin;
            var kY = translateToHeight / rangeY;
            var y = translateToHeight - ((yVal - yMin) * kY);

            return new Point()
            {
                X = double.IsNaN(x) ? 0 : x,
                Y = y
            };
        }
    }
}
