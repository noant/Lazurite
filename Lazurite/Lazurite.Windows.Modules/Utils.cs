using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Modules
{
    public static class Utils
    {
        public static readonly string[] IgnorableAssemblies;

        static Utils()
        {
            IgnorableAssemblies = Directory.GetFiles(Path.GetDirectoryName(GetAssemblyPath(typeof(Utils).Assembly)))
                .Select(x=>Path.GetFileName(x).ToUpper())
                .Where(x => x.EndsWith(".DLL") || x.EndsWith(".EXE")).ToArray();
        }

        public static bool IsIgnorableAssembly(string fileName)
        {
            fileName = fileName.ToUpper();
            return IgnorableAssemblies.Any(x => x.Equals(fileName));
        }

        public static AssemblyTargetTypes[] GetAssembliesWithType(Type type, string directory)
        {
            var result = new List<AssemblyTargetTypes>();
            var allFiles = Directory.GetFiles(directory);
            var allDirs = Directory.GetDirectories(directory);
            var currentAssembly = Assembly.GetAssembly(typeof(Utils));
            var currentAssemblyFileName = Path.GetFileName(currentAssembly.Location).ToUpper();
            var searchingAssignableTypeAssembly = type.Assembly;
            var searchingAssignableAssemblyIgnore = Path.GetFileName(searchingAssignableTypeAssembly.Location).ToUpper();
            DoWithFiles(directory,
                (filePath) => {
                    try
                    {
                        var fileName = Path.GetFileName(filePath).ToUpper();
                        if ((fileName.EndsWith(".DLL") || fileName.EndsWith(".EXE"))
                            && fileName != currentAssemblyFileName 
                            && fileName != searchingAssignableAssemblyIgnore 
                            && !IsIgnorableAssembly(fileName))
                        {
                            var assembly = LoadAssembly(filePath);
                            var types = assembly.GetTypes().Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToArray();
                            if (types.Any())
                                result.Add(new AssemblyTargetTypes()
                                {
                                    Assembly = assembly,
                                    Types = types
                                });
                        }
                    }
                    catch
                    {
                        //do nothing; file is not C# library or broken
                    }
                });

            return result.ToArray();
        }

        public static void DoWithFiles(string directory, Action<string> action)
        {
            var allFiles = Directory.GetFiles(directory);
            var allDirs = Directory.GetDirectories(directory);
            foreach (var file in allFiles)
            {
                action(file);
            }
            foreach (var dir in allDirs)
            {
                DoWithFiles(dir, action);
            }
        }

        public static void CreatePackage(string folder, string outerFile)
        {
            var zip = new FastZip();
            zip.CreateZip(outerFile, folder, true, string.Empty);
        }

        public static void UnpackFile(string filePath, string destinationFolder)
        {
            var zip = new FastZip();
            zip.ExtractZip(filePath, destinationFolder, string.Empty);
        }
        
        public static Assembly LoadAssembly(string path)
        {
            return Assembly.LoadFrom(path);
        }

        public static string GetAssemblyPath(Assembly assembly)
        {
            return Lazurite.Windows.Utils.Utils.GetAssemblyPath(assembly);
        }
    }
}
