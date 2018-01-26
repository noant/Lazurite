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
    [HumanFriendlyName("Пользователь в локации")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    [LazuriteIcon(Icon.MapLocation)]
    [Category(Category.Geolocation)]
    public class UserInLocationAction : IAction, IUsersGeolocationAccess
    {
        public string Caption
        {
            get => string.Format("Пользователь '{0}'; место '{1}'", GetUserName(), PlaceName);
            set { }
        }

        public string UserId { get; set; }

        public string DeviceId { get; set; }

        public string PlaceName { get; set; }

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
            get;
            set;
        } = new ToggleValueType();

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => true;

        private Func<IGeolocationTarget[]> _needUsers;

        public void SetNeedTargets(Func<IGeolocationTarget[]> needUsers) => _needUsers = needUsers;

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            var places = UserGeolocationPlugin.Utils.GetUserCurrentLocations(_needUsers?.Invoke(), UserId, DeviceId);
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
            window.Users = _needUsers?.Invoke();
            window.Refresh();
            window.SelectedUser = _needUsers?.Invoke().FirstOrDefault(x => x.Id.Equals(this.UserId));
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
