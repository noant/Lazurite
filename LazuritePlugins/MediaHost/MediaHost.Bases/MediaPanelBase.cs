using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace MediaHost.Bases
{
    public class MediaPanelBase: System.Windows.Controls.Grid
    {
        protected Action _needClose;

        public MediaPanelBase(string elementName)
        {
            ElementName = elementName;
        }

        public MediaPanelBase() { }

        public IDataManager DataManager { get; set; }

        public IReadOnlyCollection<MediaCommandBase> Commands { get; protected set; } = new MediaCommandBase[0];

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;
            Visibility = Visibility.Visible;
            ElementInitialized = InitializeInternal();
        }

        public virtual void CoreInitialize(Action needClose)
        {
            _needClose = needClose;
        }

        public void Close()
        {
            if (ElementInitialized)
            {
                CloseInternal();
                ElementInitialized = false;
            }
        }

        // https://dzimchuk.net/best-way-to-get-dpi-value-in-wpf/
        public Size TransformToPixels()
        {
            Matrix matrix;
            var source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                matrix = source.CompositionTarget.TransformToDevice;
            }
            else
            {
                using (var src = new HwndSource(new HwndSourceParameters()))
                    matrix = src.CompositionTarget.TransformToDevice;
            }

            return new Size(matrix.M11 * Width, matrix.M22 * Height);
        }

        public virtual bool IsCompatibleWith(MediaPanelBase panel)
        {
            return true;
        }

        protected virtual bool InitializeInternal()
        {
            return true;
        }

        protected virtual void CloseInternal() { }

        public bool ElementInitialized { get; private set; } = false;

        public string ElementName { get; }
    }
}
