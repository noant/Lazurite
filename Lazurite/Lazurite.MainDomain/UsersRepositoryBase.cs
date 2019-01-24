using System;
using System.Collections.Generic;

namespace Lazurite.MainDomain
{
    public abstract class UsersRepositoryBase
    {
        protected List<UserBase> _users = new List<UserBase>();
        protected List<UserGroupBase> _groups = new List<UserGroupBase>();

        public Action<UserBase> OnUserRemoved { get; set; }

        public UserBase[] Users => _users.ToArray();

        public UserGroupBase[] Groups => _groups.ToArray();

        protected void RaiseUserRemoved(UserBase user) => OnUserRemoved?.Invoke(user);

        public abstract void Initialize();
        public abstract UserBase SystemUser { get; }
        public abstract void Add(UserBase user);
        public abstract void Add(UserGroupBase group);
        public abstract void Remove(UserBase user);
        public abstract void Remove(UserGroupBase group);
        public abstract void AddUserToGroup(UserGroupBase group, UserBase user);
        public abstract void RemoveUserFromGroup(UserGroupBase group, UserBase user);
        public abstract void Save(UserBase user);
        public abstract void Save(UserGroupBase group);
    }
}
