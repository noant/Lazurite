using LazuriteUI.Windows.Controls;
using LazuriteUI.Icons;
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
using Lazurite.Windows.Core;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Server;
using Lazurite.Windows.Utils;
using System.Threading;

namespace LazuriteUI.Windows.WpfTests
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Icon = BitmapFrame.Create(Icons.Utils.GetIconData(Icons.Icon.Lazurite64));

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            this.Loaded += MainWindow_Loaded;

        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Singleton.Resolve<WarningHandlerBase>().Error("Необработанное исключение", e.Exception);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var core = new LazuriteCore();
            core.WarningHandler.OnWrite += WarningHandler_OnWrite;
            core.InitializeAsync(() => {
                Thread.Sleep(2000);

                core.WarningHandler.Debug("set settings");
                
                //core.ScenariosRepository.AddScenario(new SingleActionScenario()
                //{
                //    Name = "Включить компьютер",
                //    TargetAction = new TestButtonAction()
                //});

                //core.ScenariosRepository.AddScenario(new SingleActionScenario()
                //{
                //    Name = "Свет в прихожей",
                //    TargetAction = new TestToggleAction()
                //});

                //core.ScenariosRepository.AddScenario(new SingleActionScenario()
                //{
                //    Name = "Свет в прихожей",
                //    TargetAction = new TestInfoAction()
                //});

                //core.ScenariosRepository.AddScenario(new SingleActionScenario()
                //{
                //    Name = "Тест статусы",
                //    TargetAction = new TestStateAction()
                //});

                var serverSettings = core.Server.GetSettings();
                var sertName = Lazurite.Windows.Server.Utils.AddCertificate("LazuriteStandardCertificate.pfx", "28021992");
                serverSettings.CertificateSubject = sertName;
                Lazurite.Windows.Server.Utils.NetshAddUrlacl(serverSettings.GetAddress());
                Lazurite.Windows.Server.Utils.NetshAddSslCert(serverSettings.CertificateSubject, serverSettings.Port);
                core.Server.SetSettings(serverSettings);

                core.Server.StartAsync((success)=> {
                    Thread.Sleep(3000);
                    core.WarningHandler.Debug("check conn");

                    var scens = core.ClientsFactory.GetServer("192.168.0.100", serverSettings.Port, serverSettings.ServiceName, "0123456789123456", "user1", "pass").GetScenariosInfo();

                    core.WarningHandler.Debug("scens cnt = " + scens.Count);
                });

                //Thread.Sleep(2000);

                //core.WarningHandler.Debug("add user");

                //if (!core.UsersRepository.Users.Any())
                //    core.UsersRepository.Add(new Lazurite.MainDomain.UserBase() {
                //        Login = "user1",
                //        PasswordHash = CryptoUtils.CreatePasswordHash("pass")
                //    });

                //if (!core.PluginsManager.GetPlugins().Any())
                //    core.PluginsManager.AddPlugin(@"D:\Programming\Lazurite\Releases\Plugins\ZWavePlugin.pyp");

                //var zwave = core.PluginsManager.CreateInstanceOf(core.PluginsManager.GetModules().First());

                //this.Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    if (!zwave.UserInitializeWith(new ToggleValueType(), false))
                //        return;

                //core.ScenariosRepository.AddScenario(new SingleActionScenario()
                //{
                //    Name = "Дата выключения света",
                //    TargetAction = new TestDateTimeAction()
                //});

                //}));
            });

        }
            

        private void WarningHandler_OnWrite(object arg1, Lazurite.Windows.Logging.WarningEventArgs args)
        {
            this.Dispatcher.BeginInvoke(new Action(() => {
                this.Title = DateTime.Now.ToString()+ " // " + args.Message;
            }));
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            listItems.SelectionMode = Controls.ListViewItemsSelectionMode.Multiple;

            var dialogView = new DialogView(new Button() { Content = "test" });
            dialogView.Show(grid);
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            listItems.SelectionMode = Controls.ListViewItemsSelectionMode.Single;
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            listItems.SelectionMode = Controls.ListViewItemsSelectionMode.None;

            var messageView = new MessageView();
            messageView.Icon = Icons.Icon.ConfirmYesNo;
            messageView.ContentText = "Вы уверены, что хотите удалить сценарий 'Включить основной свет'?";
            messageView.HeaderText = "Удаление сценария";
            messageView.Show(grid);
            messageView.SetItems(new[] {
                new MessageItemInfo("Да", (mv) => { 
                    mv.ContentText = "Удаление сценария...";
                    mv.StartAnimateProgress();
                    mv.IsItemsEnabled = false;
                    Task.Delay(8000).ContinueWith((t) => {
                        mv.Dispatcher.BeginInvoke(new Action(()=> {
                            mv.StopAnimateProgress();
                            mv.ContentText = "Удаление завершено";
                            mv.Icon = Icons.Icon.PinRemove;
                            mv.SetItems(new [] {
                                new MessageItemInfo("Ок", (mv1) => mv1.Close(), Icons.Icon.Check),
                            });
                            mv.IsItemsEnabled = true;
                        }));
                    });
                }, Icons.Icon.Check),
                new MessageItemInfo("Нет", (mv) => mv.Close(), Icons.Icon.Cancel),
                new MessageItemInfo("Блабла", (mv) => mv.Close()),
            });
        }
    }
}