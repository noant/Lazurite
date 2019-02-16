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
    [HumanFriendlyName("Расстояние между пользователем и выбранным местом")]
    [SuitableValueTypes(typeof(FloatValueType))]
    [LazuriteIcon(Icon.MapLocation)]
    [Category(Category.Geolocation)]
    public class GetDistanceBetweenUserAndPlace : IAction, IUsersGeolocationAccess
    {
        public string Caption
        {
            get => $"Пользователь '{GetUserName()}'; место '{PlaceName}'; метров";
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
            get; set;
        } = new FloatValueType() {
            AcceptedValues = new[] { 0.0.ToString(), UserGeolocationPlugin.Utils.EquatorLength.ToString() },
            Unit = "м"
        };

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => true;

        private Func<IGeolocationTarget[]> _needUsers;

        public void SetNeedTargets(Func<IGeolocationTarget[]> needUsers) => _needUsers = needUsers;

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            return UserGeolocationPlugin.Utils.GetDistanceBetween(_needUsers?.Invoke(), UserId, DeviceId, PlaceName).ToString();
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
            var window = new GetDistanceBetweenUserAndLocationActionWindow();
            var users = _needUsers?.Invoke();
            window.Users = users;
            window.Refresh();
            window.SelectedUser = users.FirstOrDefault(x => x.Id.Equals(this.UserId));
            window.SelectedDevice = this.DeviceId;
            window.SelectedPlace = PlacesManager.Current.Places.FirstOrDefault(x => x.Name.Equals(this.PlaceName));
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
