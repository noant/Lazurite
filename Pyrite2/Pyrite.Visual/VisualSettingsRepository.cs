using Pyrite.Data;
using Pyrite.IOC;
using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Visual
{
    public class VisualSettingsRepository
    {
        private ISavior _savior = Singleton.Resolve<ISavior>();
        private UsersRepositoryBase _usersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private ScenariosRepositoryBase _scenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private List<VisualSettingsBase> _allSettings = new List<VisualSettingsBase>();
        private readonly string _key = "visualLayouts";

        public VisualSettingsRepository()
        {
            if (_savior.Has(_key))
                _allSettings = _savior.Get<List<VisualSettingsBase>>(_key);
            _usersRepository.OnUserRemoved = (user) => 
            {
                _allSettings.RemoveAll(x => x is UserVisualSettings && ((UserVisualSettings)x).UserId.Equals(user.Id));
                Save();
            };
            _scenariosRepository.OnScenarioRemoved = (scenario) =>
            {
                _allSettings.RemoveAll(x => x.ScenarioId.Equals(scenario.Id));
                Save();
            };
        }

        private void Save()
        {
            _savior.Set(_key, _allSettings);
        }

        public VisualSettingsBase[] VisualSettings
        {
            get
            {
                return _allSettings.ToArray();
            }
        }

        public void Add(VisualSettingsBase settings)
        {
            _allSettings.RemoveAll(x => x.SameAs(settings));
            _allSettings.Add(settings);
            Save();
        }

        public void Remove(VisualSettingsBase settings)
        {
            _allSettings.RemoveAll(x => x.SameAs(settings));
            Save();
        }
    }
}
