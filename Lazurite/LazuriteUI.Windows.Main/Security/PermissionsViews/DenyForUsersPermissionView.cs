using Lazurite.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    [PermissionType(typeof(DenyForUsersPermission))]
    public class DenyForUsersPermissionView : PermissionViewBase
    {
        public DenyForUsersPermissionView(DenyForUsersPermission permission): base(permission)
        {
            //do nothing
        }

        public override bool IsSelectButtonVisible
        {
            get
            {
                return true;
            }
        }

        public override void OnSelectClick()
        {
            var permission = (DenyForUsersPermission)Permission;
            UsersSelectView.Show(
                (users) => {
                    permission.Users = users.ToList();
                    OnModified();
                },
                permission.Users.ToArray());
            base.OnSelectClick();
        }
    }
}
