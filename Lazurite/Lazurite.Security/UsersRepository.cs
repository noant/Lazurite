using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lazurite.Security
{
    public class UsersRepository: UsersRepositoryBase
    {
        private static SaviorBase Savior = Singleton.Resolve<SaviorBase>();
        private static AddictionalDataManager Bus = Singleton.Resolve<AddictionalDataManager>();
        private readonly string _usersKey = "users";
        private readonly string _groupsKey = "usersGroups";
        private UserBase _systemUser = new SystemUser();

        public UsersRepository()
        {
            //do nothing
        }

        public override void Initialize()
        {
            Bus.Register<GeolocationDataHandler>();
            if (Savior.Has(_usersKey))
                _users = Savior.Get<List<UserBase>>(_usersKey);
            if (Savior.Has(_groupsKey))
                _groups = Savior.Get<List<UserGroupBase>>(_groupsKey);
        }

        private void SaveUsersList()
        {
            Savior.Set(_usersKey, _users);
        }

        private void SaveGroupsList()
        {
            Savior.Set(_groupsKey, _groups);
        }

        public override UserBase SystemUser {
            get {
                return _systemUser;
            }
        }

        public override void Add(UserGroupBase group)
        {
            if (_groups.Any(x => x.Name.Equals(group.Name)))
                throw new InvalidOperationException("Group with same name already exist");
            _groups.Add(group);
            SaveGroupsList();
        }

        public override void Add(UserBase user)
        {
            if (string.IsNullOrEmpty(user.Id))
                user.Id = Guid.NewGuid().ToString();
            if (_users.Any(x => x.Id.Equals(user.Id)))
                throw new InvalidOperationException("User with same id already exist");
            if (_users.Any(x => x.Login.Equals(user.Login)))
                throw new InvalidOperationException("User with same login already exist");
            _users.Add(user);
            SaveUsersList();
        }

        public override void AddUserToGroup(UserGroupBase group, UserBase user)
        {
            group = _groups.Single(x => x.Name.Equals(group.Name));
            if (!group.UsersIds.Any(x => x.Equals(user.Id)))
            {
                group.UsersIds.Add(user.Id);
                SaveGroupsList();
            }
        }

        public override void Remove(UserGroupBase group)
        {
            _groups.RemoveAll(x => x.Name.Equals(group.Name));
            SaveGroupsList();
        }

        public override void Remove(UserBase user)
        {
            _users.RemoveAll(x => x.Id.Equals(user.Id));
            foreach (var group in _groups)
                group.UsersIds.RemoveAll(x => x.Equals(user.Id));
            SaveUsersList();
            SaveGroupsList();

            RaiseUserRemoved(user);
        }

        public override void RemoveUserFromGroup(UserGroupBase group, UserBase user)
        {
            group = _groups.Single(x => x.Name.Equals(group.Name));
            group.UsersIds.RemoveAll(x => x.Equals(user.Id));
            SaveGroupsList();
        }
        
        public override void Save(UserGroupBase group)
        {
            _groups.RemoveAll(x => x.Name.Equals(group.Name));
            _groups.Add(group);
            SaveGroupsList();
        }

        public override void Save(UserBase user)
        {
            _users.RemoveAll(x => x.Id.Equals(user.Id));
            _users.Add(user);
            SaveUsersList();
        }
    }
}
