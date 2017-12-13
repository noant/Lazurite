namespace LazuriteUI.Windows.Main
{
    public static class Utils
    {
        public static void RestartApp()
        {
            System.Diagnostics.Process.Start(Lazurite.Windows.Utils.Utils.GetAssemblyPath(typeof(App).Assembly));
            App.Current.Shutdown();
        }
    }
}