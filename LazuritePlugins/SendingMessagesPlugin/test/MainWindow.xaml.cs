using Lazurite.Shared;
using SendingMessagesPlugin;
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

namespace test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var users = new User[] {
                new User()
                {
                    Id = "1",
                    Name = "Вася"
                },
                new User()
                {
                    Id = "2",
                    Name = "Петя"
                },
                new User()
                {
                    Id = "3",
                    Name = "Вова"
                },
            };

            var action = new SendMessageToUserAction();
            action.SetNeedTargets(() => users);
            while (true)
            {
                action.UserInitializeWith(null, true);
            }
        }

        public class User : IMessageTarget
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public Message[] ExtractMessages()
            {
                return null;
            }

            public void SetMessage(string message, string title)
            {
            }
        }
    }
}