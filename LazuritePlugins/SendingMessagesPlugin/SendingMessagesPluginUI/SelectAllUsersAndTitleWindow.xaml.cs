﻿using Lazurite.Shared;
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

namespace SendingMessagesPluginUI
{
    /// <summary>
    /// Логика взаимодействия для SelectUserAndTitleWindow.xaml
    /// </summary>
    public partial class SelectAllUsersAndTitleWindow : Window
    {
        public SelectAllUsersAndTitleWindow()
        {
            InitializeComponent();
            btApply.Click += (o, e) => DialogResult = true;
        }

        public void SetUsers(IMessageTarget[] users) => usersAndTitle.SetUsers(users);

        public string MessageTitle
        {
            get => usersAndTitle.Title;
            set => usersAndTitle.Title = value;
        }
    }
}
