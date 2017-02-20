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
    public class ModulesManager
    {
        public ModulesManager()
        {
            //get executing assembly path
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            _baseDir = Path.GetDirectoryName(path);

            //get savior
            _savior = Singleton.Resolve<ISavior>();

            //get scenarios repository
            //_scenarioRepository = Singleton.Resolve<ScenariosRepositoryBase>();

            //get all libraries
            if (_savior.Has(_saviorKey))
                _libraries = _savior.Get<List<string>>(_saviorKey);

            //get all libraries to remove
            if (_savior.Has(_saviorKey_removeLibs))
            {
                _librariesToRemove = _savior.Get<List<string>>(_saviorKey_removeLibs);                
                foreach (var libToRemove in _librariesToRemove)
                {
                    var libPath = Path.Combine(_baseDir, libToRemove);
                    if (File.Exists(libPath))
                        File.Delete(libPath);
                }
                _savior.Set(_saviorKey_removeLibs, new List<string>());
            }

            //get all target types
            _allTypes = _libraries
                .Select(x =>
                {
                    try
                    {
                        return Assembly.LoadFrom(Path.Combine(_baseDir, x));
                    }
                    catch (BadImageFormatException) //if dll is broken or file is not dll
                    {
                        //do nothing
                    }
                    return null;
                })
                .Select(x => x !=null ? GetTargetTypes(x) : new Type[0]).SelectMany(x=>x).ToList();
        }

        private readonly string _saviorKey = "modulesManager";
        private readonly string _saviorKey_removeLibs = "modulesManager_removeLibs";
        private string _baseDir;
        private ScenariosRepositoryBase _scenarioRepository;
        private ISavior _savior;
        private List<Type> _allTypes = new List<Type>();
        private List<string> _libraries = new List<string>();
        private List<string> _librariesToRemove = new List<string>();

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
            foreach (var type in _allTypes)
            {
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
        public string[] GetAllLibraries()
        {
            return _libraries.ToArray();
        }

        /// <summary>
        /// Remove all IAction types of library from Pyrite
        /// </summary>
        /// <param name="library"></param>
        public void RemoveLibrary(string library)
        {
            var canRemoveResult = CanRemoveLibrary(library);
            if (!canRemoveResult.CanRemove)
                throw new InvalidOperationException(canRemoveResult.Message);
            var destFilePath = Path.Combine(_baseDir, library);
            try
            {
                var assembly = Assembly.LoadFrom(destFilePath);
                var libraryTypes = GetTargetTypes(assembly);
                _allTypes.RemoveAll(x => libraryTypes.Any(z => z.Equals(x)));
            }
            catch (BadImageFormatException e)
            {
                //do nothing. File is corrupted or not dll
            }
            _librariesToRemove.Add(library);
            _libraries.Remove(library);
            _savior.Set(_saviorKey, _libraries);
            _savior.Set(_saviorKey_removeLibs, _librariesToRemove);
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
                    if (canAddLibResult.Message.Equals("Cannot add library. File already exists."))
                        throw new ModuleAlreadyExistsException(canAddLibResult.Message);
                    else throw new Exception(canAddLibResult.Message);
                }
                File.Copy(libraryPath, destFilePath);
                _libraries.Add(fileName);
                _allTypes.AddRange(GetTargetTypes(Assembly.LoadFrom(destFilePath)));
                _savior.Set(_saviorKey, _libraries);
                loadedFileIsNotLibrary = false;
            }
            catch (ModuleAlreadyExistsException e)
            { 
                throw e;
            }
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
        public CanRemoveLibraryResult CanRemoveLibrary(string library)
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
                    return new CanRemoveLibraryResult(false,
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
                        return new CanRemoveLibraryResult(false,
                            "Cannot remove library, because there is some libraries referenced on it:\r\n" +
                            allReferencedLibrariesNames);
                    }
                }
                return new CanRemoveLibraryResult(true);
            }
            catch (BadImageFormatException e)
            {
                //then is not just dll or dll is corrupted
                return new CanRemoveLibraryResult(true);
            }
        }

        public CanAddLibraryResult CanAddLibrary(string libraryPath)
        {
            var fileName = Path.GetFileName(libraryPath);
            var destFilePath = Path.Combine(_baseDir, fileName);
            if (_libraries.Contains(fileName) || File.Exists(destFilePath))
                return new CanAddLibraryResult(false, "Cannot add library. File already exists.");
            else return new CanAddLibraryResult(true);
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
