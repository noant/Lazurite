using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.Data;
using Pyrite.IOC;
using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Modules
{
    public class PluginsManager : IDisposable
    {
        public PluginsManager()
        {
            _baseDir = 
                Path.Combine(
                    Path.GetDirectoryName(
                        Utils.GetAssemblyPath(
                            Assembly.GetExecutingAssembly())),
                    "plugins");
            if (!Directory.Exists(_baseDir))
                Directory.CreateDirectory(_baseDir);

            Debug.WriteLine("plugins base dir: " + _baseDir);

            //get savior
            _savior = Singleton.Resolve<ISavior>();

            //get scenarios repository
            _scenarioRepository = Singleton.Resolve<ScenariosRepositoryBase>();

            //get all plugins
            if (_savior.Has(_saviorKey))
                _plugins = _savior.Get<List<PluginInfo>>(_saviorKey);

            //remove plugins
            if (_savior.Has(_saviorKey_removePlugins))
            {
                _pluginsToRemove = _savior.Get<List<PluginInfo>>(_saviorKey_removePlugins);
                foreach (var plugin in _pluginsToRemove.ToArray())
                {
                    var pluginDir = Path.Combine(_baseDir, plugin.Name);
                    Directory.Delete(pluginDir, true);
                    _pluginsToRemove.Remove(plugin);
                }
                _savior.Set(_saviorKey_removePlugins, _pluginsToRemove);
            }

            //init target types
            foreach (var plugin in _plugins)
            {
                foreach (var relativePath in plugin.TargetLibraries)
                {
                    var absolutePath = Path.Combine(_baseDir, relativePath);
                    Debug.WriteLine("plugin " + plugin.Name + " lib path " + absolutePath);
                    var assembly = Utils.LoadAssembly(absolutePath);
                    var types = GetTargetTypes(assembly);
                    _allTypes.AddRange(types.Select(x => new PluginTypeInfo() {
                        Plugin = plugin,
                        Type = x,
                        Assembly = assembly
                    }));
                }
            }

            //resolve plugin types
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var pluginTypeInfo = _allTypes.FirstOrDefault(x => x.Assembly.FullName.Equals(args.Name));
            if (pluginTypeInfo != null)
                return pluginTypeInfo.Assembly;
            else throw new DllNotFoundException("Cannot find type " + args.Name);
        }
        
        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        private readonly string _saviorKey = "modulesManager";
        private readonly string _saviorKey_removePlugins = "modulesManager_removeLibs";
        private string _baseDir;
        private ScenariosRepositoryBase _scenarioRepository;
        private ISavior _savior;
        private List<PluginTypeInfo> _allTypes = new List<PluginTypeInfo>();
        private List<PluginInfo> _plugins = new List<PluginInfo>();
        private List<PluginInfo> _pluginsToRemove = new List<PluginInfo>();

        public IAction CreateInstanceOf(Type type)
        {
            return (IAction)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Get all types in module libraries
        /// </summary>
        /// <returns></returns>
        public Type[] GetModules(Type valueType = null, ActionInstanceSide side = ActionInstanceSide.Both) {
            var result = new List<Type>();
            bool isValueTypeSupport = false;
            bool rightSide = false;
            foreach (var typeInfo in _allTypes)
            {
                var type = typeInfo.Type;
                isValueTypeSupport = valueType == null || ActionsDomain.Utils.IsComparableWithValueType(type, valueType);
                rightSide = (side == ActionInstanceSide.Both) ||
                    (side == ActionInstanceSide.OnlyLeft) ||
                    (side == ActionInstanceSide.OnlyRight 
                    && ActionsDomain.Utils.IsOnlyGetValue(type)
                    && !ActionsDomain.Utils.IsOnlyExecute(type));

                if (rightSide && isValueTypeSupport)
                    result.Add(type);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get all libraries as array
        /// </summary>
        /// <returns></returns>
        public PluginInfo[] GetPlugins()
        {
            return _plugins.ToArray();
        }

        /// <summary>
        /// Remove plugin
        /// </summary>
        /// <param name="Unique name of plugin"></param>
        public void RemovePlugin(string pluginName)
        {
            var plugin = _plugins.Single(x => x.Name.Equals(pluginName));
            var canRemoveResult = CanRemovePlugin(pluginName);
            if (!canRemoveResult.CanRemove)
                throw new InvalidOperationException(canRemoveResult.Message);
            _allTypes.RemoveAll(x => x.Plugin.Equals(plugin));
            _pluginsToRemove.Add(plugin);
            _plugins.Remove(plugin);
            _savior.Set(_saviorKey, _plugins);
            _savior.Set(_saviorKey_removePlugins, _pluginsToRemove);
        }

        /// <summary>
        /// Append plugin
        /// </summary>
        /// <param name="pluginPath"></param>
        public void AddPlugin(string pluginPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(pluginPath);
            var destDirectoryPath = Path.Combine(_baseDir, fileName);
            var canAddLibResult = CanAddPlugin(pluginPath);
            if (!canAddLibResult.CanAdd)
                throw new Exception(canAddLibResult.Message);
            if (!Directory.Exists(destDirectoryPath))
                Directory.CreateDirectory(destDirectoryPath);
            Utils.UnpackFile(pluginPath, destDirectoryPath);
            Debug.WriteLine("plugin " + fileName + " unpacked in: " + destDirectoryPath);
            var assembliesAndTypes = Utils.GetAssembliesWithType(typeof(IAction), destDirectoryPath);
            var plugin = new PluginInfo() {
                Name = fileName,
                TargetLibraries = assembliesAndTypes
                    .Select(x =>
                        {
                            var relativePath = Utils.GetAssemblyPath(x.Assembly).Substring(_baseDir.Length + 1);
                            Debug.WriteLine("plugin " + fileName + " has target dll with relative path: " + relativePath);
                            return relativePath;
                        })
                        .ToArray()
            };
            _plugins.Add(plugin);
            _allTypes.AddRange(
                assembliesAndTypes.SelectMany(x => 
                    x.Types.Select(z =>
                        new PluginTypeInfo()
                        {
                            Plugin = plugin,
                            Type = z,
                            Assembly = x.Assembly
                        })
                )
            );
            _savior.Set(_saviorKey, _plugins);
        }
        
        /// <summary>
        /// Determines where plugin can be removed
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public CanRemovePluginResult CanRemovePlugin(string pluginName)
        {
            var libraryTypes = _allTypes.Where(x => x.Plugin.Name.Equals(pluginName)).Select(x=>x.Type).ToArray();
            //determine dependent scenarios
            var dependentScenarios = _scenarioRepository.GetDependentScenarios(libraryTypes);
            var dependentTriggers = _scenarioRepository.GetDependentTriggers(libraryTypes);
            if (dependentScenarios.Any() || dependentTriggers.Any())
            {
                var allDependentNames = dependentScenarios
                        .Select(x => x.Name)
                        .Union(dependentTriggers.Select(x=>x.Name))
                        .Aggregate((x1, x2) => x1 + ";\r\n" + x2);

                return new CanRemovePluginResult(false,
                    "Cannot remove plugin, because there is some scenarios or triggers referenced on it:\r\n" +
                    allDependentNames);
            }
            return new CanRemovePluginResult(true);
        }

        /// <summary>
        /// Determines where plugin can be added
        /// </summary>
        /// <returns></returns>
        public CanAddPluginResult CanAddPlugin(string pluginPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(pluginPath);
            if (_plugins.Any(x=>x.Name.Equals(fileName)))
                return new CanAddPluginResult(false, "Cannot add library. File already exists.");
            else return new CanAddPluginResult(true);
        }

        /// <summary>
        /// Get types defined in assembly and derived from IAction
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private Type[] GetTargetTypes(Assembly assembly)
        {
            return assembly.DefinedTypes.Where(x => typeof(IAction).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToArray();
        }

        /// <summary>
        /// Determines where plugin can be updated
        /// </summary>
        /// <param name="pluginPath">Plugin file path</param>
        /// <returns></returns>
        public CanUpdatePluginResult CanUpdatePlugin(string pluginPath)
        {
            
        }

        /// <summary>
        /// Updates plugin
        /// </summary>
        /// <param name="pluginPath"></param>
        public void UpdatePlugin(string pluginPath)
        {

        }
    }
}
