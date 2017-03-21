using Android.App;
using Android.Widget;
using Android.OS;
using Pyrite.Android.ServiceClient;
using System.Threading.Tasks;
using System;

namespace Pyrite.Android.Application
{
    [Activity(Label = "Pyrite.Android.Application", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Task.Delay(TimeSpan.FromSeconds(10));

            var client = ServiceClientManager.Create("desktop", 444, "PyriteService.svc", "anton", "123");
            client.GetScenariosInfoCompleted += Client_GetScenariosInfoCompleted;
            client.GetScenariosInfoAsync();
            
            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }

        private void Client_GetScenariosInfoCompleted(object sender, GetScenariosInfoCompletedEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("!!!");
            alert.SetMessage(e.Result[0].ScenarioId);

            Dialog dialog = alert.Create();
            dialog.Show();
        }
    }
}

