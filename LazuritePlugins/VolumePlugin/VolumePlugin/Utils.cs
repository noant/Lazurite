using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Linq;

namespace VolumePlugin
{
    public static class Utils
    {
        private static CoreAudioController CoreAudioController = new CoreAudioController();
        private static CoreAudioDevice CoreAudioDevice;

        static Utils()
        {
            CoreAudioDevice = CoreAudioController.DefaultPlaybackDevice;
            CoreAudioController.AudioDeviceChanged.Subscribe(new Observer<DeviceChangedArgs>((args) =>
            {
                switch (args.ChangedType)
                {
                    case DeviceChangedType.DefaultChanged:
                    case DeviceChangedType.DeviceAdded:
                    case DeviceChangedType.DeviceRemoved:
                        {
                            CoreAudioDevice = CoreAudioController.DefaultPlaybackDevice;
                            break;
                        }
                }
            }));
        }

        public static double GetVolumeLevel()
        {
            if (CoreAudioDevice != null)
                return (CoreAudioDevice?.Volume ?? 0);
            else return 0;
        }

        public static void SetVolumeLevel(double value)
        {
            if (CoreAudioDevice != null)
            {
                if (value > 0 && CoreAudioDevice.IsMuted)
                    CoreAudioDevice.Mute(false);
                CoreAudioDevice.Volume = value;
            }
        }

        public static void SetOutputAudioDevice(int index)
        {
            var devices = CoreAudioController.GetPlaybackDevices();
            if (devices.Count() <= index)
                index = devices.Count() - 1;
            if (index > -1)
            {
                var device = devices.ElementAt(index);
                CoreAudioController.DefaultPlaybackDevice = device;
            }
        }

        public static int GetDefaultOutputDeviceIndex()
        {
            var devices = CoreAudioController.GetPlaybackDevices().ToList();
            return devices.IndexOf(CoreAudioDevice);
        }

        public static string GetDefaultOutputDeviceName() =>
            CoreAudioDevice.FullName;

        public static string[] GetDevices() => 
            CoreAudioController.GetPlaybackDevices().Select(x => x.FullName).ToArray();

        public static void SetPlaybackDevice(string name)
        {
            var devices = CoreAudioController.GetPlaybackDevices();
            var device = devices.FirstOrDefault(x => x.FullName == name);
            if (device != null)
                CoreAudioController.DefaultPlaybackDevice = device;
        }

        private class Observer<T> : IObserver<T>
        {
            public Observer(Action<T> callback)
            {
                Callback = callback;
            }

            public Action<T> Callback { get; private set; }

            public void OnCompleted()
            {
                //do nothing
            }

            public void OnError(Exception error)
            {
                //do nothing
            }

            public void OnNext(T value)
            {
                Callback?.Invoke(value);
            }
        }
    }
}
