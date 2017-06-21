using Lazurite.IOC;
using Lazurite.Windows.Modules;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        public void Initialize(Type valueType = null, ActionInstanceSide side = ActionInstanceSide.Both, Type selectedType = null)
        {
            var types = _manager.GetModules(valueType, side);
            SelectedType = selectedType;

            var itemViews = types.Select(type =>
            {
                var itemView = new ItemView();
                itemView.Content = Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(type);
                itemView.Icon = Icons.LazuriteIconAttribute.GetIcon(type);
                itemView.Tag = type;
                return itemView;
            }).OrderBy(x => x.Content.ToString());

            foreach (var itemView in itemViews)
                itemsView.Children.Add(itemView);

            itemsView.SelectionMode = ListViewItemsSelectionMode.Single;
            itemsView.SelectionChanged += (o, e) => {
                if (itemsView.GetSelectedItems().Any())
                {
                    SelectedType = (Type)((itemsView.GetSelectedItems().FirstOrDefault() as ItemView).Tag);
                    SelectionChanged?.Invoke(this);
                }
            };
        }

        public static void Show(Action<Type> selectedCallback, Panel parent, Type valueType = null, ActionInstanceSide side = ActionInstanceSide.Both, Type selectedType = null)
        {
            var control = new SelectActionView();
            var dialogView = new DialogView(control);
            control.Initialize(valueType, side, selectedType);
            control.SelectionChanged += (ctrl) => dialogView.Close();
            dialogView.Closed += (o, e) => selectedCallback?.Invoke(control.SelectedType);
            dialogView.Show(parent);
        }

        public Type SelectedType { get; private set; }

        public event Action<SelectActionView> SelectionChanged;
    }
}