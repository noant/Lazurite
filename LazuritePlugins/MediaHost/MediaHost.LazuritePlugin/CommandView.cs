using MediaHost.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHost.LazuritePlugin
{
    public class CommandView
    {
        public CommandView(MediaPanelBase panel, MediaCommandBase command = null)
        {
            IsToggle = command == null;
            Panel = panel;
            Command = command;
        }

        public MediaPanelBase Panel { get; private set; }
        public bool IsToggle { get; private set; }
        public MediaCommandBase Command { get; private set; }

        public override bool Equals(object obj)
        {
            var view = obj as CommandView;
            return view != null &&
                   EqualityComparer<MediaPanelBase>.Default.Equals(Panel, view.Panel) &&
                   IsToggle == view.IsToggle &&
                   EqualityComparer<MediaCommandBase>.Default.Equals(Command, view.Command);
        }

        public override int GetHashCode()
        {
            var hashCode = 1195689860;
            hashCode = hashCode * -1521134295 + EqualityComparer<MediaPanelBase>.Default.GetHashCode(Panel);
            hashCode = hashCode * -1521134295 + IsToggle.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<MediaCommandBase>.Default.GetHashCode(Command);
            return hashCode;
        }

        public override string ToString()
        {
            if (IsToggle)
                return Panel.ElementName + " (включить/выключить)";
            return Panel.ElementName + "/" + Command.Name;
        }
    }
}
