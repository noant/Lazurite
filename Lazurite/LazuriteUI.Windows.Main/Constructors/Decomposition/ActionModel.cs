using Lazurite.ActionsDomain;
using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    public class ActionModel: ObservableObject
    {
        public ActionModel(IAction action)
        {
            Action = action;
            ActionName = Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(action.GetType());
            Icon = Icons.LazuriteIconAttribute.GetIcon(action.GetType());
            OnPropertyChanged(nameof(ActionName));
            OnPropertyChanged(nameof(ActionCaption));
            OnPropertyChanged(nameof(Icon));
        }

        private bool _editMode;

        public string ActionCaption
        {
            get
            {
                return Action.Caption;
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

        public Icon Icon { get; private set; }

        public IAction Action { get; private set; }
    }
}
