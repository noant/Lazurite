using System;
using System.Linq;

namespace MediaHost.VlcWrapper.Playlists
{
    public class Playlist : MediaPath
    {
        public Playlist()
        {
            // Do nothing
        }

        public Playlist(string name, Uri path, MediaPath parent) : base(name, path, parent)
        {
            // Do nothing
        }

        public MediaPath[] Children { get; set; }

        public MediaPath[] Expand()
        {
            MediaPath[] result = null;

            if (!Children.Any())
            {
                result = new MediaPath[0];
            }
            else if (Children.All(x => !(x is Playlist) && (x.RealName == RealName || string.IsNullOrEmpty(x.RealName) || x.RealName.StartsWith("#EXTINF"))))
            {
                result = new[] { this };
            }
            else if (Children.All(x => !(x is Playlist)))
            {
                result = Children;
            }
            else
            {
                result =
                    Children
                    .Where(x => x is Playlist)
                    .SelectMany(x => (x as Playlist).Expand())
                    .Union(Children.Where(x => !(x is Playlist)))
                    .ToArray();
            }

            if (Parent == null)
            {
                var cnt = 0;

                foreach (var item in result)
                {
                    if ((cnt = result.Count(x => x.Title == item.Title)) > 1)
                    {
                        item.RealName = string.Format("{0} ({1})", item.Title, cnt);
                    }
                }

                result = result.OrderBy(x => x.Title).ToArray();
            }

            return result;
        }
    }
}