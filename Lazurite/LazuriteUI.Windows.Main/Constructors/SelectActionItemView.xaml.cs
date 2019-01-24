using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для SelectActionItemView.xaml
    /// </summary>
    public partial class SelectActionItemView : Grid, ISelectable
    {
        public SelectActionItemView()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsEnabledProperty)
                Opacity = IsEnabled ? 1 : 0.4;
        }

        public bool Selected { get => itemView.Selected; set => itemView.Selected = value; }
        public bool Selectable { get => itemView.Selectable; set => itemView.Selectable = value; }

        public event RoutedEventHandler SelectionChanged
        {
            add => itemView.SelectionChanged += value;
            remove => itemView.SelectionChanged -= value;
        }

        public string Text
        {
            get => tbText.Text;
            set => tbText.Text = value;
        }

        public Icon Icon
        {
            get => iconView.Icon;
            set => iconView.Icon = value;
        }

        public event RoutedEventHandler Click
        {
            add => itemView.Click += value;
            remove => itemView.Click -= value;
        }
    }
}
