using Xamarin.Forms;

namespace LazuriteMobile.App
{
    public static class Helper
    {
        public static Grid GetLastParent(Element view)
        {
            Grid grid = null;
            while (view != null)
            {
                view = view.Parent;
                if (view is Grid)
                    grid = (Grid)view;
            }
            return grid;
        }
    }
}
