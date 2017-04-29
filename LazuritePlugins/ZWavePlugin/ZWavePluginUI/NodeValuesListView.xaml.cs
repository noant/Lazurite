using OpenZWrapper;
using LazuriteUI.Controls;
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

namespace ZWavePluginUI
{
    /// <summary>
    /// Логика взаимодействия для NodeValuesListView.xaml
    /// </summary>
    public partial class NodeValuesListView : UserControl
    {
        public NodeValuesListView()
        {
            InitializeComponent();

            this.SelectionChanged += (o, e) => {
                tbDescription.Text = "";
                if (SelectedNodeValue != null)
                {
                    var description =
                        string.Format("Узел: {0}\r\n           {1}\r\n           {2}\r\n            {3}\r\n\r\n", 
                        SelectedNodeValue.Node.ProductName, 
                        SelectedNodeValue.Node.Manufacturer, 
                        SelectedNodeValue.Node.ProductType, 
                        SelectedNodeValue.Node.Name);
                    description += "Параметр узла: " + SelectedNodeValue.Name + "\r\n\r\n";                                        
                    description += "Тип значения: " + Enum.GetName(typeof(OpenZWrapper.ValueType), SelectedNodeValue.ValueType)+ "\r\n\r\n";
                    if (!string.IsNullOrEmpty(SelectedNodeValue.Description))
                        description+= "Описание: " + SelectedNodeValue.Description + "\r\n\r\n";
                    if (!string.IsNullOrEmpty(SelectedNodeValue.Unit))
                        description += "Единица измерения: " + SelectedNodeValue.Unit + "\r\n\r\n";
                    if (SelectedNodeValue.PossibleValues != null && SelectedNodeValue.PossibleValues.Any())
                        description += "Возможные значения: " + SelectedNodeValue.PossibleValues.Aggregate((x1, x2) => x1 + "; " + x2);
                    tbDescription.Text = description;
                }
            };
        }

        private Node _node;
        private ValueGenre _selectedGenre;
        public event RoutedEventHandler SelectionChanged;
        public NodeValue SelectedNodeValue { get; private set; }
        private Func<NodeValue, bool> _nodeValueFilter;

        public void InitializeWith(Node node, NodeValue selectedValue=null, Func<NodeValue, bool> nodeValueFilter = null)
        {
            _nodeValueFilter = nodeValueFilter;
            _node = node;
            SelectedNodeValue = selectedValue;

            if (SelectedNodeValue != null)
            {
                _selectedGenre = SelectedNodeValue.Genre;
                genreList.GetItems()[(int)_selectedGenre].Selected = true;
            }

            RefreshValues();
        }
        
        private void RefreshValues()
        {
            this.SelectedNodeValue = null;
            SelectionChanged?.Invoke(this, new RoutedEventArgs());
            listItems.Children.Clear();
            if (_node != null)
            {
                this.captionView.StartAnimateProgress();
                foreach (var value in _node.Values.Where(x => x.Genre == _selectedGenre))
                {
                    var nodeValueView = new NodeValueView(value);
                    nodeValueView.IsEnabled = _nodeValueFilter == null || _nodeValueFilter(value);
                    if (value.Equals(SelectedNodeValue) && nodeValueView.IsEnabled)
                        nodeValueView.Selected = true;
                    nodeValueView.Click += (o, e) =>
                    {
                        this.SelectedNodeValue = nodeValueView.Selected ? nodeValueView.NodeValue : null;
                        SelectionChanged?.Invoke(this, new RoutedEventArgs());
                    };
                    listItems.Children.Add(nodeValueView);
                }
                this.captionView.StopAnimateProgress();
            }
        }

        private void UserGenreSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (((ISelectable)sender).Selected)
            {
                _selectedGenre = ValueGenre.User;
                RefreshValues();
            }
        }

        private void BasicGenreSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (((ISelectable)sender).Selected)
            {
                _selectedGenre = ValueGenre.Basic;
                RefreshValues();
            }
        }

        private void SystemGenreSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (((ISelectable)sender).Selected)
            {
                _selectedGenre = ValueGenre.System;
                RefreshValues();
            }
        }

        private void ConfigGenreSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (((ISelectable)sender).Selected)
            {
                _selectedGenre = ValueGenre.Config;
                RefreshValues();
            }
        }
    }
}