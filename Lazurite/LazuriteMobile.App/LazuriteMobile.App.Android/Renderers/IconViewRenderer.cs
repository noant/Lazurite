using Xamarin.Forms.Platform.Android;
using Android.Widget;
using System.ComponentModel;
using Xamarin.Forms;
using IconApp.Droid.Renderers;
using Android.Graphics;
using LazuriteMobile.App.Controls;
using Android.Graphics.Drawables;
using System;

[assembly: ExportRendererAttribute(typeof(IconView), typeof(IconViewRenderer))]

namespace IconApp.Droid.Renderers
{
    // Thanks to https://github.com/andreinitescu
    public class IconViewRenderer : ViewRenderer<IconView, ImageView>
    {
        private bool _isDisposed;

#pragma warning disable CS0618 // Тип или член устарел
        public IconViewRenderer()
        {
            AutoPackage = false;
        }
#pragma warning restore CS0618 // Тип или член устарел

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<IconView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                SetNativeControl(new ImageView(Context));
            }
            UpdateBitmap(e.OldElement);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == IconView.SourceProperty.PropertyName)
            {
                UpdateBitmap(null);
            }
            else if (e.PropertyName == IconView.ForegroundProperty.PropertyName)
            {
                UpdateBitmap(null);
            }
        }

        private void UpdateBitmap(IconView previous = null)
        {
            if (!_isDisposed && Element.Source != null)
            {
                if (Element.Foreground != Xamarin.Forms.Color.Transparent)
                {
                    var d = Drawable.CreateFromStream(Element.Source, "").Mutate();
                    d.SetColorFilter(new LightingColorFilter(Element.Foreground.ToAndroid(), Element.Foreground.ToAndroid()));
                    d.Alpha = Element.Foreground.ToAndroid().A;
                    Control.SetImageDrawable(d);
                }
                else
                {
                    var d = Drawable.CreateFromStream(Element.Source, "");
                    Control.SetImageDrawable(d);
                }
                ((IVisualElementController)Element).NativeSizeChanged();
            }
        }
    }
}

