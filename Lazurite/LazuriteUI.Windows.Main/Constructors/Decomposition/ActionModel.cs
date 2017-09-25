using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    public class ActionModel: ObservableObject
    {
        public void Refresh(ActionHolder actionHolder)
        {
            ActionHolder = actionHolder;
            Refresh();
        }
        
        public void Refresh()
        {
            ActionName = Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(ActionHolder.Action.GetType());
            Icon = Icons.LazuriteIconAttribute.GetIcon(ActionHolder.Action.GetType());
            ActionIconVisibility = Icon == Icon._None ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(IsSupportsModification));
            OnPropertyChanged(nameof(ActionName));
            OnPropertyChanged(nameof(ActionCaption));
            OnPropertyChanged(nameof(ActionIconVisibility));
            OnPropertyChanged(nameof(Icon));
        }

        private bool _editMode;

        public string ActionCaption
        {
            get
            {
                return ActionHolder?.Action.Caption ?? string.Empty;
            }
        }

        public string ActionName
        {
            get; private set;
        }

        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
                OnPropertyChanged(nameof(EditMode));
            }
        }

        public bool IsSupportsModification
        {
            get
            {
                return ActionHolder.Action.IsSupportsModification;
            }
        }

        public Visibility ActionIconVisibility { get; private set; }

        public Icon Icon { get; private set; }

        public ActionHolder ActionHolder { get; private set; }
    }
}
