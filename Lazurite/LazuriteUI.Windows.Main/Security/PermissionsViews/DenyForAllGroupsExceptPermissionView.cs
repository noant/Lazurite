using Lazurite.Security.Permissions;
using System.Linq;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    [PermissionType(typeof(DenyForAllGroupsExceptPermission))]
    public class DenyForAllGroupsExceptPermissionView : PermissionViewBase
    {
        public DenyForAllGroupsExceptPermissionView(DenyForAllGroupsExceptPermission permission): base(permission)
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
            var permission = (DenyForAllGroupsExceptPermission)Permission;
            GroupsSelectView.Show(
                (groups) => {
                    permission.GroupsIds = groups.Select(x=>x.Name).ToList();
                    OnModified();
                },
                permission.GroupsIds.ToArray());
            base.OnSelectClick();
        }
    }
}
