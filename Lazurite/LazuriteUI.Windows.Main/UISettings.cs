﻿using Lazurite.Data;
using Lazurite.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main
{
    public class UISettings
    {
        private static readonly IDataManager DataManager = Singleton.Resolve<SaviorBase>();

        private static UISettings Settings;
        
        public static UISettings Current
        {
            get
            {
                if (Settings == null)
                {
                    if (DataManager.Has(nameof(UISettings)))
                        Settings = DataManager.Get<UISettings>(nameof(UISettings));
                    else
                        Settings = new UISettings();
                }
                return Settings;
            }
        }

        public static void Save()
        {
            DataManager.Set(nameof(UISettings), Settings);
        }

        public bool MouseRightSideHoverEvent { get; set; } = true;
    }
}
