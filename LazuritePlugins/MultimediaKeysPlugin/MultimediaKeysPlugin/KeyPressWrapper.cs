using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MultimediaKeysPlugin
{
    public static class KeyPressWrapper
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        private const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        private const int KEYEVENTF_KEYUP = 0x0002; //Key up flag

        private static void Press(byte keyModifier, byte key)
        {
            keybd_event(keyModifier, 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(key, 0, KEYEVENTF_EXTENDEDKEY, 0);
        }
        private static void Release(byte keyModifier, byte key)
        {
            keybd_event(key, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(keyModifier, 0, KEYEVENTF_KEYUP, 0);
        }
        private static void Press(byte key)
        {
            keybd_event(key, 0, KEYEVENTF_EXTENDEDKEY, 0);
        }
        private static void Release(byte key)
        {
            keybd_event(key, 0, KEYEVENTF_KEYUP, 0);
        }

        public static void Release(Keys key)
        {
            Release((byte)key);
        }

        public static void Release(Keys modifier, Keys key)
        {
            Release((byte)modifier, (byte)key);
        }

        public static void Press(Keys key)
        {
            Press((byte)key);
        }

        public static void Press(Keys modifier, Keys key)
        {
            Press((byte)modifier, (byte)key);
        }
    }
}
