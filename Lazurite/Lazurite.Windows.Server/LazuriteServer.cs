using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Service;
using Lazurite.Shared;
using Lazurite.Windows.Logging;
using SimpleRemoteMethods.ServerSide;
using SimpleRemoteMethods.Utils.Windows;
using System;

namespace Lazurite.Windows.Server
{
    public class LazuriteServer
    {
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly int MaxConcurrentCalls = GlobalSettings.Get(100);
        public static readonly string SettingsKey = "serverSettings";

        private readonly DataEncryptorBase _dataEncryptor = Singleton.Resolve<DataEncryptorBase>();
        private DataManagerBase _dataManager = Singleton.Resolve<DataManagerBase>();
        private WarningHandlerBase _warningHandler = Singleton.Resolve<WarningHandlerBase>();
        private ServerSettings _settings;
        private Server<IServer> _server;

        public bool Started => _server?.Started ?? false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventsHandler<LazuriteServer> StatusChanged;

        public ServerSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                _dataManager.Set(SettingsKey, value);
            }
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Stop();
                StatusChanged?.Invoke(this, new EventsArgs<LazuriteServer>(this));
            }
        }

        public void StartAsync(Action<bool> callback)
        {
            try
            {
                _warningHandler.Info("Service starting: " + _settings.GetAddress());

                _server = new Server<IServer>(new LazuriteService(), true, _settings.Port, _settings.SecretKey)
                {
                    MaxConcurrentCalls = (ushort)MaxConcurrentCalls,
                    MaxRequestLength = 300000,
                    AuthenticationValidator = new LoginValidator()
                };

                _server.AfterServerStopped += (o, e) => StatusChanged?.Invoke(this, new EventsArgs<LazuriteServer>(this));
            
                _server.LogRecord += _server_LogRecord;

                ServerHelper.PrepareHttpsServer(_server, _settings.CertificateHash);

                _server.StartAsync();

                callback?.Invoke(true);
            }
            catch (Exception exception)
            {
                _warningHandler.Error("Ошибка запуска сервиса: " + _settings.GetAddress(), exception);
                callback?.Invoke(false);
                StatusChanged?.Invoke(this, new EventsArgs<LazuriteServer>(this));
            }
        }

        private void _server_LogRecord(object sender, LogRecordEventArgs e)
        {
            switch (e.Type)
            {
                case LogType.Debug: _warningHandler.Debug(e.Message, e.Exception); break;
                case LogType.Error: _warningHandler.Error(e.Message, e.Exception); break;
                case LogType.Info: _warningHandler.Info(e.Message, e.Exception); break;
            }
        }

        public void Restart(Action<bool> callback)
        {
            Stop();
            StartAsync(callback);
        }

        public LazuriteServer()
        {
            try
            {
                if (_dataManager.Has(SettingsKey))
                    _settings = _dataManager.Get<ServerSettings>(SettingsKey);
                else
                    _settings = new ServerSettings();
            }
            catch (Exception e)
            {
                _warningHandler.Error("Невозможно загрузить файл настроект сервера. Будут использоваться настройки поумолчанию. " +
                    "Возможно это связано с тем, что ранее файл был зашифрован а сейчас ключ шифрования файлов неизвестен " +
                    "(что может быть связано с переносом или восстановлением файлов настроек). " +
                    "В данном случае может помоч задание старого ключа шифрования файлов в настроках сервера.", e);
                _settings = new ServerSettings();
            }

            _dataEncryptor.SecretKeyChanged += DataEncryptor_SecretKeyChanged;
        }

        private void DataEncryptor_SecretKeyChanged(object sender, EventsArgs<DataEncryptorBase> args)
        {
            // Re-save settings with new secret key
            _dataManager.Set(SettingsKey, Settings);
        }
    }
}
