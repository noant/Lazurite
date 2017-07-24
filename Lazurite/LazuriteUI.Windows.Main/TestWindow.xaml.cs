using Lazurite.ActionsDomain;
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
using System.Windows.Shapes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Main.Switches;
using Lazurite.IOC;
using Lazurite.Windows.Logging;
using System.Threading;
using Lazurite.MainDomain;
using Lazurite.Windows.Core;
using Lazurite.Security;
using Lazurite.Data;
using Lazurite.Visual;
using Lazurite.Scenarios;
using Lazurite.ActionsDomain.Attributes;
using LazuriteUI.Windows.Main.Constructors.Decomposition;
using LazuriteUI.Icons;
using Lazurite.CoreActions;
using LazuriteUI.Windows.Main.Security.PermissionsViews;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для TestWindows.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();

            grid.Children.Add(new DenyForUsersPermissionView(new Lazurite.Security.Permissions.DenyForUsersPermission()));
        }
    }
}
