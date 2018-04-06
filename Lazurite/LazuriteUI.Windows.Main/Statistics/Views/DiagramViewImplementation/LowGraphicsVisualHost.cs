using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation
{
    public class LowGraphicsVisualHost : FrameworkElement
    {
        private VisualCollection children;

        public LowGraphicsVisualHost()
        {
            children = new VisualCollection(this);
        }

        public void DrawPoints(Point[] points, Brush brush)
        {
            children.Clear();
            var visual = new DrawingVisual();
            children.Add(visual);

            using (var dc = visual.RenderOpen())
            {
                var pen = new Pen(brush, 1);

                if (points.Length > 1)
                {
                    for (int i = 1; i < points.Length; i++)
                    {
                        var p1 = points[i - 1];
                        var p2 = points[i];
                        dc.DrawLine(pen, p1, p2);
                    }
                }
                else if (points.Length == 1)
                {
                    dc.DrawEllipse(brush, pen, points[0], 2, 2);
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get => children.Count;
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= children.Count)
                throw new ArgumentOutOfRangeException();

            return children[index];
        }
    }
}
