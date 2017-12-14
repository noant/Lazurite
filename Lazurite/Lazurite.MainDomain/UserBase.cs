namespace Lazurite.MainDomain
{
    public class UserBase
    {
        private UserInfo _info;
        public string Id { get; set; } //guid
        public string Name { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; }

        public override string ToString() => string.Format("{0} ({1})", Name, Login);

        public UserInfo UserInfo => _info ?? (_info = new UserInfo(this));

        public void UpdateLocation(GeolocationInfo geolocationInfo) => UserInfo.AddGeolocationIfNotLast(geolocationInfo);
    }
}