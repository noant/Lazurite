using MediaHost.Bases;
using MediaHost.VlcWrapper.Playlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaHost.Vlc
{
    /// <summary>
    /// Логика взаимодействия для VlcIptvHost.xaml
    /// </summary>
    public partial class VlcIptvHost : MediaPanelBase
    {
        private const string PlaylistPath = "VlcPlaylistMediaPath";
        private const string PlaylistUriPath = "VlcPlaylistMediaPath_Uri";
        private const string DefaultChannelPath = "DefaultChannel";
        private const string TvOffCommandTitle = "Выключено";
        private const string LoadPlaylistTitle = "Загрузить новый плейлист";
        private const string TvChannelCommandTitle = "ТВ канал";
        private const string NextChannelTitle = "Следующий канал";
        private const string PrevChannelTitle = "Предыдущий канал";
        private const string CancelPlaylistLoad = "Отменить загрузку плейлиста";
        private const string RefreshPlaylistTitle = "Обновить плейлист";
        private CancellationTokenSource _cancellationTS_LoadPlaylist;
        private string _currentPlaylist;

        public StatesMediaCommand TvChannels { get; private set; }

        public VlcIptvHost(string elementName) :
            base(elementName)
        {
            NotificationUtils.Initialize();
            InitializeComponent();

            vlcIptvControl.ChannelChanged += (o, e) =>
            {
                TvChannels?.RaiseValueChanged(e.Value?.Title ?? TvOffCommandTitle);
            };
        }

        private string GetDefaultChannel()
        {
            if (DataManager.TryLoad(DefaultChannelPath, out string defChannel))
                return defChannel;
            return null;
        }

        private void NewPlaylist(string path)
        {
            _cancellationTS_LoadPlaylist?.Cancel();
            _cancellationTS_LoadPlaylist = new CancellationTokenSource();
            _cancellationTS_LoadPlaylist.Token.Register(() =>
                NotificationUtils.ShowNotification("Загрузка плейлиста отменена...", NotificationUtils.InfoType.VlcLoading));

            NotificationUtils.ShowNotification("Загрузка нового плейлиста...", NotificationUtils.InfoType.VlcLoading);

            Task.Factory.StartNew(() =>
            {
                MediaPath[] newPlaylist = null;
                var data = PlaylistsHelper.FromPath(
                    path,
                    (p) => NotificationUtils.ShowNotification(p + " загрузка...", NotificationUtils.InfoType.VlcLoading),
                    _cancellationTS_LoadPlaylist.Token,
                    isPlaylist: true);
                if (data is Playlist pl)
                    newPlaylist = pl.Expand();
                else if (data != null)
                    newPlaylist = new[] { data };
                if (newPlaylist != null && !_cancellationTS_LoadPlaylist.IsCancellationRequested)
                {
                    LoadPlaylist(newPlaylist);
                    InitCommands();
                    DataManager.Save(PlaylistPath, newPlaylist);
                    DataManager.Save(PlaylistUriPath, path);
                    _currentPlaylist = path;
                    NotificationUtils.ShowNotification("Плейлист загружен", NotificationUtils.InfoType.ChannelOK);
                }
                else
                    NotificationUtils.ShowNotification("Плейлист не загружен", NotificationUtils.InfoType.Error);
                _cancellationTS_LoadPlaylist = null;
            });
        }

        protected override bool InitializeInternal()
        {
            if (!vlcIptvControl.Started)
                vlcIptvControl.Start();

            return base.InitializeInternal();
        }

        protected override void CloseInternal()
        {
            vlcIptvControl.Stop();

            base.CloseInternal();
        }

        private void LoadFromDataManager()
        {
            if (DataManager.TryLoad(PlaylistPath, out MediaPath[] playlist))
                LoadPlaylist(playlist);
            else
                NotificationUtils.ShowNotification("Список каналов отсутсвует", NotificationUtils.InfoType.Error);

            DataManager.TryLoad(PlaylistUriPath, out _currentPlaylist);

            InitCommands();
        }

        private void InitCommands()
        {
            var commands = new List<MediaCommandBase>();

            commands.Add(new SimpleMediaCommand((path) => NewPlaylist(path), LoadPlaylistTitle, () => _currentPlaylist));
            commands.Add(new SimpleMediaCommand(() => NewPlaylist(_currentPlaylist), RefreshPlaylistTitle));
            commands.Add(new SimpleMediaCommand(() => _cancellationTS_LoadPlaylist?.Cancel(), CancelPlaylistLoad));

            if (vlcIptvControl.Channels?.Any() ?? false)
            {
                commands.Add(new SimpleMediaCommand(() => vlcIptvControl.SetNextChannel(), NextChannelTitle));
                commands.Add(new SimpleMediaCommand(() => vlcIptvControl.SetPreviousChannel(), PrevChannelTitle));

                commands.Add(TvChannels = new StatesMediaCommand(
                    (c) =>
                    {
                        if (c == TvOffCommandTitle)
                            _needClose?.Invoke();
                        var channel = vlcIptvControl.Channels.FirstOrDefault(x => x.Title == c);
                        if (channel != null)
                            vlcIptvControl.SetChannel(channel);
                    },
                    () => new[] { TvOffCommandTitle }.Union(vlcIptvControl.Channels.Select(x => x.Title)).ToArray(),
                    () => vlcIptvControl.CurrentChannel?.Title ?? TvOffCommandTitle,
                    TvChannelCommandTitle,
                    (param) => param != TvOffCommandTitle
                ));
            }

            foreach (var oldCommand in Commands)
            {
                var newCommand = commands.FirstOrDefault(x => x.Name == oldCommand.Name);
                oldCommand.TransferEvents(newCommand);
                newCommand.RaiseEvents(); // Lazurite scenario can be reinitialized by call this method
            }

            Commands = commands.ToArray();
        }

        private void LoadPlaylist(MediaPath[] playlist)
        {
            var defaultChannelPath = GetDefaultChannel();
            var defaultChannel = playlist.FirstOrDefault(x => x.GetUri().OriginalString == defaultChannelPath);
            vlcIptvControl.Initialize(playlist, defaultChannel);
        }

        public override bool IsCompatibleWith(MediaPanelBase panel)
        {
            return !(panel is VlcIptvHost);
        }

        public override void CoreInitialize(Action needClose)
        {
            LoadFromDataManager();

            base.CoreInitialize(needClose);
        }
    }
}