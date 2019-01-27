using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Lazurite.Shared;
using LazuriteMobile.MainDomain;

namespace LazuriteMobile.App.Droid
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2146:TypesMustBeAtLeastAsCriticalAsBaseTypesFxCopRule")]
    public class GeolocationViewIntentCreator : IGeolocationView
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void View(Geolocation geolocation, string label)
        {
            var uriBase = "geo:{0},{1}?q={0},{1}({2})";
            var intent = new Intent(Intent.ActionView);
            intent.AddFlags(ActivityFlags.NewTask);
            var uri = global::Android.Net.Uri.Parse(string.Format(
                    uriBase,
                    geolocation.Latitude.ToString().Replace(",", "."),
                    geolocation.Longtitude.ToString().Replace(",", "."),
                    label));
            intent.SetData(uri);
            var context = global::Android.App.Application.Context;
            if (intent.ResolveActivity(context.PackageManager) != null)
                context.StartActivity(intent);
        }
    }
}