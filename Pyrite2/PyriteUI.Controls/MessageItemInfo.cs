using PyriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyriteUI.Controls
{
    public class MessageItemInfo
    {
        public MessageItemInfo(string text, Action<MessageView> click, Icon? icon = null)
        {
            Text = text;
            Click = click;
            Icon = icon;
        }

        public string Text { get; private set; }
        public Action<MessageView> Click { get; private set; }
        public Icon? Icon { get; private set; }
    }
}
