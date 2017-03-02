using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Modules
{
    public static class Utils
    {
        public static AssemblyTargetTypes[] GetAssembliesWithType(Type type, string directory)
        {
            var result = new List<AssemblyTargetTypes>();
            var allFiles = Directory.GetFiles(directory);
            var allDirs = Directory.GetDirectories(directory);
            foreach (var file in allFiles)
            {
                try
                {
                    var assembly = LoadAssembly(file);
                    var types = assembly.DefinedTypes.Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToArray();
                    if (types.Any())
                        result.Add(new AssemblyTargetTypes()
                        {
                            Assembly = assembly,
                            Types = types
                        });
                }
                catch
                {
                    //do nothing; file is not C# library or broken
                }
            }
            foreach (var dir in allDirs)
            {
                result.AddRange(GetAssembliesWithType(type, dir));
            }

            return result.ToArray();
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

        public static string GetAssemblyPath(Assembly assembly)
        {
            string codeBase = assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            return Path.GetFullPath(Uri.UnescapeDataString(uri.Path));
        }

        public static Assembly LoadAssembly(string path)
        {
            return Assembly.UnsafeLoadFrom(path);
        }        
    }
}
