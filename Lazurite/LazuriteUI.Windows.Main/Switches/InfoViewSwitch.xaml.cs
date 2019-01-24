using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для InfoInitializationView.xaml
    /// </summary>
    public partial class InfoViewSwitch : UserControl
    {
        public InfoViewSwitch(bool numeric = false, double min = 0, double max = 100)
        {
            InitializeComponent();
            KeyDown += (o, e) => {
                if (e.Key == System.Windows.Input.Key.Enter)
                    ApplyClicked?.Invoke(this, new EventsArgs<string>(tbText.Text));
            };
            if (numeric)
                tbText.Validation = EntryViewValidation.DoubleValidation(string.Empty, min, max);
            btApply.Click += (o, e) => ApplyClicked?.Invoke(this, new EventsArgs<string>(tbText.Text));
        }

        public event EventsHandler<string> ApplyClicked;

        public static void Show(Action<string> callbackEnter, bool numeric = false, double min = 0, double max = 100)
        {
            var @switch = new InfoViewSwitch(numeric, min, max);
            var dialog = new DialogView(@switch);
            @switch.ApplyClicked += (o, e) =>
            {
                callbackEnter?.Invoke(e.Value);
                dialog.Close();
            };
            dialog.Show();
        }
    }
}