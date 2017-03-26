using Android.App;
using Android.Widget;
using Android.OS;
using Pyrite.Android.ServiceClient;
using System.Threading.Tasks;
using System;
using Pyrite.MainDomain.MessageSecurity;

namespace Pyrite.Android.Application
{
    [Activity(Label = "Pyrite.Android.Application", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Task.Delay(TimeSpan.FromSeconds(20));

            var client = ServiceClientManager.Create("desktop", 444, "PyriteService.svc", "secretKey1234567" , "anton", "123");
            client.GetScenariosInfoCompleted += Client_GetScenariosInfoCompleted;
            client.GetScenariosInfoAsync();
            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }

        private void Client_GetScenariosInfoCompleted(object sender, Andriod.ServiceClient.GetScenariosInfoCompletedEventArgs e)
        {
            try
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("!!!");
                var result = e.Result[0];
                var scenInfo = result.Decrypt("secretKey1234567");
                alert.SetMessage(scenInfo.ScenarioId + " " + scenInfo.CurrentValue + " " + scenInfo.ValueType.HumanFriendlyName);

                //FindViewById()

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            catch (Exception ee)
            {
                var a = ee;
            }
        }
    }
}

