using AVerCap;
using MediaHost.AverMedia.Wrapper;
using MediaHost.Bases;
using System;

namespace MediaHost.AverMedia
{
    /// <summary>
    /// Логика взаимодействия для AverMediaHost.xaml
    /// </summary>
    public partial class AverMediaHost : MediaPanelBase
    {
        private static readonly string[] DevicesIndexes = new[] { "0", "1", "3", "4", "5", "6", "7", "8", "9" };

        private LocalStreamer _streamer;
        private uint _deviceIndex = 0;
        private EnumObject<VIDEOSOURCE> _source = new EnumObject<VIDEOSOURCE>(VIDEOSOURCE.HDMI);
        private EnumObject<VIDEORESOLUTION> _resolution = new EnumObject<VIDEORESOLUTION>(VIDEORESOLUTION._1920X1080);
        private EnumObject<VIDEOFORMAT> _format = new EnumObject<VIDEOFORMAT>();

        private object _locker = new object();

        public AverMediaHost(string elementName)
            : base(elementName)
        {
            InitializeComponent();
            InitCommands();
        }

        private void InitCommands()
        {
            var deviceCommand = new StatesMediaCommand(
                (d) => ChangeDevice(d),
                () => DevicesIndexes,
                () => _deviceIndex.ToString(),
                "Индекс платы захвата",
                (p) => false);

            var sourceCommand = new StatesMediaCommand(
                (s) => ChangeSource(s),
                () => _source.Values,
                () => _source.SelectedValue,
                "Порт платы захвата",
                (p) => false);

            var formatCommand = new StatesMediaCommand(
                (f) => ChangeFormat(f),
                () => _format.Values,
                () => _format.SelectedValue,
                "Формат вывода (NTSC/PAL)",
                (p) => false);

            var resolutionCommand = new StatesMediaCommand(
                (r) => ChangeResolution(r),
                () => _resolution.Values,
                () => _resolution.SelectedValue,
                "Разрешение",
                (p) => false);

            Commands = new MediaCommandBase[] {
                deviceCommand,
                sourceCommand,
                resolutionCommand,
                formatCommand
            };
        }

        private void ChangeDevice(string device)
        {
            if (uint.TryParse(device, out uint deviceIndex))
            {
                _deviceIndex = deviceIndex;
                if (ElementInitialized)
                    InitStreaming();
            }
        }

        private void ChangeSource(string source)
        {
            _source.SelectedValue = source;
            if (ElementInitialized)
                InitStreaming();
        }

        private void ChangeFormat(string format)
        {
            _format.SelectedValue = format;
            if (ElementInitialized)
                InitStreaming();
        }

        private void ChangeResolution(string resolution)
        {
            _resolution.SelectedValue = resolution;
            if (ElementInitialized)
                InitStreaming();
        }

        protected override bool InitializeInternal()
        {
            InitStreaming();
            return base.InitializeInternal();
        }

        private void InitStreaming()
        {
            lock (_locker)
                try
                {
                    _streamer?.StopStreaming();
                    wfControl.BeginInvoke(new Action(() =>
                    {
                        var size = TransformToPixels();
                        _streamer = new LocalStreamer(_deviceIndex, wfControl.Handle, _source.SelectedEnum, CAPTURETYPE.CAPTURETYPE_ALL, (uint)size.Width, (uint)size.Height, _resolution.SelectedEnum, _format.SelectedEnum);
                        _streamer.StartStreaming();
                    }));
                }
                catch
                {
                    throw;
                }
        }

        public override bool IsCompatibleWith(MediaPanelBase panel)
        {
            if (panel is AverMediaHost p)
                return p._deviceIndex != _deviceIndex;
            return true;
        }

        protected override void CloseInternal()
        {
            lock (_locker)
            {
                _streamer?.StopStreaming();
                _streamer = null;
                base.CloseInternal();
            }
        }
    }
}
