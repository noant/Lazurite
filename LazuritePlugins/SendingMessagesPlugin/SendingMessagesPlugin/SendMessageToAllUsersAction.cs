using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using SendingMessagesPluginUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendingMessagesPlugin
{
    [OnlyExecute]
    [SuitableValueTypes(true)]
    [LazuriteIcon(Icon.Message)]
    [HumanFriendlyName("Cообщение всем пользователям")]
    [Category(Category.Other)]
    public class SendMessageToAllUsersAction : IAction, IMessagesSender
    {
        public string Title { get; set; }
        private Func<IMessageTarget[]> _needTargets;

        public string Caption {
            get => string.Empty;
            set { }
        }

        public ValueTypeBase ValueType { get; set; } = new InfoValueType();
        
        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => true;

        public event ValueChangedEventHandler ValueChanged;
        
        public string GetValue(ExecutionContext context) => string.Empty;

        public void Initialize()
        {
            //
        }

        public void SetNeedTargets(Func<IMessageTarget[]> needTargets) => _needTargets = needTargets;

        public void SetValue(ExecutionContext context, string value)
        {
            foreach (var user in _needTargets?.Invoke())
                user.SetMessage(string.IsNullOrEmpty(value) ? "[пусто]" : value, this.Title);
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            var users = _needTargets?.Invoke();
            var window = new SelectAllUsersAndTitleWindow();
            window.MessageTitle = Title;
            window.SetUsers(users);
            if (window.ShowDialog() ?? false)
            {
                this.Title = window.MessageTitle;
                return true;
            }
            return false;
        }
    }
}
