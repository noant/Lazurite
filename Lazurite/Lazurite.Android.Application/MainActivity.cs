using Android.App;
using Android.Widget;
using Android.OS;
using Lazurite.Android.ServiceClient;
using System.Threading.Tasks;
using System;
using Lazurite.MainDomain.MessageSecurity;

namespace Lazurite.Android.Application
{
    [Activity(Label = "Lazurite.Android.Application", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView view;
        LinearLayout linearLayout;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            
            var client = ServiceClientManager.Create("desktop", 444, "LazuriteService.svc", "secretKey1234567" , "anton", "123");
            client.GetScenariosInfoCompleted += Client_GetScenariosInfoCompleted;

            Task.Factory.StartNew(() => {
                while (true)
                {
                    client.GetScenariosInfoAsync();
                    Task.Delay(TimeSpan.FromSeconds(3)).Wait();
                }
            });
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            view = FindViewById<TextView>(Resource.Id.textView1);
            linearLayout = FindViewById<LinearLayout>(Resource.Id.main_layout);
        }
        
        private void Client_GetScenariosInfoCompleted(object sender, Andriod.ServiceClient.GetScenariosInfoCompletedEventArgs e)
        {
            try
            {
                var result = e.Result[0];
                var scenInfo = result.Decrypt("secretKey1234567");
                RunOnUiThread(() => {
                    view.Text = scenInfo.ScenarioId + " " + scenInfo.CurrentValue + " " + scenInfo.ValueType.HumanFriendlyName;
                });
            }
            catch (Exception ee)
            {
                var a = ee;
            }
        }
    }
}

