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

        public void DrawLines(List<Line> lines, Brush brush)
        {
            children.Clear();
            var visual = new DrawingVisual();
            children.Add(visual);

            Optimize(lines);

            var pen = new Pen(brush, 1);
            using (var dc = visual.RenderOpen())
                foreach (var line in lines)
                    dc.DrawLine(pen, line.Point1, line.Point2);
        }

        public void DrawLines(List<ColoredLine> coloredLines)
        {
            children.Clear();
            var visual = new DrawingVisual();
            children.Add(visual);

            var lines = coloredLines.Cast<Line>().ToList();

            Optimize(lines);

            using (var dc = visual.RenderOpen())
                foreach (var line in lines)
                    dc.DrawLine(new Pen(((ColoredLine)line).Brush, 4), line.Point1, line.Point2);
        }

        public void DrawPoint(Point p, Brush brush)
        {
            children.Clear();
            var visual = new DrawingVisual();
            children.Add(visual);
            var pen = new Pen(brush, 1);
            using (var dc = visual.RenderOpen())
                dc.DrawEllipse(brush, pen, p, 2, 2);
        }

        private void Optimize(List<Line> lines)
        {
            var changesCnt = 1;
            while (changesCnt != 0)
            {
                changesCnt = 0;
                for (int i = 1; i < lines.Count; i++)
                {
                    var line1 = lines[i - 1];
                    var line2 = lines[i];
                    if (line1.IsOneLine(line2))
                    {
                        line1.Merge(line2);
                        lines.Remove(line2);
                        changesCnt++;
                    }
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

    public class Line
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }

        public bool IsOneLine(Line line)
        {
            if (Point2.Equals(line.Point1))
            {
                if (Point1.Equals(Point2))
                    return true;
                else
                    return Math.Abs((Point1.X - Point2.X) / (Point1.Y - Point2.Y)) 
                        == Math.Abs((Point2.X * line.Point2.X) / (Point2.Y - line.Point2.Y));
            }
            return false;
        }

        public void Merge(Line line)
        {
            Point2 = line.Point2;
        }
    }

    public class ColoredLine : Line
    {
        public Brush Brush { get; set; }
    }
}
