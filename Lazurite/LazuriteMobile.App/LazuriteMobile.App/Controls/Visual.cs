using Lazurite.Data;
using Lazurite.IOC;
using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public static class Visual
    {
        private static readonly string SkinNameKey = "skinName";
        public static SkinBase Current { get; private set; } = new Skin_Lazurite();

        static Visual()
        {
            var fileManager = Singleton.Resolve<DataManagerBase>();

            if (fileManager.Has(SkinNameKey))
            {
                var skinName = fileManager.Get<string>(SkinNameKey);
                var skinBaseType = typeof(SkinBase);

                var types = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => x != skinBaseType && skinBaseType.IsAssignableFrom(x));

                var skinType = types.FirstOrDefault(x => x.FullName == skinName);

                if (skinType != null)
                    Current = Activator.CreateInstance(skinType) as SkinBase;
            }
        }

        public static void ApplySkin(SkinBase skin)
        {
            Current = skin;

            var fileManager = Singleton.Resolve<DataManagerBase>();
            fileManager.Set(SkinNameKey, skin.GetType().FullName);

            // Reinit main page
            var mainPage = new MainPage();
            Application.Current.MainPage = mainPage;
            mainPage.InitializeManager();
        }
    }
}