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
    [HumanFriendlyName("Cообщение пользователю")]
    [Category(Category.Other)]
    public class SendMessageToUserAction : IAction, IMessagesSender
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        private Func<IMessageTarget[]> _needTargets;

        public string Caption {
            get => GetUserName();
            set { }
        }

        public ValueTypeBase ValueType { get; set; } = new InfoValueType();
        
        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => true;

        public event ValueChangedEventHandler ValueChanged;

        private IMessageTarget GetUser()
        {
            return _needTargets?.Invoke().FirstOrDefault(x => x.Id == this.UserId);
        }

        private string GetUserName() => GetUser()?.Name ?? "[не выбран]";

        public string GetValue(ExecutionContext context) => string.Empty;

        public void Initialize()
        {
            //
        }

        public void SetNeedTargets(Func<IMessageTarget[]> needTargets) => _needTargets = needTargets;

        public void SetValue(ExecutionContext context, string value)
        {
            GetUser()?.SetMessage(string.IsNullOrEmpty(value) ? "[пусто]" : value, this.Title);
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            var users = _needTargets?.Invoke();

            var window = new SelectUserAndTitleWindow();
            window.SetUsers(users);
            window.SelectedUsers = users.Where(x => x.Id == this.UserId).ToArray();
            window.MessageTitle = Title;
            if (window.ShowDialog() ?? false)
            {
                this.UserId = window.SelectedUsers.FirstOrDefault()?.Id;
                this.Title = window.MessageTitle;
                return true;
            }
            return false;
        }
    }
}
