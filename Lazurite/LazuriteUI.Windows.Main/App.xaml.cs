using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Windows.Core;
using Lazurite.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Lazurite.Windows.Logging;
using System.Windows.Controls;
using LazuriteUI.Windows.Controls;
using static LazuriteUI.Windows.Main.TestWindow;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Windows.Main.Journal;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            var core = new LazuriteCore();
            core.Initialize();

            core.WarningHandler.OnWrite += (o, e) => {
                JournalManager.Set(e.Message, e.Type, e.Exception);
            };

            core.Server.StartAsync(null);
            Singleton.Add(core);
            //core.UsersRepository.Add(new Lazurite.MainDomain.UserBase()
            //{
            //    Login = "user1",
            //    PasswordHash = CryptoUtils.CreatePasswordHash("pass")
            //});

            for (int i = 0; i <= -1; i++)
            {
                var scenario = new SingleActionScenario();
                scenario.TargetAction = new ToggleTestAction();
                scenario.Initialize(null);
                scenario.Name = "Переключатель";
                core.ScenariosRepository.AddScenario(scenario);

                var scenario0 = new SingleActionScenario();
                scenario0.TargetAction = new ToggleTestAction();
                scenario0.Initialize(null);
                scenario0.Name = "Свет в коридоре";
                core.ScenariosRepository.AddScenario(scenario0);

                var scenario3 = new SingleActionScenario();
                scenario3.Name = "Свет в ванной";
                scenario3.TargetAction = new StatusTestAction();
                scenario3.Initialize(null);
                core.ScenariosRepository.AddScenario(scenario3);

                var scenario2 = new SingleActionScenario();
                scenario2.TargetAction = new FloatTestAction() {
                    ValueType = new FloatValueType() {
                        AcceptedValues = new[] {"-1", "1" }
                    }
                };
                scenario2.Initialize(null);
                scenario2.Name = "Уровень звука";
                core.ScenariosRepository.AddScenario(scenario2);
                core.VisualSettingsRepository.Add(new UserVisualSettings() { ScenarioId = scenario2.Id, AddictionalData = new[] { "Sound2" }, UserId="0" });

                var scenario4 = new SingleActionScenario();
                scenario4.Name = "Компьютер";
                scenario4.TargetAction = new ButtonTestAction();
                scenario4.Initialize(null);
                core.ScenariosRepository.AddScenario(scenario4);
                core.VisualSettingsRepository.Add(new UserVisualSettings() { ScenarioId = scenario4.Id, AddictionalData = new[] { "TvNews" }, UserId = "0" });

                var scenario5 = new SingleActionScenario();
                scenario5.Name = "Свет в ванной";
                scenario5.TargetAction = new DateTimeTestAction();
                scenario5.Initialize(null);
                core.ScenariosRepository.AddScenario(scenario5);
                core.VisualSettingsRepository.Add(new UserVisualSettings() { ScenarioId = scenario5.Id, AddictionalData = new[] { "TvNews" }, UserId = "0" });
            }
        }
        
    }
}
