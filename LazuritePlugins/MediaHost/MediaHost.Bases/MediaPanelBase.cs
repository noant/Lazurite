using System;
using System.Collections.Generic;
using System.Windows;

namespace MediaHost.Bases
{
    public class MediaPanelBase : System.Windows.Controls.Grid
    {
        protected Action _needClose;

        public MediaPanelBase(string elementName)
        {
            ElementName = elementName;
        }

        public MediaPanelBase()
        {
            // Empty constructor
        }

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

        public virtual bool IsCompatibleWith(MediaPanelBase panel)
        {
            return true;
        }

        protected virtual bool InitializeInternal()
        {
            return true;
        }

        protected virtual void CloseInternal()
        {
            // Do nothing
        }

        public bool ElementInitialized { get; private set; } = false;

        public string ElementName { get; }
    }
}