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
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Controls;
using Lazurite.Windows.Logging;
using Lazurite.IOC;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для RemoteScenarioView.xaml
    /// </summary>
    public partial class RemoteScenarioView : UserControl, IScenarioConstructorView
    {
        private RemoteScenario _scenario;
        private WarningHandlerBase _warningHandler = Singleton.Resolve<WarningHandlerBase>();
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>();
        
        public RemoteScenarioView(RemoteScenario scenario)
        {
            InitializeComponent();
            Refresh(scenario);
            
            tbPort.Validation = (str) => ushort.Parse(str);

            tbServiceName.TextChanged += (o, e) => ApplyCurrent();
            tbSecretCode.PasswordChanged += (o, e) => ApplyCurrent();
            tbPort.TextChanged += (o, e) => ApplyCurrent();
            tbPassword.PasswordChanged += (o, e) => ApplyCurrent();
            tbLogin.TextChanged += (o, e) => ApplyCurrent();
            tbHost.TextChanged += (o, e) => ApplyCurrent();
            btScenariosList.Click += (o, e) =>
            {
                try
                {
                    var remoteScenarios = _scenario.GetServer().GetScenariosInfo().Decrypt(_scenario.SecretKey).ToArray();
                    if (!remoteScenarios.Any())
                        throw new Exception("На удаленном сервере отсутсвуют сценарии.");
                    var selectScenarioControl = new RemoteScenarioSelect(remoteScenarios, _scenario.RemoteScenarioId);
                    var dialog = new DialogView(selectScenarioControl);
                    selectScenarioControl.ScenarioInfoSelected += (info) => {
                        dialog.Close();
                        _scenario.RemoteScenarioId = info.ScenarioId;
                        _scenario.RemoteScenarioName = info.Name;
                        _scenario.Initialize(_repository);
                        Modified?.Invoke();
                        Succeed?.Invoke();
                    };
                    dialog.Show();
                }
                catch (Exception exception)
                {
                    _warningHandler.Error("Невозможно получить список сценариев.", exception);
                    Failed?.Invoke();
                }
            };
            btTest.Click += (o, e) => {
                if (_scenario.Initialize(_repository))
                {
                    MessageView.ShowMessage("Соединение успешно!", "Тест удаленного сценария", Icons.Icon.Check);
                    Succeed?.Invoke();
                }
                else
                {
                    MessageView.ShowMessage("Невозможно активировать удаленный сценарий!", "Тест удаленного сценария", Icons.Icon.Cancel);
                    Failed?.Invoke();
                }
            };
        }
        
        private void Refresh(RemoteScenario scenario)
        {
            _scenario = scenario;
            tbHost.Text = _scenario.AddressHost;
            tbLogin.Text = _scenario.UserLogin;
            tbPassword.Password = _scenario.Password;
            tbPort.Text = _scenario.Port.ToString();
            tbScenario.Text = _scenario.RemoteScenarioName;
            tbSecretCode.Password = _scenario.SecretKey;
            tbServiceName.Text = _scenario.ServiceName;
        }

        private void ApplyCurrent()
        {
            _scenario.AddressHost = tbHost.Text;
            _scenario.UserLogin = tbLogin.Text;
            _scenario.Password = tbPassword.Password;
            _scenario.Port = ushort.Parse(string.IsNullOrEmpty(tbPort.Text) ? "0" : tbPort.Text);
            _scenario.SecretKey = tbSecretCode.Password;
            _scenario.ServiceName = tbServiceName.Text;
            Modified?.Invoke();
            Failed?.Invoke();
        }

        public event Action Failed;
        public event Action Modified;
        public event Action Succeed;

        public void Revert(ScenarioBase scenario)
        {
            Refresh((RemoteScenario)scenario);
        }
    }
}
