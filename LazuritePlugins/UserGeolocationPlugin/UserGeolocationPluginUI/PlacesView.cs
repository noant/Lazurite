using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserGeolocationPlugin;

namespace UserGeolocationPluginUI
{
    public class PlacesView: ListItemsView
    {
        public PlacesView()
        {
            Refresh();
            this.SelectionMode = ListViewItemsSelectionMode.Single;
            this.SelectionChanged += (o, e) =>
            {
                SelectedPlaceChanged?.Invoke(this, new EventsArgs<GeolocationPlace>(this.SelectedPlace));
            };
        }

        public GeolocationPlace SelectedPlace
        {
            get
            {
                return (this.GetSelectedItems().FirstOrDefault() as PlaceItemView)?.Place;
            }
            set
            {
                this.GetItems().Where(x => ((PlaceItemView)x).Place.Name.Equals(value.Name)).All(x => x.Selected = true);
                if (this.SelectedPlace == null)
                    this.SelectedPlaceChanged?.Invoke(this, new EventsArgs<GeolocationPlace>(null));
            }
        }

        public GeolocationPlace[] Places
        {
            get
            {
                return this.GetItems().Cast<PlaceItemView>().Select(x => x.Place).ToArray();
            }
            set
            {
                this.Children.Clear();
                foreach (var place in value)
                    this.Children.Add(new PlaceItemView(place));
            }
        }        

        public void Refresh()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                this.Places = PlacesManager.Current.Places.ToArray();
        }

        public void RemovePlace(GeolocationPlace place)
        {
            this.Places = this.Places.Where(x => x != place).ToArray();
            this.SelectedPlace = this.Places.FirstOrDefault();
            PlacesManager.Current.Places.Remove(place);
            PlacesManager.Save();
        }

        public void AddPlace(GeolocationPlace place)
        {
            this.Places = this.Places.Union(new[] { place }).ToArray();
            this.SelectedPlace = place;
            PlacesManager.Current.Places.Add(place);
            PlacesManager.Save();
        }

        public event EventsHandler<GeolocationPlace> SelectedPlaceChanged;
    }
}
