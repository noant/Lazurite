using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumePlugin
{
    public static class Utils
    {
        public static double GetVolumeLevel()
        {
            var defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
            return defaultPlaybackDevice?.Volume ?? 0;
        }

        public static void SetVolumeLevel(double value)
        {
            var defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
            if (defaultPlaybackDevice != null)
                defaultPlaybackDevice.Volume = value;
        }
    }
}
