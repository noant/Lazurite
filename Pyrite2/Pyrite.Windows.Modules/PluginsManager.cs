using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.Data;
using Pyrite.IOC;
using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Modules
{
    public class PluginsManager
    {
        public PluginsManager()
        {
            //get executing assembly path
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            _baseDir = Path.Combine(Path.GetDirectoryName(path), "plugins");
            if (!Directory.Exists(_baseDir))
                Directory.CreateDirectory(_baseDir);

            //get savior
            _savior = Singleton.Resolve<ISavior>();

            //get scenarios repository
            //_scenarioRepository = Singleton.Resolve<ScenariosRepositoryBase>();

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
                    Directory.Delete(pluginDir);
                    _pluginsToRemove.Remove(plugin);
                }
                _savior.Set(_saviorKey_removePlugins, _pluginsToRemove);
            }

            //init target types
            foreach (var plugin in _plugins)
            {
                foreach (var assemblyPath in plugin.TargetLibraries.Select(x=>Path.Combine(_baseDir, x))
                {
                    var assembly = Assembly.LoadFrom(assemblyPath);
                    var types = GetTargetTypes(assembly);
                    _allTypes.AddRange(types.Select(x=>new PluginTypeInfo() {
                        Plugin = plugin,
                        Type = x
                    }));
                }
            }
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
            return (IAction)type.GetConstructor(new Type[0]).Invoke(new object[0]);
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
        /// Get all libraries array
        /// </summary>
        /// <returns></returns>
        public PluginInfo[] GetAllLibraries()
        {
            return _plugins.ToArray();
        }

        /// <summary>
        /// Remove plugin
        /// </summary>
        /// <param name="library"></param>
        public void RemovePlugin(string pluginName)
        {
            var plugin = _plugins.Single(x => x.Name.Equals(pluginName));
            var canRemoveResult = CanRemoveLibrary(pluginName);
            if (!canRemoveResult.CanRemove)
                throw new InvalidOperationException(canRemoveResult.Message);
            _allTypes.RemoveAll(x => x.Plugin.Equals(plugin));
            _pluginsToRemove.Add(plugin);
            _plugins.Remove(plugin);
            _savior.Set(_saviorKey, _plugins);
            _savior.Set(_saviorKey_removePlugins, _pluginsToRemove);
        }

        /// <summary>
        /// Add IAction types of library or just add dependent library to root path
        /// </summary>
        /// <param name="libraryPath"></param>
        public void AddLibrary(string libraryPath, out bool loadedFileIsNotLibrary)
        {
            var fileName = Path.GetFileName(libraryPath);
            var destFilePath = Path.Combine(_baseDir, fileName);
            try
            {
                var canAddLibResult = CanAddLibrary(libraryPath);
                if (!canAddLibResult.CanAdd)
                {
                    //if (canAddLibResult.Message.Equals("Cannot add library. File already exists."))
                    //    throw new ModuleAlreadyExistsException(canAddLibResult.Message);
                    //else throw new Exception(canAddLibResult.Message);
                }
                File.Copy(libraryPath, destFilePath);
                _libraries.Add(fileName);
                _allTypes.AddRange(GetTargetTypes(Assembly.LoadFrom(destFilePath)));
                _savior.Set(_saviorKey, _libraries);
                loadedFileIsNotLibrary = false;
            }
            //catch (ModuleAlreadyExistsException e)
            //{ 
            //    throw e;
            //}
            catch (BadImageFormatException e)
            {
                loadedFileIsNotLibrary = true;
                //do nothing, just add file
            }
            catch (Exception e) {
                if (File.Exists(destFilePath))
                    File.Delete(destFilePath);
                throw e;
            }
        }
        
        /// <summary>
        /// Determines where library can be removed
        /// </summary>
        /// <param name="library"></param>
        /// <returns></returns>
        public CanRemovePluginResult CanRemoveLibrary(string library)
        {
            var destFilePath = Path.Combine(_baseDir, library);
            try
            {
                var assembly = Assembly.LoadFrom(destFilePath);
                var libraryTypes = GetTargetTypes(assembly);
                // var dependentScenarios = _scenarioRepository.GetDependentScenarios(libraryTypes);
                var dependentScenarios = _scenarioRepository != null ? _scenarioRepository.GetDependentScenarios(libraryTypes) : new ScenarioBase[0];
                //determines dependent scenarios
                if (dependentScenarios.Any())
                {
                    var allDependentScenariosNames = dependentScenarios
                            .Select(x => x.Name)
                            .Aggregate((x1, x2) => x1 + ";\r\n" + x2);
                    return new CanRemovePluginResult(false,
                        "Cannot remove library, because there is some scenarios referenced on it:\r\n" +
                        allDependentScenariosNames);
                }
                else
                {
                    //determines referenced libraries
                    var referencedLibraries = _libraries
                        .Select(x => Assembly.LoadFrom(Path.Combine(_baseDir, x)))
                        .Where(x => x.GetReferencedAssemblies()
                        .Any(z => z.FullName.Equals(assembly.FullName))).ToArray();
                    if (referencedLibraries.Any())
                    {
                        var allReferencedLibrariesNames = referencedLibraries
                            .Select(x => x.FullName)
                            .Aggregate((x1, x2) => x1 + ";\r\n" + x2);
                        return new CanRemovePluginResult(false,
                            "Cannot remove library, because there is some libraries referenced on it:\r\n" +
                            allReferencedLibrariesNames);
                    }
                }
                return new CanRemovePluginResult(true);
            }
            catch (BadImageFormatException e)
            {
                //then is not just dll or dll is corrupted
                return new CanRemovePluginResult(true);
            }
        }

        public CanAddPluginResult CanAddLibrary(string libraryPath)
        {
            var fileName = Path.GetFileName(libraryPath);
            var destFilePath = Path.Combine(_baseDir, fileName);
            if (_libraries.Contains(fileName) || File.Exists(destFilePath))
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
    }
}
