using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWPluginUI
{
    public class ControllersListView: ListItemsView
    {
        public void RefreshWith(Controller[] controllers, ZWaveManager manager)
        {
            var selected = this.SelectedController;
            Children.Clear();
            foreach (var controller in controllers)
                this.Children.Add(new ControllerView(controller, manager));
            this.SelectedController = selected;
        }

        public Controller SelectedController
        {
            get
            {
                return GetItems().Cast<ControllerView>().FirstOrDefault(x => x.Selected)?.Controller;
            }
            set
            {
                GetItems().Cast<ControllerView>().Where(x => x.Controller.Equals(value)).All(x => x.Selected = true);
            }
        }
    }
}
