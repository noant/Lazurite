using Lazurite.IOC;
using Lazurite.Shared.ActionCategory;
using Lazurite.Windows.Modules;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для SelectActionView.xaml
    /// </summary>
    public partial class SelectActionView : UserControl
    {
        private PluginsManager _manager = Singleton.Resolve<PluginsManager>();

        public SelectActionView()
        {
            InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        public void Initialize(Type valueType = null, ActionInstanceSide side = ActionInstanceSide.Both, Type selectedType = null)
        {
            var types = _manager.GetModules();
            var targetTypes = _manager.GetModules(valueType, side);

            SelectedType = selectedType;

            foreach (var type in types)
            {
                var itemView = new SelectActionItemView();
                itemView.Text = Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(type);
                itemView.Icon = Icons.LazuriteIconAttribute.GetIcon(type);
                if (itemView.Icon == Icons.Icon._None)
                    itemView.Icon = Icons.Icon.Brick;
                itemView.HorizontalAlignment = HorizontalAlignment.Stretch;
                itemView.Tag = type;
                itemView.Click += (o, e) => {
                    SelectedType = itemView.Tag as Type;
                    SelectionChanged?.Invoke(this);
                };
                itemView.IsEnabled = targetTypes.Any(x => x.Equals(type));
                var category = CategoryAttribute.Get(type);
                switch (category)
                {
                    case Category.Control:
                        itemsViewControl.Children.Add(itemView);
                        break;
                    case Category.DateTime:
                        itemsViewDateTime.Children.Add(itemView);
                        break;
                    case Category.Geolocation:
                        itemsViewGeolocation.Children.Add(itemView);
                        break;
                    case Category.Meta:
                        itemsViewMeta.Children.Add(itemView);
                        break;
                    case Category.Multimedia:
                        itemsViewMedia.Children.Add(itemView);
                        break;
                    case Category.Administration:
                    case Category.Other:
                        itemsViewOther.Children.Add(itemView);
                        break;
                }
            }
        }

        public static void Show(Action<Type> selectedCallback, Type valueType = null, ActionInstanceSide side = ActionInstanceSide.Both, Type selectedType = null)
        {
            var control = new SelectActionView();
            var dialogView = new DialogView(control);
            control.Initialize(valueType, side, selectedType);
            control.SelectionChanged += (ctrl) =>
            {
                selectedCallback?.Invoke(control.SelectedType);
                dialogView.Close();
            };
            dialogView.Show();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = tbSearch.Text.Trim().ToUpper();
            foreach (SelectActionItemView item in GetAllItems())
            {
                if (string.IsNullOrEmpty(txt) || item.Text.ToUpper().Contains(txt))
                    item.Visibility = Visibility.Visible;
                else item.Visibility = Visibility.Collapsed;
            }
        }

        private ISelectable[] GetAllItems()
        {
            return itemsViewControl.GetItems()
                .Union(itemsViewDateTime.GetItems())
                .Union(itemsViewGeolocation.GetItems())
                .Union(itemsViewMedia.GetItems())
                .Union(itemsViewMeta.GetItems())
                .Union(itemsViewOther.GetItems())
                .ToArray();
        }

        public Type SelectedType { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<SelectActionView> SelectionChanged;
    }
}