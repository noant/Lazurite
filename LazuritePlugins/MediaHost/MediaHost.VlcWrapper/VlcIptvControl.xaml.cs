using global::Vlc.DotNet.Core;
using global::Vlc.DotNet.Core.Interops.Signatures;
using Lazurite.Shared;
using MediaHost.VlcWrapper.Playlists;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using static MediaHost.VlcWrapper.Playlists.NotificationUtils;

namespace MediaHost.VlcWrapper
{
    /// <summary>
    /// Логика взаимодействия для VlcMain.xaml
    /// </summary>
    public partial class VlcIptvControl : UserControl, IDisposable
    {
        public static readonly IReadOnlyList<string> AspectRatios = new string[] {
            "5:4",
            "4:3",
            "16:10",
            "16:9"
        };

        private static Timer DelayTimer;

        public IReadOnlyCollection<MediaPath> Channels { get; private set; }

        private bool _nextVector = true;

        private string _currentPath;

        public VlcIptvControl()
        {
            InitializeComponent();

            vlc.Child.MouseEnter += Child_MouseEnter;
            vlc.Child.MouseLeave += Child_MouseLeave;

            Loaded += (o, e) => ShowNotification("Загрузка...", InfoType.VlcLoading);
        }

        public string AspectRatio
        {
            get => vlc.MediaPlayer.Video.AspectRatio;
            set => vlc.MediaPlayer.Video.AspectRatio = value;
        }

        public MediaPath CurrentChannel
        {
            get; private set;
        }

        public void SetNextChannel()
        {
            _nextVector = true;
            if (Channels.Any())
            {
                if (CurrentChannel == null)
                    SetChannelInternal(Channels.FirstOrDefault());
                else
                {
                    var channels = Channels.ToList();
                    var nextIndex = channels.IndexOf(CurrentChannel) + 1;
                    if (nextIndex >= channels.Count)
                        nextIndex = 0;
                    SetChannelInternal(channels[nextIndex]);
                }
            }
        }

        public void SetPreviousChannel()
        {
            _nextVector = false;
            if (Channels.Any())
            {
                if (CurrentChannel == null)
                    SetChannelInternal(Channels.LastOrDefault());
                else
                {
                    var channels = Channels.ToList();
                    var nextIndex = channels.IndexOf(CurrentChannel) - 1;
                    if (nextIndex <= 0)
                        nextIndex = channels.Count - 1;
                    SetChannelInternal(channels[nextIndex]);
                }
            }
        }

        private void SetChannelInternal(MediaPath channel)
        {
            CurrentChannel = channel;
            ChannelChanged?.Invoke(this, new EventsArgs<MediaPath>(CurrentChannel));
            if (CurrentChannel is Playlist pl)
            {
                if (pl.Children.Any())
                {
                    SetPlaylistVariant(0);
                }
                else
                {
                    ShowNotification(string.Format("Невозможно получить данные о {0}", CurrentChannel.Title), InfoType.Error);
                    LoadFailed();
                }
            }
            else if (CurrentChannel != null)
            {
                PlayInternal(CurrentChannel.GetUri().OriginalString);
                ShowNotification(string.Format("Загрузка {0} ...", CurrentChannel.Title), InfoType.Loading);
            }
            else Stop();
        }

        public void SetChannel(MediaPath channel)
        {
            _nextVector = true;
            SetChannelInternal(channel);
        }

        private void SetPlaylistVariant(int variant)
        {
            var pl = CurrentChannel as Playlist;
            PlayInternal(pl.Children[variant].GetUri().OriginalString);
            ShowNotification(string.Format("Загрузка {0} ...", pl.Title), InfoType.Loading);
        }

        private void PlayInternal(string path)
        {
            _currentPath = path;
            StartWithDelay(() =>
            {
                lock (_startStopLocker)
                {
                    if (vlc.MediaPlayer.IsPlaying)
                        vlc.MediaPlayer.Stop();
                    vlc.MediaPlayer.Play(path);
                }
            });
        }

        private void ShowNotification(string text, InfoType type)
        {
            NotificationUtils.ShowNotification(text, type);
        }

        private void LoadFailed()
        {
            if (CurrentChannel != null)
            {
                var channelTemp = CurrentChannel;
                bool loadNext = true;
                if (CurrentChannel is Playlist pl)
                {
                    var curInd = pl.Children.Select(x => x.GetUri().OriginalString).ToList().IndexOf(_currentPath);
                    var nextInd = curInd + 1;
                    if (nextInd < pl.Children.Length)
                    {
                        loadNext = false;
                        SetPlaylistVariant(nextInd);
                    }
                }
                if (loadNext)
                {
                    ShowNotification(string.Format("Ошибка {0}", CurrentChannel?.Title), InfoType.Error);
                    if (_nextVector)
                        SetNextChannel();
                    else
                        SetPreviousChannel();
                }
            }
        }

        private void Child_MouseLeave(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Show();
        }

        private void Child_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Hide();
        }

        public void Initialize(MediaPath[] channels, MediaPath defaultChannel = null)
        {
            if (Environment.Is64BitProcess)
                throw new Exception("Интеграция VLC работает только в x86 процессах");

            var vlcFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "VideoLAN", "VLC");
            if (!Directory.Exists(vlcFolder))
                throw new DirectoryNotFoundException(string.Format("Не найдена директория VLC [{0}]", vlcFolder));

            Channels = channels ?? throw new ArgumentNullException(nameof(channels));

            vlc.MediaPlayer.VlcLibDirectory = new DirectoryInfo(vlcFolder);

            vlc.MediaPlayer.EndInit();
            vlc.MediaPlayer.Playing += MediaPlayer_Playing;
            vlc.MediaPlayer.Stopped += MediaPlayer_Stopped;
            vlc.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;

            CurrentChannel = defaultChannel;
        }

        private void MediaPlayer_EncounteredError(object sender, VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            LoadFailed();
        }

        private void MediaPlayer_Playing(object sender, VlcMediaPlayerPlayingEventArgs e)
        {
            if (CurrentChannel != null)
                ShowNotification(CurrentChannel.Title, InfoType.ChannelOK);
        }

        private void MediaPlayer_Stopped(object sender, VlcMediaPlayerStoppedEventArgs e)
        {
            // Do nothing
        }

        private static void StartWithDelay(Action action)
        {
            DelayTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            DelayTimer?.Dispose();

            DelayTimer = new Timer(
                (s) =>
                {
                    action();
                    DelayTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                    DelayTimer?.Dispose();
                    DelayTimer = null;
                },
                null, 1000, Timeout.Infinite);
        }

        public void Dispose()
        {
            vlc.MediaPlayer.Stop();
            vlc.MediaPlayer.Dispose();
        }

        public void Start()
        {
            if (CurrentChannel == null && Channels != null)
                CurrentChannel = Channels.FirstOrDefault();

            // if any channel exists
            if (CurrentChannel != null)
                SetChannel(CurrentChannel);
        }

        public void Stop()
        {
            lock (_startStopLocker)
            {
                CurrentChannel = null;
                ChannelChanged?.Invoke(this, new EventsArgs<MediaPath>(CurrentChannel));
                vlc.MediaPlayer.Stop();
            }
        }

        public bool Started =>
            vlc.MediaPlayer.State == MediaStates.Playing;

        public event EventsHandler<MediaPath> ChannelChanged;

        public object _startStopLocker = new object();
    }
}