using AudioSwitcher.AudioApi.CoreAudio;
using Lazurite.ActionsDomain.Attributes;
using LazuriteUI.Icons;
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
            return (defaultPlaybackDevice?.Volume ?? 0);
        }

        public static void SetVolumeLevel(double value)
        {
            var defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
            if (defaultPlaybackDevice != null)
                defaultPlaybackDevice.Volume = ((double)value);
        }

        public static void SetOutputAudioDevice(int index)
        {
            var coreAudioController = new CoreAudioController();
            var devices = coreAudioController.GetPlaybackDevices();
            if (devices.Count() <= index)
                index = devices.Count() - 1;
            devices.ElementAt(index).SetAsDefault();
        }

        public static int GetDefaultOutputDeviceIndex()
        {
            var coreAudioController = new CoreAudioController();
            var devices = coreAudioController.GetPlaybackDevices().ToList();
            return devices.IndexOf(coreAudioController.DefaultPlaybackDevice);
        }
    }
}
