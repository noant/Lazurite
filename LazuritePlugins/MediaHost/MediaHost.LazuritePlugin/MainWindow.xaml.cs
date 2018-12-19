using Lazurite.ActionsDomain.ValueTypes;
using MediaHost.Bases;
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

namespace MediaHost.LazuritePlugin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ValueTypeBase valueType, CommandView selectedCommand)
        {
            InitializeComponent();

            var commands = new List<CommandView>();

            foreach (var source in MediaObjects.MediaHostWindow.Sources)
            {
                if (valueType is ToggleValueType || valueType == null)
                    commands.Add(new CommandView(source));
                foreach (var command in source.Commands)
                {
                    if (valueType == null)
                        commands.Add(new CommandView(source, command));
                    else if (valueType is InfoValueType && command.AllowParam && !(command is StatesMediaCommand))
                        commands.Add(new CommandView(source, command));
                    else if (valueType is ButtonValueType && !command.AllowParam)
                        commands.Add(new CommandView(source, command));
                    else if(valueType is StateValueType && command is StatesMediaCommand)
                        commands.Add(new CommandView(source, command));
                }
            }

            cbCommands.Info = new LazuriteUI.Windows.Controls.ComboItemsViewInfo(
                LazuriteUI.Windows.Controls.ListViewItemsSelectionMode.Single,
                commands.ToArray(),
                x => x.ToString(),
                x => LazuriteUI.Icons.Icon.ChevronRight,
                selectedCommand != null ? new[] { selectedCommand } : null,
                "Выбор объекта и комманды для мультимедиа",
                gridHost);

            btOk.Click += (o, e) => DialogResult = true;
            btCancel.Click += (o, e) => DialogResult = false;
        }

        public CommandView SelectedCommand => 
            cbCommands.Info.SelectedObjects.FirstOrDefault() as CommandView;
    }
}
