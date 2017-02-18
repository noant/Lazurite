using Pyrite.ActionsDomain;
using Pyrite.IOC;
using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Modules
{
    public class ModulesManager
    {
        private ScenariosRepositoryBase _scenarioRepository = Singleton.Resolve<ScenariosRepositoryBase>();

        private List<Type> _allTypes = new List<Type>();

        public List<string> _libraries = new List<string>();

        /// <summary>
        /// Remove all type of library from Pyrite
        /// </summary>
        /// <param name="library"></param>
        public void RemoveLibrary(string library)
        {
            var assembly = Assembly.LoadFrom(library);
            var libraryTypes = GetTargetTypes(assembly);
            _allTypes.RemoveAll(x => libraryTypes.Any(z => z.Equals(x)));
        }

        /// <summary>
        /// Determines when library can be removed
        /// </summary>
        /// <param name="library"></param>
        /// <returns></returns>
        public CanRemoveLibraryResult CanRemoveLibrary(string library)
        {
            var assembly = Assembly.LoadFrom(library);
            var libraryTypes = GetTargetTypes(assembly);
            var dependentScenarios = _scenarioRepository.GetDependentScenarios(libraryTypes);
            //determines dependent scenarios
            if (dependentScenarios.Any())
            {
                var allDependentScenariosNames = dependentScenarios
                        .Select(x => x.Name)
                        .Aggregate((x1, x2) => x1 + ";\r\n" + x2);
                return new CanRemoveLibraryResult(false, 
                    "Cannot remove library, because there is some scenarios referenced on it:\r\n"+ 
                    allDependentScenariosNames);
            }
            else
            {
                //determines referenced libraries
                var referencedLibraries = _libraries
                    .Select(x => Assembly.LoadFrom(x))
                    .Where(x => assembly.GetReferencedAssemblies()
                    .Any(z => z.FullName.Equals(x.FullName))).ToArray();
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

        /// <summary>
        /// Get types defined in assembly and derived from IAction
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private Type[] GetTargetTypes(Assembly assembly)
        {
            return assembly.DefinedTypes.Where(x => x.IsAssignableFrom(typeof(IAction))).ToArray();
        }
    }
}
