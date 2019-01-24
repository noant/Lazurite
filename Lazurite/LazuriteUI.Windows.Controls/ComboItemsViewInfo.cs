using LazuriteUI.Icons;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Controls
{
    public class ComboItemsViewInfo
    {
        public ComboItemsViewInfo(ListViewItemsSelectionMode selectionMode, object[] objects, Func<object, string> getCaption, Func<object, Icon> getIcon, object[] selectedObjects, string caption, Panel mainPanel)
        {
            SelectionMode = selectionMode;
            Objects = objects ?? throw new ArgumentNullException(nameof(objects));
            GetCaption = getCaption ?? throw new ArgumentNullException(nameof(getCaption));
            MainPanel = mainPanel ?? throw new ArgumentNullException(nameof(mainPanel));
            GetIcon = getIcon;
            SelectedObjects = selectedObjects;
            Caption = caption;
        }

        public Panel MainPanel { get; private set; }
        public ListViewItemsSelectionMode SelectionMode { get; private set; }
        public object[] Objects { get; private set; }
        public Func<object, string> GetCaption { get; private set; }
        public Func<object, Icon> GetIcon { get; private set; }
        public object[] SelectedObjects { get; private set; }
        public string Caption { get; private set; }
    }
}
