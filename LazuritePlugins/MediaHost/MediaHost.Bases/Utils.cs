using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace MediaHost.Bases
{
    public static class Utils
    {
        // https://dzimchuk.net/best-way-to-get-dpi-value-in-wpf/
        public static Size TransformToPixels(Visual visual, Size size)
        {
            Matrix matrix;
            var source = PresentationSource.FromVisual(visual);
            if (source != null)
            {
                matrix = source.CompositionTarget.TransformToDevice;
            }
            else
            {
                using (var src = new HwndSource(new HwndSourceParameters()))
                {
                    matrix = src.CompositionTarget.TransformToDevice;
                }
            }

            return new Size(matrix.M11 * size.Width, matrix.M22 * size.Height);
        }

        public static Size TransformFromPixels(Visual visual, Size size)
        {
            Matrix matrix;
            var source = PresentationSource.FromVisual(visual);
            if (source != null)
            {
                matrix = source.CompositionTarget.TransformToDevice;
            }
            else
            {
                using (var src = new HwndSource(new HwndSourceParameters()))
                {
                    matrix = src.CompositionTarget.TransformToDevice;
                }
            }

            return new Size(size.Width / matrix.M11, size.Height / matrix.M22);
        }
    }
}