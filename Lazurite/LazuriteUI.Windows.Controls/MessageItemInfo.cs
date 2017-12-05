using LazuriteUI.Icons;
using System;

namespace LazuriteUI.Windows.Controls
{
    public class MessageItemInfo
    {
        public MessageItemInfo(string text, Action<MessageView> click, Icon? icon = null, bool focused = false)
        {
            Text = text;
            Click = click;
            Icon = icon;
            Focused = focused;
        }

        public string Text { get; private set; }
        public Action<MessageView> Click { get; private set; }
        public Icon? Icon { get; private set; }
        public bool Focused { get; private set; }
    }
}
