using Lazurite.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    [PermissionType(typeof(DenyForGroupsPermission))]
    public class DenyForGroupsPermissionView : PermissionViewBase
    {
        public DenyForGroupsPermissionView(DenyForGroupsPermission permission): base(permission)
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
            var permission = (DenyForGroupsPermission)Permission;
            GroupsSelectView.Show(
                (groups) => {
                    permission.GroupsIds = groups.Select(x => x.Name).ToList();
                    OnModified();
                },
                permission.GroupsIds.ToArray());
            base.OnSelectClick();
        }
        
    }
}
