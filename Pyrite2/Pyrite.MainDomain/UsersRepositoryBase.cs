using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public abstract class UsersRepositoryBase
    {
        protected List<UserBase> _users = new List<UserBase>();
        protected List<UserGroupBase> _groups = new List<UserGroupBase>();

        public Action<UserBase> OnUserRemoved { get; set; }

        public UserBase[] Users
        {
            get
            {
                return _users.ToArray();
            }
        }

        public UserGroupBase[] Groups
        {
            get
            {
                return _groups.ToArray();
            }
        }

        protected void RaiseUserRemoved(UserBase user)
        {
            OnUserRemoved?.Invoke(user);
        }

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
