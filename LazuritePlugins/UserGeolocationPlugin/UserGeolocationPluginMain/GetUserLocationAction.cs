using Geolocation;
using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserGeolocationPlugin;
using UserGeolocationPluginUI;

namespace UserGeolocationPluginMain
{
    [OnlyGetValue]
    [HumanFriendlyName("Текущая локация пользователя")]
    [SuitableValueTypes(typeof(StateValueType))]
    [LazuriteIcon(Icon.MapLocation)]
    [Category(Category.Geolocation)]
    public class GetUserLocationAction : IAction, IUsersGeolocationAccess
    {
        public string Caption
        {
            get => string.Format("'{0}'", GetUserName());
            set { }
        }

        public string UserId { get; set; }

        public string DeviceId { get; set; }

        private string GetUserName()
        {
            var user = _needUsers?.Invoke().FirstOrDefault(x => x.Id.Equals(UserId));
            if (user != null)
                return user.Name;
            else
                return "[неизвестный]";
        }

        public ValueTypeBase ValueType
        {
            get
            {
                return new StateValueType()
                {
                    AcceptedValues = PlacesManager.GetAllAvailablePlaces()
                        .Select(x => x.Name).ToArray()
                };
            }
            set
            {
                //do nothing 
            }
        }

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => true;

        private Func<IGeolocationTarget[]> _needUsers;

        public void SetNeedTargets(Func<IGeolocationTarget[]> needUsers) => _needUsers = needUsers;

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            return UserGeolocationPlugin.Utils.GetCurrentUserLocation(_needUsers?.Invoke(), UserId, DeviceId).Name;
        }

        public void Initialize()
        {
            //
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            var window = new GetUserLocationActionWindow();
            var users = _needUsers?.Invoke();
            window.Users = users;
            window.Refresh();
            window.SelectedUser = users.FirstOrDefault(x => x.Id.Equals(this.UserId));
            window.SelectedDevice = this.DeviceId;
            if (window.ShowDialog() ?? false)
            {
                this.DeviceId = window.SelectedDevice;
                this.UserId = window.SelectedUser.Id;
                return true;
            }
            else return false;
        }
    }
}
