using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для ComboBoxItemsView_List.xaml
    /// </summary>
    public partial class ComboBoxItemsView_List : UserControl
    {
        private ComboItemsViewInfo _info;
        public Action<ComboItemsViewInfo> OkClicked { get; }

        public ComboBoxItemsView_List(ComboItemsViewInfo info, Action<ComboItemsViewInfo> okClicked)
        {
            InitializeComponent();

            _info = info;
            OkClicked = okClicked ?? throw new ArgumentNullException(nameof(okClicked));
            itemsView.SelectionMode = info.SelectionMode;
            
            foreach (var obj in info.Objects)
            {
                var itemView = new ItemView();
                itemView.Icon = info.GetIcon != null ? info.GetIcon(obj) : Icons.Icon._None;
                var caption = info.GetCaption(obj);
                if (caption.Length > 94)
                {
                    itemView.ToolTip = caption;
                    caption = caption.Substring(0, 92) + "...";
                }
                itemView.Content = caption;
                itemView.Tag = obj;
                itemView.Margin = new Thickness(1);

                itemsView.Children.Add(itemView);
            }

            foreach (var item in itemsView.GetItems().Cast<ItemView>())
                if (info.SelectedObjects?.Contains(item.Tag) ?? false)
                    item.Selected = true;

            lblCaption.Content = info.Caption;
        }


        public ComboItemsViewInfo GetResult()
        {
            var selected = itemsView.GetSelectedItems().Cast<ItemView>().Select(x => x.Tag).ToArray();

            return new ComboItemsViewInfo(_info.SelectionMode, _info.Objects, _info.GetCaption, _info.GetIcon, selected, _info.Caption, _info.MainPanel);
        }

        private void btApply_Click(object sender, RoutedEventArgs e)
        {
            OkClicked(GetResult());
        }
    }
}
