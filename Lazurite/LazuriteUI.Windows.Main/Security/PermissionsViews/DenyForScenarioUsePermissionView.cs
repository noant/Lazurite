using Lazurite.Security.Permissions;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    [PermissionType(typeof(DenyForScenarioUsePermission))]
    public class DenyForScenarioUsePermissionView : PermissionViewBase
    {
        public DenyForScenarioUsePermissionView(DenyForScenarioUsePermission permission): base(permission)
        {
            //do nothing
        }

        public override bool IsSelectButtonVisible
        {
            get
            {
                return false;
            }
        }
    }
}
