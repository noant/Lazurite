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

            var actionView = new ActionView(new ActionHolder() { Action = new ToggleTestAction() });
            this.grid.Children.Add(actionView);
        }

        [HumanFriendlyName("Modbus устройство")]
        [LazuriteIcon(Icons.Icon.Connect)]
        public class ToggleTestAction : IAction
        {
            public string Caption
            {
                get
                {
                    return "Контроллер: 1, устройство: 0";
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
                ValueChanged?.Invoke(this, _val);
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
                get;set;
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
                ValueChanged?.Invoke(this, _val);
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
                ValueChanged?.Invoke(this, _val);
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
                ValueChanged?.Invoke(this, _val);
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
                ValueChanged?.Invoke(this, _val);
            }

            public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
            {
                return true;
            }
        }
    }
}
