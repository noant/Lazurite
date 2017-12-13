using LazuriteUI.Icons;
using System;

namespace LazuriteUI.Windows.Main.PluginsViews
{
    public class PluginActionViewModel: ObservableObject
    {
        public PluginActionViewModel(Type type)
        {
            Text = Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(type);
            Icon = LazuriteUI.Icons.LazuriteIconAttribute.GetIcon(type);
            if (Icon == Icon._None)
                Icon = Icon.LayerUp;
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(Icon));
        }

        public Icon Icon { get; private set; }
        public string Text { get; private set; }
    }
}
