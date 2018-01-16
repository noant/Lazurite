using Geolocation;
using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared;
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
    [HumanFriendlyName("Пользователь в локации")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    [LazuriteIcon(Icon.MapLocation)]
    public class UserInLocationAction : IAction, IUsersDataAccess
    {
        public string Caption
        {
            get => string.Format("Пользователь '{0}' в локации '{1}'", GetUserName(), PlaceName);
            set { }
        }

        public string UserId { get; set; }

        public string DeviceId { get; set; }

        public string PlaceName { get; set; }

        private string GetUserName()
        {
            var user = NeedUsers?.Invoke().FirstOrDefault(x => x.Id.Equals(UserId));
            if (user != null)
                return user.Name;
            else
                return "[неизвестный]";
        }

        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new ToggleValueType();

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => true;

        public Func<IGeolocationTarget[]> NeedUsers { get; set; }

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            var places = UserGeolocationPlugin.Utils.GetCurrentUserGeolocations(NeedUsers?.Invoke(), UserId, DeviceId);
            return places.Any(x => x.Name.Equals(this.PlaceName)) ? ToggleValueType.ValueON : ToggleValueType.ValueOFF;
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
            var window = new UserInLocationActionWindow();
            window.Users = NeedUsers?.Invoke();
            window.Refresh();
            window.SelectedUser = NeedUsers?.Invoke().FirstOrDefault(x => x.Id.Equals(this.UserId));
            window.SelectedDevice = this.DeviceId;
            window.SelectedPlace = PlacesManager.Current.Places.FirstOrDefault(x => x.Name.Equals(this.PlaceName));
            if (window.ShowDialog() ?? false)
            {
                this.DeviceId = window.SelectedDevice;
                this.UserId = window.SelectedUser.Id;
                this.PlaceName = window.SelectedPlace.Name;
                return true;
            }
            else return false;
        }
    }
}
