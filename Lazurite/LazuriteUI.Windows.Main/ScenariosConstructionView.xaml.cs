﻿using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для ScenariosConstructionView.xaml
    /// </summary>
    [LazuriteIcon(Icon.MovieClapperSelect)]
    [DisplayName("Конструктор сценариев")]
    public partial class ScenariosConstructionView : UserControl
    {
        public ScenariosConstructionView()
        {
            InitializeComponent();
            this.switchesGrid.SelectedModelChanged += SwitchesGrid_SelectedModelChanged;
            this.switchesGrid.Initialize();
        }

        private void SwitchesGrid_SelectedModelChanged(Switches.ScenarioModel obj)
        {
            this.constructorsResolver.SetScenario(this.switchesGrid.SelectedModel.Scenario);
        }
    }
}
