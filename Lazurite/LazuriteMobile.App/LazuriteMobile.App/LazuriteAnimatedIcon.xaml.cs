using Lazurite.Utils;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LazuriteAnimatedIcon : Grid
    {
        private SafeCancellationToken _stopAnimate = new SafeCancellationToken();

        public LazuriteAnimatedIcon()
        {
            _stopAnimate.Cancel(); // Crutch

            InitializeComponent();
        }

        public async void StartAnimate()
        {
            if (_stopAnimate.IsCancellationRequested)
            {
                _stopAnimate = new SafeCancellationToken();

                //while (!_stopAnimate.IsCancellationRequested)
                //{
                //    await icon2.FadeTo(1, 1000, Easing.SpringOut);
                //    await icon2.FadeTo(0, 1000, Easing.SpringOut);
                //} // Код работает плохо на ранних версиях Android

                await icon2.FadeTo(1, 1000, Easing.SpringOut);
                await icon2.FadeTo(0, 1000, Easing.SpringOut);

                TaskUtils.Start(() =>
                {
                    while (!_stopAnimate.IsCancellationRequested)
                    {
                        icon2.FadeTo(0, 1000, Easing.SpringOut).Wait();
                        icon2.FadeTo(1, 1000, Easing.SpringOut).Wait();
                    }
                });
            }
        }

        public void StopAnimate()
        {
            if (!_stopAnimate.IsCancellationRequested)
            {
                _stopAnimate.Cancel();
            }
        }
    }
}