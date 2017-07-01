using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Switches.SwitchSettings
{
    public class SwitchIconModel: ObservableObject
    {
        public SwitchIconModel(ScenarioModel scenarioModel, bool isSecondIcon)
        {
            ScenarioModel = scenarioModel;
            IsSecondIcon = isSecondIcon;
            
            if (ScenarioModel != null)
                ScenarioModel.PropertyChanged += (o, e) => {
                    if ((e.PropertyName == nameof(ScenarioModel.Icon1) && !IsSecondIcon) ||
                        (e.PropertyName == nameof(ScenarioModel.Icon2) && IsSecondIcon))
                    {
                        OnPropertyChanged(nameof(ItemSelected));
                    }
                };
            OnPropertyChanged(nameof(ItemSelected));
        }

        public ScenarioModel ScenarioModel { get; private set; }
        public bool IsSecondIcon { get; private set; }

        private string _icon;

        public string Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public bool ItemSelected
        {
            get
            {
                return _icon.Equals(IsSecondIcon ? ScenarioModel.Icon2 : ScenarioModel.Icon1);
            }
            set
            {
                if (value)
                {
                    if (IsSecondIcon)
                        ScenarioModel.Icon2 = _icon;
                    else ScenarioModel.Icon1 = _icon;
                    OnPropertyChanged(nameof(ItemSelected));
                }
            }
        }
    }
}
