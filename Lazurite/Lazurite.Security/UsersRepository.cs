using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Security
{
    public class UsersRepository: UsersRepositoryBase
    {
        private ISavior _savior = Singleton.Resolve<ISavior>();
        private readonly string _usersKey = "users";
        private readonly string _groupsKey = "usersGroups";
        private UserBase _systemUser;

        public UsersRepository()
        {
            _systemUser = new SystemUser();
            if (_savior.Has(_usersKey))
                _users = _savior.Get<List<UserBase>>(_usersKey);
            if (_savior.Has(_groupsKey))
                _groups = _savior.Get<List<UserGroupBase>>(_groupsKey);
        }

        private void SaveUsersList()
        {
            _savior.Set(_usersKey, _users);
        }

        private void SaveGroupsList()
        {
            _savior.Set(_groupsKey, _groups);
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
            if (!group.Users.Any(x => x.Id.Equals(user)))
            {
                group.Users.Add(user);
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
                group.Users.RemoveAll(x => x.Id.Equals(user.Id));
            SaveUsersList();
            SaveGroupsList();

            RaiseUserRemoved(user);
        }

        public override void RemoveUserFromGroup(UserGroupBase group, UserBase user)
        {
            group = _groups.Single(x => x.Name.Equals(group.Name));
            group.Users.RemoveAll(x => x.Id.Equals(user.Id));
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
            foreach (var group in _groups.Where(x => x.Users.Any(z => z.Id.Equals(user.Id))))
            {
                group.Users.RemoveAll(x => x.Id.Equals(user.Id));
                group.Users.Add(user);
            }
            SaveUsersList();
            SaveGroupsList();
        }
    }
}
