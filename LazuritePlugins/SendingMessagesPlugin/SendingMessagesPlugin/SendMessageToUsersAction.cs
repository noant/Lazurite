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
    [HumanFriendlyName("Cообщение нескольким пользователям")]
    [Category(Category.Other)]
    public class SendMessageToUsersAction : IAction, IMessagesSender
    {
        public string[] UsersIds { get; set; }
        public string Title { get; set; }
        private Func<IMessageTarget[]> _needTargets;

        public string Caption {
            get => GetUsersNamesShort();
            set { }
        }

        public ValueTypeBase ValueType { get; set; } = new InfoValueType();
        
        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => true;

        public event ValueChangedEventHandler ValueChanged;

        private IMessageTarget[] GetUsers()
        {
            if (UsersIds == null)
                return new IMessageTarget[0];
            return _needTargets?.Invoke().Where(x => UsersIds.Any(z => z == x.Id)).ToArray();
        }

        private string GetUsersNames()
        {
            var users = GetUsers();
            return users.Any() ?
                users.Select(x => x.Name).Aggregate((x1, x2) => string.Format("{0}, {1}", x1, x2))
                : string.Empty;                
        }
        
        private string GetUsersNamesShort()
        {
            var fullStr = GetUsersNames();
            if (fullStr.Length >= 50)
                return fullStr.Substring(0, 48) + "...";
            else if (string.IsNullOrEmpty(fullStr))
                return "[не выбраны]";
            return fullStr;
        }

        public string GetValue(ExecutionContext context) => string.Empty;

        public void Initialize()
        {
            //
        }

        public void SetNeedTargets(Func<IMessageTarget[]> needTargets) => _needTargets = needTargets;

        public void SetValue(ExecutionContext context, string value)
        {
            foreach (var user in GetUsers())
                user.SetMessage(string.IsNullOrEmpty(value) ? "[пусто]" : value, this.Title);
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            var users = _needTargets?.Invoke();
            var window = new SelectUsersAndTitleWindow();
            window.SetUsers(users);
            window.SelectedUsers = GetUsers();
            window.MessageTitle = Title;
            if (window.ShowDialog() ?? false)
            {
                this.UsersIds = window.SelectedUsers.Select(x => x.Id).ToArray();
                this.Title = window.MessageTitle;
                return true;
            }
            return false;
        }
    }
}
