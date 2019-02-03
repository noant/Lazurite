using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using System.Collections.Generic;

namespace Lazurite.Visual
{
    public class VisualSettingsRepository
    {
        private DataManagerBase _dataManager = Singleton.Resolve<DataManagerBase>();
        private UsersRepositoryBase _usersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private ScenariosRepositoryBase _scenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private List<UserVisualSettings> _allSettings = new List<UserVisualSettings>();
        private readonly string _key = "visualLayouts";

        public VisualSettingsRepository()
        {
            if (_dataManager.Has(_key))
                _allSettings = _dataManager.Get<List<UserVisualSettings>>(_key);
            _usersRepository.OnUserRemoved = (user) => 
            {
                _allSettings.RemoveAll(x => x.UserId.Equals(user.Id));
                Save();
            };
            _scenariosRepository.OnScenarioRemoved += (sender, args) =>
            {
                _allSettings.RemoveAll(x => x.ScenarioId.Equals(args.Value.Id));
                Save();
            };
        }

        public void Save()
        {
            _dataManager.Set(_key, _allSettings);
        }

        public UserVisualSettings[] VisualSettings
        {
            get
            {
                return _allSettings.ToArray();
            }
        }

        public void Add(UserVisualSettings settings)
        {
            _allSettings.RemoveAll(x => x.SameAs(settings));
            _allSettings.Add(settings);
        }

        public void Remove(UserVisualSettings settings)
        {
            _allSettings.RemoveAll(x => x.SameAs(settings));
        }

        public void Update(UserVisualSettings settings)
        {
            Add(settings); //operations equals
        }
    }
}
