using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Windows.Logging;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows.Controls;

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
            
            tbPort.Validation = (v) => EntryViewValidation.UShortValidation().Invoke(v);

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
                    var remoteScenarios = _scenario.GetServer().GetScenariosInfo().Decrypt(_scenario.Credentials.SecretKey).ToArray();
                    if (!remoteScenarios.Any())
                        throw new Exception("На удаленном сервере отсутсвуют сценарии.");
                    var selectScenarioControl = new RemoteScenarioSelect(remoteScenarios, _scenario.RemoteScenarioId);
                    var dialog = new DialogView(selectScenarioControl);
                    selectScenarioControl.ScenarioInfoSelected += (info) =>
                    {
                        dialog.Close();
                        _scenario.RemoteScenarioId = info.ScenarioId;
                        _scenario.RemoteScenarioName = info.Name;
                        tbScenario.Text = _scenario.RemoteScenarioName;
                        var loadWindowCloseToken = MessageView.ShowLoad("Соединение с удаленным сервером...");
                        _scenario.Initialize(_repository, (result) =>
                        {
                            loadWindowCloseToken.Cancel();
                            this.Dispatcher.BeginInvoke(new Action(() => {
                                if (result)
                                {
                                    Modified?.Invoke();
                                    Succeed?.Invoke();
                                }
                                else
                                {
                                    _warningHandler.Error("Невозможно получить список сценариев.");
                                    Failed?.Invoke();
                                }
                            }));
                        });
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
                var loadWindowCloseToken = MessageView.ShowLoad("Соединение с удаленным сервером...");
                _scenario.Initialize(_repository, (result) => {
                    loadWindowCloseToken.Cancel();
                    if (result)
                    {
                        MessageView.ShowMessage("Соединение успешно!", "Тест удаленного сценария", Icons.Icon.Check);
                        this.Dispatcher.BeginInvoke(new Action(() => Succeed?.Invoke()));
                    }
                    else
                    {
                        MessageView.ShowMessage("Невозможно активировать удаленный сценарий!", "Тест удаленного сценария", Icons.Icon.Cancel);
                        this.Dispatcher.BeginInvoke(new Action(() => Failed?.Invoke()));
                    }
                });
            };
        }
        
        private void Refresh(RemoteScenario scenario)
        {
            _scenario = scenario;
            tbHost.Text = _scenario.Credentials.Host;
            tbLogin.Text = _scenario.Credentials.Login;
            tbPassword.Password = _scenario.Credentials.Password;
            tbPort.Text = _scenario.Credentials.Port.ToString();
            tbScenario.Text = _scenario.RemoteScenarioName;
            tbSecretCode.Password = _scenario.Credentials.SecretKey;
            tbServiceName.Text = _scenario.Credentials.ServiceName;
        }

        private void ApplyCurrent()
        {
            _scenario.Credentials = new ConnectionCredentials()
            {
                Host = tbHost.Text,
                Login = tbLogin.Text,
                Password = tbPassword.Password,
                Port = ushort.Parse(string.IsNullOrEmpty(tbPort.Text) ? "0" : tbPort.Text),
                SecretKey = tbSecretCode.Password,
                ServiceName = tbServiceName.Text
            };
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
