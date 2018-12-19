using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHost.VlcWrapper.Playlists
{
    public class Playlist: MediaPath
    {
        public Playlist() { }

        public Playlist(string name, Uri path, MediaPath parent) : base(name, path, parent) { }
        
        public MediaPath[] Children { get; set; }

        public MediaPath[] GetLowNesting()
        {
            MediaPath[] result = null;
            if (Children.All(x => !(x is Playlist) &&
                (x.RealName == RealName || x.RealName.StartsWith("#EXTINF") || string.IsNullOrEmpty(x.RealName))))
                result = new[] { this };
            else if (Children.All(x => !(x is Playlist)))
                result = Children;
            else result = Children
                    .Where(x => x is Playlist)
                    .SelectMany(x => (x as Playlist).GetLowNesting())
                    .Union(Children.Where(x => !(x is Playlist)))
                    .ToArray();

            if (Parent == null)
            {
                int cnt = 0;

                foreach (var item in result)
                {
                    if ((cnt = result.Count(x => x.Title == item.Title)) > 1)
                        item.RealName = string.Format("{0} ({1})", item.Title, cnt);
                }

                result = result.OrderBy(x => x.Title).ToArray();
            }

            return result;
        }
    }
}
