using Lazurite.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    [PermissionType(typeof(DenyForAllUsersExceptPermission))]
    public class DenyAllUsersExceptPermissionView : PermissionViewBase
    {
        public DenyAllUsersExceptPermissionView(DenyForAllUsersExceptPermission permission): base(permission)
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
            var permission = (DenyForAllUsersExceptPermission)Permission;
            UsersSelectView.Show(
                (users) => {
                    permission.UsersIds = users.Select(x=>x.Id).ToList();
                    OnModified();
                },
                permission.UsersIds.ToArray());
            base.OnSelectClick();
        }
    }
}
