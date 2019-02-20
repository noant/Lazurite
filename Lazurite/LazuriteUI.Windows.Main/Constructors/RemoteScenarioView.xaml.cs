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
        public static readonly WarningHandlerBase WarningHandler = Singleton.Resolve<WarningHandlerBase>();

        private RemoteScenario _scenario;

        public RemoteScenarioView(RemoteScenario scenario)
        {
            InitializeComponent();
            Refresh(scenario);

            tbPort.Validation = (v) => EntryViewValidation.UShortValidation().Invoke(v);

            tbSecretCode.PasswordChanged += (o, e) => ApplyCurrent();
            tbPort.TextChanged += (o, e) => ApplyCurrent();
            tbPassword.PasswordChanged += (o, e) => ApplyCurrent();
            tbLogin.TextChanged += (o, e) => ApplyCurrent();
            tbHost.TextChanged += (o, e) => ApplyCurrent();
            btExistingCredentials.Click += (o, e) =>
            {
                var credentialsSelect = new ExistingConnectionSelect(ServiceClientFactory.Current.ConnectionCredentials);
                var dialog = new DialogView(credentialsSelect);
                credentialsSelect.SelectedCredentialsChanged += (o1, args) =>
                {
                    tbHost.Text = args.Value.Host;
                    tbLogin.Text = args.Value.Login;
                    tbPassword.Password = args.Value.Password;
                    tbPort.Text = args.Value.Port.ToString();
                    tbSecretCode.Password = args.Value.SecretKey;
                    Modified?.Invoke();
                    dialog.Close();
                };
                dialog.Show();
            };
            btScenariosList.Click += async (o, e) =>
            {
                using (MessageView.ShowLoad("Загрузка списка сценариев..."))
                {
                    try
                    {
                        var remoteScenarios = await _scenario.GetClient().GetScenariosInfo();

                        if (!remoteScenarios.Any())
                        {
                            throw new Exception("На удаленном сервере отсутсвуют сценарии.");
                        }

                        var selectScenarioControl = new RemoteScenarioSelect(remoteScenarios, _scenario.RemoteScenarioId);
                        var dialog = new DialogView(selectScenarioControl);
                        selectScenarioControl.ScenarioInfoSelected += async (info) =>
                        {
                            dialog.Close();
                            _scenario.RemoteScenarioId = info.ScenarioId;
                            _scenario.RemoteScenarioName = info.Name;
                            tbScenario.Text = _scenario.RemoteScenarioName;
                            var loadWindowCloseToken = MessageView.ShowLoad("Соединение с удаленным сервером...");
                            var initialized = await _scenario.Initialize();
                            loadWindowCloseToken.Cancel();
                            if (initialized)
                            {
                                Modified?.Invoke();
                                Succeed?.Invoke();
                            }
                            else
                            {
                                WarningHandler.Error("Невозможно получить список сценариев.");
                                MessageView.ShowMessage("Невозможно получить список сценариев.", "Тест удаленного сценария", Icons.Icon.Cancel);
                                Failed?.Invoke();
                            }
                        };
                        dialog.Show();
                    }
                    catch (Exception exception)
                    {
                        WarningHandler.Error("Невозможно получить список сценариев.", exception);
                        MessageView.ShowMessage("Невозможно получить список сценариев.", "Тест удаленного сценария", Icons.Icon.Cancel);
                        Failed?.Invoke();
                    }
                }
            };
            btTest.Click += async (o, e) =>
            {
                var loadWindowCloseToken = MessageView.ShowLoad("Соединение с удаленным сервером...");
                var initialized = await _scenario.Initialize();
                loadWindowCloseToken.Cancel();
                if (initialized)
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
            tbHost.Text = _scenario.Credentials.Host;
            tbLogin.Text = _scenario.Credentials.Login;
            tbPassword.Password = _scenario.Credentials.Password;
            tbPort.Text = _scenario.Credentials.Port.ToString();
            tbScenario.Text = _scenario.RemoteScenarioName;
            tbSecretCode.Password = _scenario.Credentials.SecretKey;
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