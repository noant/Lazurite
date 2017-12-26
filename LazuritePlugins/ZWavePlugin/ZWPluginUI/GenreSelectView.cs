using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZWPluginUI
{
    public class GenreSelectView: ItemView
    {
        public GenreSelectView()
        {
            this.Selectable = false;
            this.Icon = LazuriteUI.Icons.Icon.HomeQuestion;
            SelectGenre(null);

            this.Click += (o, e) => {
                var messageView = new MessageView();
                messageView.SetItems(new[] {
                    CreateItemInfo(null),
                    CreateItemInfo(ValueGenre.Basic),
                    CreateItemInfo(ValueGenre.Config),
                    CreateItemInfo(ValueGenre.System),
                    CreateItemInfo(ValueGenre.User),
                });
                messageView.ShowInNewWindow();
            };
        }
        
        private MessageItemInfo CreateItemInfo(ValueGenre? genre)
        {
            var caption = "All";
            if (genre != null)
                caption = Enum.GetName(typeof(ValueGenre), genre);
            return new MessageItemInfo(caption, (m) => SelectGenre(genre));
        }

        private void SelectGenre(ValueGenre? genre)
        {
            SelectedGenre = genre;
            if (genre == null)
                this.Content = "All";
            else
                this.Content = Enum.GetName(typeof(ValueGenre), genre);
            SelectedGenreChanged?.Invoke(this, new RoutedEventArgs());
        }

        public ValueGenre? SelectedGenre { get; private set; }

        public event RoutedEventHandler SelectedGenreChanged;
    }
}
