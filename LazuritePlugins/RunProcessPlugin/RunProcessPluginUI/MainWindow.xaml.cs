using LazuriteUI.Windows.Controls;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace RunProcessPluginUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IRunProcessAction _action;
        public MainWindow(IRunProcessAction action)
        {
            _action = action;
            InitializeComponent();
            btCancel.Click += (o, e) => this.DialogResult = false;
            btApply.Click += (o, e) => {
                Commit();
                this.DialogResult = true;
            };
            btSelectPath.Click += (o, e) => {
                var dialog = new OpenFileDialog();
                if (dialog.ShowDialog() ?? false)
                    tbExePath.Text = dialog.FileName;
            };
            SetCloseProcessMode(_action.CloseMode);
            SetProcessPriority(_action.Priority);
            tbArguments.Text = _action.Arguments;
            tbExePath.Text = _action.ExePath;
        }

        private void Commit()
        {
            _action.Arguments = tbArguments.Text;
            _action.ExePath = tbExePath.Text;
            _action.Priority = GetProcessPriority();
            _action.CloseMode = GetCloseProcessMode();
        }

        private CloseProcessMode GetCloseProcessMode()
        {
            var selectedItem = lvCloseProcessMode.GetSelectedItems().FirstOrDefault();
            if (selectedItem == null)
                return CloseProcessMode.Close;
            else
                return (CloseProcessMode)Enum.Parse(typeof(CloseProcessMode), ((ItemView)selectedItem).Tag.ToString());
        }

        private void SetCloseProcessMode(CloseProcessMode mode)
        {
            var name = Enum.GetName(typeof(CloseProcessMode), mode);
            lvCloseProcessMode.GetItems().Where(x => ((ItemView)x).Tag.ToString().Equals(name)).All(x => x.Selected = true);
        }

        private ProcessPriorityClass GetProcessPriority()
        {
            var selectedItem = lvProcessPriority.GetSelectedItems().FirstOrDefault();
            if (selectedItem == null)
                return ProcessPriorityClass.Normal;
            else
                return (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), ((ItemView)selectedItem).Tag.ToString());
        }

        private void SetProcessPriority(ProcessPriorityClass @class)
        {
            var name = Enum.GetName(typeof(ProcessPriorityClass), @class);
            lvProcessPriority.GetItems().Where(x => ((ItemView)x).Tag.ToString().Equals(name)).All(x => x.Selected = true);
        }
    }
}
