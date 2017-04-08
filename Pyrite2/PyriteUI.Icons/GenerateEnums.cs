using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyriteUI.Icons
{
    public class GenerateEnums
    {
        public static string GenerateString()
        {
            var allResources = typeof(GenerateEnums).Assembly.GetManifestResourceNames();
            var targetStr = "";
            foreach (var resourceName in allResources)
            {
                if (resourceName.EndsWith(".png"))
                {
                    var splt = resourceName.Split('.');
                    targetStr += "    " + splt[splt.Length - 2]+",\r\n"; 
                }
            }
            return targetStr;
        }
    }
}
