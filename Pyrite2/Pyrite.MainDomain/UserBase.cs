namespace Pyrite.MainDomain
{
    public class UserBase
    {
        public string Id { get; set; } //guid
        public string Name { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
    }
}