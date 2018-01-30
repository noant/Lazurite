using Lazurite.ActionsDomain;
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
        public InfoViewSwitch()
        {
            InitializeComponent();
            KeyDown += (o, e) => {
                if (e.Key == System.Windows.Input.Key.Enter)
                    ApplyClicked?.Invoke(this, new EventsArgs<string>(tbText.Text));
            };
            btApply.Click += (o, e) => ApplyClicked?.Invoke(this, new EventsArgs<string>(tbText.Text));
        }

        public event EventsHandler<string> ApplyClicked;

        public static void Show(Action<string> callbackEnter)
        {
            var @switch = new InfoViewSwitch();
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