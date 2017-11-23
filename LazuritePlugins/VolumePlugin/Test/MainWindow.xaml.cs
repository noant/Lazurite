﻿using System;
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
using VolumePlugin;

namespace Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Utils.SetOutputAudioDevice(3);

            tb.TextChanged += (o, e) => {
                try
                {
                    var deviceNum = int.Parse(tb.Text);
                    Utils.SetOutputAudioDevice(deviceNum);
                    tb1.Text = Utils.GetDefaultOutputDeviceIndex().ToString();
                }
                catch { }
            };

            slider.Value = Utils.GetVolumeLevel();

            slider.ValueChanged += (o, e) => {
                Utils.SetVolumeLevel(e.NewValue);
            };

        }
    }
}