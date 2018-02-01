using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using System;
using System.Windows;

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
            foreach (Icon icon in Enum.GetValues(typeof(Icon)))
            {
                var iconCtrl = new IconView();
                iconCtrl.Icon = icon;
                sp.Children.Add(iconCtrl);
            }
        }
    }
}
