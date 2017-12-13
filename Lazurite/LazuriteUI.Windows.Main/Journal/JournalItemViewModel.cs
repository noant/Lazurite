using Lazurite.Windows.Logging;
using System;

namespace LazuriteUI.Windows.Main.Journal
{
    public class JournalItemViewModel: ObservableObject
    {
        public JournalItemViewModel(string message, WarnType type)
        {
            Message = message;
            WarnType = type;
            Time = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
        }

        public string Message { get; private set; }
        public WarnType WarnType { get; private set; }
        public string Time { get; private set; }
    }
}
