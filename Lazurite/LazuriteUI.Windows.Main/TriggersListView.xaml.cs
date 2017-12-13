using Lazurite.IOC;
using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для TriggersListView.xaml
    /// </summary>
    public partial class TriggersListView : UserControl
    {
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>();

        public TriggersListView()
        {
            InitializeComponent();
            this.Loaded += (o,e) => Initialize();
            this.itemsView.SelectionChanged += (o, e) =>
            {
                if (this.SelectionChanging == null)
                {
                    this.SelectedTrigger = (this.itemsView.SelectedItem as ItemView)?.Tag as Lazurite.MainDomain.TriggerBase;
                    this.SelectionChanged?.Invoke(this.SelectedTrigger);
                }
                else
                {
                    this.SelectionChanging(this.SelectedTrigger,
                        new TriggerChangingEventArgs()
                        {
                            Apply = () =>
                            {
                                this.SelectedTrigger = (this.itemsView.SelectedItem as ItemView)?.Tag as Lazurite.MainDomain.TriggerBase;
                                this.SelectionChanged?.Invoke(this.SelectedTrigger);
                            }
                        });
                }
            };
        }

        public void Initialize()
        {
            itemsView.Children.Clear();
            ItemView last = null;
            foreach (var trigger in _repository.Triggers)
                last = AddInternal(trigger);
            if (last != null)
                last.Selected = true;
        }

        public void Refresh(Lazurite.MainDomain.TriggerBase trigger)
        {
            var itemView =
                itemsView
                .Children
                .Cast<ItemView>()
                .First(x => ((Lazurite.MainDomain.TriggerBase)x.Tag).Id.Equals(trigger.Id));

            itemView.Tag = trigger;
            itemView.Content = trigger.Name;
        }

        public void Add(Lazurite.MainDomain.TriggerBase trigger)
        {
            AddInternal(trigger).Selected = true;
        }

        private ItemView AddInternal(Lazurite.MainDomain.TriggerBase trigger)
        {
            var itemView = new ItemView();
            itemView.Content = trigger.Name;
            itemView.Icon = Icons.Icon.ChevronRight;
            itemView.Tag = trigger;
            itemView.Margin = new Thickness(0, 1, 0, 0);
            itemsView.Children.Add(itemView);
            tbTriggersEmpty.Visibility = Visibility.Collapsed;
            return itemView;
        }

        public void Remove(Lazurite.MainDomain.TriggerBase trigger)
        {
            var itemView =
                itemsView
                .Children
                .Cast<ItemView>()
                .First(x => ((Lazurite.MainDomain.TriggerBase)x.Tag).Id.Equals(trigger.Id));
            itemsView.Children.Remove(itemView);
            if (itemsView.GetItems().Any())
                itemsView.GetItems().First().Selected = true;
            else
            {
                this.SelectedTrigger = null;
                this.SelectionChanged?.Invoke(null);
                tbTriggersEmpty.Visibility = Visibility.Collapsed;
            }
        }

        public event Action<Lazurite.MainDomain.TriggerBase> SelectionChanged;
        public event Action<Lazurite.MainDomain.TriggerBase, TriggerChangingEventArgs> SelectionChanging;
        public Lazurite.MainDomain.TriggerBase SelectedTrigger { get; private set; }
    }

    public class TriggerChangingEventArgs
    {
        public Action Apply { get; set; }
    }
}
