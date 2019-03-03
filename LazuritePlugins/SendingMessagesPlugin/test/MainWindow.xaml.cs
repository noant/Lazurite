using Lazurite.Shared;
using SendingMessagesPlugin;
using System.Windows;

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

            public Messages ExtractMessages()
            {
                return null;
            }

            public void SetMessage(string message, string title)
            {
            }
        }
    }
}