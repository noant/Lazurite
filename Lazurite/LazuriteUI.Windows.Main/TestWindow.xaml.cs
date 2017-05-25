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
            Singleton.Add(new WarningHandler());
            Singleton.Add(new FileSavior());
            Singleton.Add(new UsersRepository());

            var scens = new List<ScenarioBase>();
            var visualSettings = new List<UserVisualSettings>();

            var scenario = new SingleActionScenario();
            scenario.TargetAction = new ToggleTestAction();
            scenario.Initialize(null);
            scenario.Name = "Переключатель";
            scens.Add(scenario);

            var scenario0 = new SingleActionScenario();
            scenario0.TargetAction = new ToggleTestAction();
            scenario0.Initialize(null);
            scenario0.Name = "Свет в коридоре";
            scens.Add(scenario0);

            var scenario3 = new SingleActionScenario();
            scenario3.Name = "Свет в ванной";
            scenario3.TargetAction = new StatusTestAction();
            scenario3.Initialize(null);
            scens.Add(scenario3);
            
            var scenario2 = new SingleActionScenario();
            scenario2.TargetAction = new FloatTestAction();
            scenario2.Initialize(null);
            scenario2.Name = "Уровень звука";
            scens.Add(scenario2);
            visualSettings.Add(new UserVisualSettings() { ScenarioId = scenario2.Id, AddictionalData = new[] { "Sound2", "LightbulbHue" } });
            
            var scenario4 = new SingleActionScenario();
            scenario4.Name = "Компьютер";
            scenario4.TargetAction = new ButtonTestAction();
            scenario4.Initialize(null);
            scens.Add(scenario4);
            visualSettings.Add(new UserVisualSettings() { ScenarioId = scenario4.Id, AddictionalData = new[] { "TvNews", "LightbulbHue" } });

            var scenario5 = new SingleActionScenario();
            scenario5.Name = "Свет в ванной";
            scenario5.TargetAction = new DateTimeTestAction();
            scenario5.Initialize(null);
            scens.Add(scenario5);
            visualSettings.Add(new UserVisualSettings() { ScenarioId = scenario5.Id, AddictionalData = new[] { "TvNews", "LightbulbHue" } });

            switches.Initialize(scens.ToArray(), visualSettings.ToArray());
        }

        public class ToggleTestAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "";
                }
                set
                {
                }
            }

            public bool IsSupportsEvent
            {
                get
                {
                    return true;
                }
            }

            public ValueTypeBase ValueType
            {
                get
                {
                    return new ToggleValueType();
                }

                set
                {
                }
            }

            public event ValueChangedDelegate ValueChanged;

            private string _val = ToggleValueType.ValueON;

            public string GetValue(Lazurite.ActionsDomain.ExecutionContext context)
            {
                return _val;
            }

            public void Initialize()
            {
            }

            public void SetValue(Lazurite.ActionsDomain.ExecutionContext context, string value)
            {
                _val = value;
                Task.Factory.StartNew(() => {
                    Thread.Sleep(4000);
                    _val = ToggleValueType.ValueON;
                    ValueChanged?.Invoke(this, _val);
                });
            }

            public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
            {
                return true;
            }
        }

        public class ButtonTestAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "";
                }
                set
                {
                }
            }

            public bool IsSupportsEvent
            {
                get
                {
                    return true;
                }
            }

            public ValueTypeBase ValueType
            {
                get
                {
                    return new ButtonValueType();
                }

                set
                {
                }
            }

            public event ValueChangedDelegate ValueChanged;
            
            public string GetValue(Lazurite.ActionsDomain.ExecutionContext context)
            {
                return "";
            }

            public void Initialize()
            {
            }

            public void SetValue(Lazurite.ActionsDomain.ExecutionContext context, string value)
            {

            }

            public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
            {
                return true;
            }
        }

        public class FloatTestAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "";
                }

                set
                {
                }
            }

            public bool IsSupportsEvent
            {
                get
                {
                    return true;
                }
            }

            public ValueTypeBase ValueType
            {
                get
                {
                    return new FloatValueType()
                    {
                        AcceptedValues = new[] {
                            0.ToString(),
                            450.ToString()
                        }
                    };
                }

                set
                {
                }
            }

            public event ValueChangedDelegate ValueChanged;

            private string _val = 200.ToString();

            public string GetValue(Lazurite.ActionsDomain.ExecutionContext context)
            {
                return _val;
            }

            public void Initialize()
            {
            }

            public void SetValue(Lazurite.ActionsDomain.ExecutionContext context, string value)
            {
                _val = value;
                MessageBox.Show(_val);
                Task.Factory.StartNew(() => {
                    Thread.Sleep(4000);
                    _val = 340.ToString();
                    ValueChanged?.Invoke(this, _val);
                });
            }

            public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
            {
                return true;
            }
        }

        public class StatusTestAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "";
                }

                set
                {
                }
            }

            public bool IsSupportsEvent
            {
                get
                {
                    return true;
                }
            }

            public ValueTypeBase ValueType
            {
                get
                {
                    return new StateValueType()
                    {
                        AcceptedValues = new[] {
                            "Полный",
                            "Приглушен",
                            "Ночник",
                            "Отключен",
                            "Аварийный"
                        },
                    };
                }

                set
                {
                }
            }

            public event ValueChangedDelegate ValueChanged;

            private string _val = "Приглушен";

            public string GetValue(Lazurite.ActionsDomain.ExecutionContext context)
            {
                return _val;
            }

            public void Initialize()
            {
            }

            public void SetValue(Lazurite.ActionsDomain.ExecutionContext context, string value)
            {
                _val = value;
                Task.Factory.StartNew(() => {
                    Thread.Sleep(4000);
                    _val = "Аварийный";
                    ValueChanged?.Invoke(this, _val);
                });
            }

            public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
            {
                return true;
            }
        }

        public class DateTimeTestAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "";
                }

                set
                {
                }
            }

            public bool IsSupportsEvent
            {
                get
                {
                    return true;
                }
            }

            public ValueTypeBase ValueType
            {
                get
                {
                    return new DateTimeValueType();
                }

                set
                {
                }
            }

            public event ValueChangedDelegate ValueChanged;

            private string _val = DateTime.Now.ToString();

            public string GetValue(Lazurite.ActionsDomain.ExecutionContext context)
            {
                return _val;
            }

            public void Initialize()
            {
            }

            public void SetValue(Lazurite.ActionsDomain.ExecutionContext context, string value)
            {
                _val = value;
                Task.Factory.StartNew(() => {
                    Thread.Sleep(8000);
                    _val = DateTime.Now.ToString();
                    ValueChanged?.Invoke(this, _val);
                });
            }

            public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
            {
                return true;
            }
        }

        public class InfoTestAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "";
                }

                set
                {
                }
            }

            public bool IsSupportsEvent
            {
                get
                {
                    return true;
                }
            }

            public ValueTypeBase ValueType
            {
                get
                {
                    return new InfoValueType();
                }

                set
                {
                }
            }

            public event ValueChangedDelegate ValueChanged;

            private string _val = "+25";

            public string GetValue(Lazurite.ActionsDomain.ExecutionContext context)
            {
                return _val;
            }

            public void Initialize()
            {
            }

            public void SetValue(Lazurite.ActionsDomain.ExecutionContext context, string value)
            {
                _val = value;
                Task.Factory.StartNew(() => {
                    Thread.Sleep(4000);
                    _val = DateTime.Now.ToString();
                    ValueChanged?.Invoke(this, _val);
                });
            }

            public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
            {
                return true;
            }
        }
    }
}
