﻿using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Data;
using Lazurite.Exceptions;
using Lazurite.IOC;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Modules
{
    public class PluginsManager : IDisposable
    {
        public static readonly string PluginFileExtension = ".pyp";

        public PluginsManager()
        {
            //resolve plugin types
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

            _baseDir = 
                Path.Combine(
                    Path.GetDirectoryName(
                        Utils.GetAssemblyPath(
                            Assembly.GetExecutingAssembly())),
                    "plugins");
            _tmpDir =
                Path.Combine(
                    Path.GetDirectoryName(
                        Utils.GetAssemblyPath(
                            Assembly.GetExecutingAssembly())),
                    "tmp_plugins");

            _exceptionsHandler.Handle(this, () => {
                if (!Directory.Exists(_baseDir))
                    Directory.CreateDirectory(_baseDir);
            });

            Debug.WriteLine("plugins base dir: " + _baseDir);

            //get all plugins
            _exceptionsHandler.Handle(this, () =>
            {
                if (_savior.Has(_saviorKey))
                    _plugins = _savior.Get<List<PluginInfo>>(_saviorKey);
            });

            //remove plugins
            if (_savior.Has(_saviorKey_removePlugins))
            {
                _exceptionsHandler.Handle(this, () =>
                {
                    _pluginsToRemove = _savior.Get<List<PluginInfo>>(_saviorKey_removePlugins);
                    foreach (var plugin in _pluginsToRemove.ToArray())
                    {
                        var pluginDir = Path.Combine(_baseDir, plugin.Name);
                        _exceptionsHandler.Handle(this, () => Directory.Delete(pluginDir, true));
                        _pluginsToRemove.Remove(plugin);
                    }
                    _savior.Set(_saviorKey_removePlugins, _pluginsToRemove);
                });
            }

            //updated plugins initilized by adding in app
            var updatedPlugins = new List<string>();

            //update plugins
            if (Directory.Exists(_tmpDir))
            {
                foreach (var tmpPluginFileToUpdate in Directory.GetFiles(_tmpDir))
                {
                    var pluginName = Path.GetFileNameWithoutExtension(tmpPluginFileToUpdate);
                    _exceptionsHandler.Handle(this, () =>
                    {
                        RemovePluginInternal(pluginName);
                        AddPlugin(tmpPluginFileToUpdate);
                        updatedPlugins.Add(pluginName);
                    });
                }
            }

            //init target types
            foreach (var plugin in _plugins)
            {
                if (!updatedPlugins.Any(x=> x.Equals(plugin.Name)))
                    foreach (var relativePath in plugin.TargetLibraries)
                    {
                        _exceptionsHandler.Handle(this, () =>
                        {
                            var absolutePath = Path.Combine(_baseDir, relativePath);
                            Debug.WriteLine("plugin " + plugin.Name + " lib path " + absolutePath);
                            var assembly = Utils.LoadAssembly(absolutePath);
                            var types = GetTargetTypes(assembly);
                            _allTypes.AddRange(types.Select(x => new PluginTypeInfo()
                            {
                                Plugin = plugin,
                                Type = x,
                                Assembly = assembly
                            }));
                        });
                    }
            }

            //clear temporary plugin directory
            _exceptionsHandler.Handle(this, () =>
            {
                if (Directory.Exists(_tmpDir))
                    Directory.Delete(_tmpDir, true);
                Directory.CreateDirectory(_tmpDir);
            });
        }
        
        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (!_catchedAssemblies.ContainsKey(args.LoadedAssembly.FullName))
                _catchedAssemblies.Add(args.LoadedAssembly.FullName, args.LoadedAssembly);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_catchedAssemblies.ContainsKey(args.Name))
                return _catchedAssemblies[args.Name];
            var pluginTypeInfo = _allTypes.FirstOrDefault(x => x.Assembly.FullName.Equals(args.Name));
            if (pluginTypeInfo != null)
                return pluginTypeInfo.Assembly;
            else throw new DllNotFoundException("Cannot resolve assembly: " + args.Name);
        }
        
        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.AssemblyLoad -= CurrentDomain_AssemblyLoad;
        }
        
        private Dictionary<string, Assembly> _catchedAssemblies = new Dictionary<string, Assembly>();
        private readonly string _saviorKey = "modulesManager";
        private readonly string _saviorKey_removePlugins = "modulesManager_removeLibs";
        private string _baseDir;
        private List<PluginTypeInfo> _allTypes = new List<PluginTypeInfo>();
        private List<PluginInfo> _plugins = new List<PluginInfo>();
        private List<PluginInfo> _pluginsToRemove = new List<PluginInfo>();
        private List<string> _pluginsToUpdate = new List<string>();
        private string _tmpDir;
        private ScenariosRepositoryBase _scenarioRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private ISavior _savior = Singleton.Resolve<ISavior>();
        private IExceptionsHandler _exceptionsHandler = Singleton.Resolve<IExceptionsHandler>();

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
            _exceptionsHandler.Handle(this, () =>
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
            });
        }

        private void RemovePluginInternal(string pluginName)
        {
            _plugins.RemoveAll(x => x.Name.Equals(pluginName));
            _savior.Set(_saviorKey, _plugins);
        }

        /// <summary>
        /// Append plugin
        /// </summary>
        /// <param name="pluginPath"></param>
        public void AddPlugin(string pluginPath)
        {
            _exceptionsHandler.Handle(this, () =>
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
                var plugin = new PluginInfo()
                {
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
            });
        }
        
        /// <summary>
        /// Determines where plugin can be removed
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public CanRemovePluginResult CanRemovePlugin(string pluginName)
        {
            return _exceptionsHandler.Handle(this, () =>
            {
                var libraryTypes = _allTypes.Where(x => x.Plugin.Name.Equals(pluginName)).Select(x => x.Type).ToArray();
                //determine dependent scenarios
                var dependentScenarios = _scenarioRepository.GetDependentScenarios(libraryTypes);
                var dependentTriggers = _scenarioRepository.GetDependentTriggers(libraryTypes);
                if (dependentScenarios.Any() || dependentTriggers.Any())
                {
                    var allDependentNames = dependentScenarios
                            .Select(x => x.Name)
                            .Union(dependentTriggers.Select(x => x.Name))
                            .Aggregate((x1, x2) => x1 + ";\r\n" + x2);

                    return new CanRemovePluginResult(false,
                        "Cannot remove plugin, because there is some scenarios or triggers referenced on it:\r\n" +
                        allDependentNames);
                }
                return new CanRemovePluginResult(true);
            });
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
            return _exceptionsHandler.Handle(this, () =>
            {
                var result = new CanUpdatePluginResult(true);
                var pluginName = Path.GetFileNameWithoutExtension(pluginPath);
                if (!_plugins.Any(x => x.Name.Equals(pluginName)))
                    result = new CanUpdatePluginResult(false, "Plugin not exist");
                var tmpPluginDir = Path.Combine(_tmpDir, pluginName);
                if (Directory.Exists(tmpPluginDir))
                    Directory.Delete(tmpPluginDir, true);
                Utils.UnpackFile(pluginPath, tmpPluginDir);
                var newPluginTypes =
                    Utils.GetAssembliesWithType(typeof(IAction), tmpPluginDir)
                    .SelectMany(x => x.Types).Select(x => x.Name).ToArray();
                var oldPluginTypes = _allTypes.Where(x => x.Plugin.Name.Equals(pluginName))
                    .Select(x => x.Type.Name).ToArray();
                if (oldPluginTypes.Intersect(newPluginTypes).Count() != oldPluginTypes.Count())
                    result = new CanUpdatePluginResult(false, "Some plugin types not exists in new plugin");
                Directory.Delete(tmpPluginDir, true);
                return result;
            });
        }

        /// <summary>
        /// Updates plugin
        /// </summary>
        /// <param name="pluginPath"></param>
        public void UpdatePlugin(string pluginPath)
        {
            _exceptionsHandler.Handle(this, () =>
            {
                var canUpdateResult = CanUpdatePlugin(pluginPath);
                if (canUpdateResult.CanUpdate)
                {
                    var pluginName = Path.GetFileNameWithoutExtension(pluginPath);
                    var tmpPluginFile = Path.Combine(_tmpDir, pluginName + PluginFileExtension);
                    if (File.Exists(tmpPluginFile))
                        File.Delete(tmpPluginFile);
                    File.Copy(pluginPath, tmpPluginFile);
                }
                else throw new Exception(canUpdateResult.Message);
            });
        }
    }
}