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
            IconVisibility = Icon == Icon.None ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(ActionName));
            OnPropertyChanged(nameof(ActionCaption));
            OnPropertyChanged(nameof(Icon));
        }

        private bool _editMode;

        public string ActionCaption
        {
            get
            {
                return ActionHolder?.Action.Caption ?? "[null]";
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

        public Visibility IconVisibility { get; private set; }

        public Icon Icon { get; private set; }

        public ActionHolder ActionHolder { get; private set; }
    }
}
