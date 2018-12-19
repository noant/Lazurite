using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHost.VlcWrapper.Playlists
{
    public class MediaPath
    {
        public MediaPath()
        {
        }

        public MediaPath(string name, Uri path, MediaPath parent)
        {
            RealName = name;
            _path = path;
            Parent = parent;
        }

        public string RealName { get; set; }

        public string Title
        {
            get
            {
                if (!string.IsNullOrEmpty(RealName))
                    return RealName;
                var parentName = Parent?.Title;
                if (!string.IsNullOrEmpty(parentName))
                    return parentName;
                return "Без имени";
            }
        }

        private Uri _path;

        public Uri GetUri() => _path;

        public string Path
        {
            get => _path.OriginalString;
            set => _path = new Uri(value);
        }

        public MediaPath Parent { get; set; }
    }
}
