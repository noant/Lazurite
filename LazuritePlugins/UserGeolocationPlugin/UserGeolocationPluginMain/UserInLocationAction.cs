using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using System;
using System.Linq;
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
            get => $"Пользователь '{GetUserName()}'; место '{PlaceName}'";
            set { }
        }

        public string UserId { get; set; }

        public string DeviceId { get; set; }

        public string PlaceName { get; set; }

        private string GetUserName()
        {
            var user = _needUsers?.Invoke().FirstOrDefault(x => x.Id.Equals(UserId));
            return user?.Name ?? "[неизвестный]";
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
            window.SelectedUser = _needUsers?.Invoke().FirstOrDefault(x => x.Id.Equals(UserId));
            window.SelectedDevice = DeviceId;
            window.SelectedPlace = PlacesManager.Current.Places.FirstOrDefault(x => x.Name.Equals(PlaceName));
            if (window.ShowDialog() ?? false)
            {
                DeviceId = window.SelectedDevice;
                UserId = window.SelectedUser.Id;
                PlaceName = window.SelectedPlace.Name;
                return true;
            }
            else return false;
        }
    }
}
