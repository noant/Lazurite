using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation
{
    /// <summary>
    /// Логика взаимодействия для StatisticsItemView.xaml
    /// </summary>
    public partial class StatisticsItemView : Grid
    {
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private ScenarioBase _scen;
        private bool _notExist;

        public StatisticsItemView()
        {
            InitializeComponent();
        }

        public void Refresh(StatisticsItem item, DateTime dateTime)
        {
            if (item == null)
                Visibility = Visibility.Collapsed;
            else
            {
                Visibility = Visibility.Visible;
                tbScenName.Text = item.Target.Name;
                if (item.Target.ValueTypeName == Lazurite.ActionsDomain.Utils.GetValueTypeClassName(typeof(ToggleValueType)))
                    tbScenVal.Text = item.Value == ToggleValueType.ValueON ? "Вкл." : "Выкл.";
                else
                    tbScenVal.Text = item.Value;
                var unit = string.Empty;
                if (_scen == null && !_notExist)
                {
                    _scen = ScenariosRepository.Scenarios.FirstOrDefault(x => x.Id == item.Target.ID);
                    if (_scen == null)
                        _notExist = true;
                }
                if (_scen != null && _scen.ValueType is FloatValueType floatValueType)
                    tbScenVal.Text += floatValueType.Unit;
                tbScenVal.Visibility = string.IsNullOrEmpty(tbScenVal.Text) ? Visibility.Collapsed : Visibility.Visible;
                tbUser.Text = item.Source?.Name;
                tbUser.Visibility = string.IsNullOrEmpty(tbUser.Text) || tbUser.Text == UsersRepository.SystemUser.Name ? Visibility.Collapsed : Visibility.Visible;
                tbDateTimeSetted.Text = "Выставлено: " + item.DateTime.ToString();
            }
        }
    }
}
