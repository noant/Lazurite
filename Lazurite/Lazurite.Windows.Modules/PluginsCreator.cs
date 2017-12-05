using Lazurite.ActionsDomain;
using System;
using System.IO;
using System.Linq;

namespace Lazurite.Windows.Modules
{
    public static class PluginsCreator
    {
        public static readonly string PluginExtension = ".pyp";

        public static void CreatePluginFile(string pluginFilesDirectoryPath, string destinationFile)
        {
            var targetAssemblies = Utils.GetAssembliesWithType(typeof(IAction), pluginFilesDirectoryPath);
            if (!targetAssemblies.Any())
                throw new DllNotFoundException("Folder has no any assembly with defined derived from IAction type");
            Utils.CreatePackage(pluginFilesDirectoryPath, destinationFile);
        }

        public static void ExtractPluginFile(string pluginFilePath, string destinationFolder)
        {
            var pluginName = Path.GetFileNameWithoutExtension(pluginFilePath);
            var targetFolder = Path.Combine(destinationFolder, pluginName);
            if (Directory.Exists(targetFolder))
                throw new FileLoadException("Plugin with name '" + pluginName + "' already exist");
            else Directory.CreateDirectory(targetFolder);
            Utils.UnpackFile(pluginFilePath, targetFolder);
        }
    }
}
