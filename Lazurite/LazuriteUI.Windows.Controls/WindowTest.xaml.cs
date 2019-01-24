using LazuriteUI.Icons;
using System;
using System.Linq;
using System.Windows;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для WindowTest.xaml
    /// </summary>
    public partial class WindowTest : Window
    {
        public WindowTest()
        {
            InitializeComponent();
            var objs = new[] {
                new TestClass("test1", Icons.Icon.AdobeDreamweaver),
                new TestClass("test13"),
                new TestClass("test144"),
                new TestClass("test155", Icons.Icon.AlignJustify),
                new TestClass("test16666", Icons.Icon.AdobeDreamweaver),
                new TestClass("test17777"),
                new TestClass("test18888", Icons.Icon.AdobeFireworks),
                new TestClass("test18888 test18888 test18888 test18888 test18888 test18888 test18888 test18888 test18888 test18888 test18888 test18888 test18888 ", Icons.Icon.AdobePhotoshop),
            };
            combo.Info =
                new ComboItemsViewInfo(ListViewItemsSelectionMode.Multiple,
                objs,
                (o) => (o as TestClass).Name,
                (o) => (o as TestClass).Icon,
                objs.Take(2).ToArray(),
                "TESTTT",
                grid);
        }

        private class TestClass
        {
            public TestClass(string name, Icon? icon = null)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Icon = icon ?? Icons.Icon._None;
            }

            public string Name { get; set; }
            public Icon Icon { get; set; }
        }
    }
}
