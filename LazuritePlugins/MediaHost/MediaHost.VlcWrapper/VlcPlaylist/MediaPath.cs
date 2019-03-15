using System;
using System.Diagnostics;

namespace MediaHost.VlcWrapper.Playlists
{
    public class MediaPath
    {
        public MediaPath()
        {
            // Do nothing
        }

        public MediaPath(string name, Uri path, MediaPath parent)
        {
            RealName = name;
            _path = path;
            Parent = parent;

#if DEBUG
            Debug.WriteLine($"MediaPath created: {Title} ({Path})");
#endif
        }

        public string RealName { get; set; }

        public string Title
        {
            get
            {
                if (!string.IsNullOrEmpty(RealName))
                {
                    return RealName;
                }

                var parentName = Parent?.Title;
                if (!string.IsNullOrEmpty(parentName))
                {
                    return parentName;
                }

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