using LazuriteUI.Windows.Controls;
using OpenZWrapper;
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

namespace ZWPluginUI
{
    /// <summary>
    /// Логика взаимодействия для GenreSelectView.xaml
    /// </summary>
    public partial class GenreSelectView : UserControl
    {
        public GenreSelectView()
        {
            InitializeComponent();
            this.itemView.Selectable = false;
            this.itemView.Icon = LazuriteUI.Icons.Icon.HomeQuestion;
            SelectGenre(null);

            this.itemView.Click += (o, e) => {
                var messageView = new MessageView();
                messageView.ContentText = "Выберите тип отображаемых параметров:";
                messageView.HeaderText = "Тип параметра узла Z-Wave";                
                messageView.Icon = LazuriteUI.Icons.Icon.HomeQuestion;
                messageView.SetItems(new[] {
                    CreateItemInfo(null, messageView),
                    CreateItemInfo(ValueGenre.Basic, messageView),
                    CreateItemInfo(ValueGenre.Config, messageView),
                    CreateItemInfo(ValueGenre.System, messageView),
                    CreateItemInfo(ValueGenre.User, messageView),
                });
                messageView.ShowInNewWindow(width: 800, showDialog: true);
            };
        }

        private MessageItemInfo CreateItemInfo(ValueGenre? genre, MessageView mview)
        {
            var caption = "All";
            if (genre != null)
                caption = Enum.GetName(typeof(ValueGenre), genre);
            return new MessageItemInfo(caption, (m) =>
            {
                SelectGenre(genre);
                mview.Close();
            },
            focused: genre == SelectedGenre);
        }

        private void SelectGenre(ValueGenre? genre)
        {
            SelectedGenre = genre;
            if (genre == null)
                this.itemView.Content = "All";
            else
                this.itemView.Content = Enum.GetName(typeof(ValueGenre), genre);
            SelectedGenreChanged?.Invoke(this, new RoutedEventArgs());
        }

        public ValueGenre? SelectedGenre { get; private set; }

        public event RoutedEventHandler SelectedGenreChanged;
    }
}
