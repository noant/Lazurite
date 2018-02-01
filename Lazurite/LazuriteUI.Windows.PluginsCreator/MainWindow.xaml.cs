using Lazurite.Utils;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Modules;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace LazuriteUI.Windows.PluginsCreator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WarningHandler _warningHandler = new WarningHandler();

        public MainWindow()
        {
            InitializeComponent();
            Icon = BitmapFrame.Create(Icons.Utils.GetIconData(Icons.Icon.Lazurite64));
            string[] args = Environment.GetCommandLineArgs();
            if (args != null && args.Length > 2)
            {
                _selectedPath = args[1];
                _pluginPath = args[2];
                btSelect.Content = _selectedPath;
                btCreate.Content = _pluginPath;
                CreatePlugin(() => Dispatcher.BeginInvoke(new Action(()=>Close())));
            }
        }

        private string _selectedPath;
        private string _pluginPath;

        private string SelectFolder() {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return fbd.SelectedPath;
            }
            return "";
        }

        private void CreatePlugin(Action callback=null)
        {
            cvTop.StartAnimateProgress();
            TaskUtils.Start(() =>
            {
                try
                {
                    var targetFilePath = System.IO.Path.Combine(System.IO.Path.GetPathRoot(_selectedPath), (string.IsNullOrEmpty(_pluginPath) ? System.IO.Path.GetFileName(_selectedPath) : _pluginPath) + ".pyp");
                    if (!string.IsNullOrEmpty(_pluginPath))
                        targetFilePath = _pluginPath;
                    Console.WriteLine("target file path = " + targetFilePath);
                    Utils.CreatePackage(_selectedPath, targetFilePath);
                }
                catch (Exception e)
                {
                    _warningHandler.Error("Ошибка при создании файла плагина", e);
                    System.Windows.MessageBox.Show(e.Message);
                }
                finally
                {
                    Dispatcher.BeginInvoke(new Action(() => cvTop.StopAnimateProgress()));
                    callback?.Invoke();
                }
            });
        }

        private void btCreate_Click(object sender, RoutedEventArgs e)
        {
            CreatePlugin();
        }

        private void btSelect_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedPath = SelectFolder()))
            {
                btCreate.IsEnabled = true;
                btSelect.Content = _selectedPath;
            }
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(System.IO.Path.Combine(target.FullName, file.Name));
        }
    }
}
