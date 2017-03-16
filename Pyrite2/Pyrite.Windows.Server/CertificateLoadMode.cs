using Pyrite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.Server
{
    public enum CertificateLoadMode
    {
        [HumanFriendlyName("Встроенный")]
        Default,
        [HumanFriendlyName("Из файла")]
        File,
        [HumanFriendlyName("Найти в Windows по имени cубъекта")]
        SubjectName
    }
}
