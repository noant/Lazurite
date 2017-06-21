using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.PluginsViews
{
    public class PluginActionViewModel: ObservableObject
    {
        public PluginActionViewModel(Type type)
        {
            Text = Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(type);
            Icon = LazuriteUI.Icons.LazuriteIconAttribute.GetIcon(type);
            if (Icon == Icon.None)
                Icon = Icon.LayerUp;
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(Icon));
        }

        public Icon Icon { get; private set; }
        public string Text { get; private set; }
    }
}
