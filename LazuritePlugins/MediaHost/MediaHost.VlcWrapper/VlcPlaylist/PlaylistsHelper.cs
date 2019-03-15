using MediaHost.VlcWrapper.VlcPlaylist;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace MediaHost.VlcWrapper.Playlists
{
    public static class PlaylistsHelper
    {
        private static readonly WebClient WebClient = new MyWebClient();

        public static MediaPath FromPath(string path, Action<string> itemLoaded, CancellationToken cancellationToken, bool? isPlaylist = null)
        {
            return FromPathInternal(path, string.Format("Без имени [{0}]", path), null, itemLoaded, cancellationToken, isPlaylist);
        }

        private static MediaPath FromPathInternal(string path, string name, MediaPath parent, Action<string> itemLoaded, CancellationToken cancellationToken, bool? isPlaylist = null)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var uri = GetUri(path);

            if (uri != null)
            {
                itemLoaded?.Invoke(uri.OriginalString);

                if (!IsAbsolute(uri))
                {
                    if (parent == null)
                    {
                        return null;
                    }

                    uri = new Uri(new Uri(WithoutQuery(parent.GetUri())), uri);
                }

                var pathType = GetPathType(uri);

                if (isPlaylist != null) // We can specify the file type
                {
                    pathType = isPlaylist.Value ? PathType.M3U8 : PathType.Other;
                }

                if (pathType == PathType.Other) // Если это не файл плейлиста, то это файл или путь к потоку
                {
                    return new MediaPath(name, uri, parent);
                }
                else
                {
                    var stream = ToStream(uri);
                    if (stream != null)
                    {
                        var items = LowLevelLoader.FromStream(stream);
                        var newPl = new Playlist(name, uri, parent);
                        var innerPlaylists = items
                            .Select(x =>
                                FromPathInternal(
                                    x.Path,
                                    x.Name?.Replace("&amp;", "&") ?? x.TvgName?.Replace("&amp;", "&"),
                                    newPl,
                                    itemLoaded,
                                    cancellationToken))
                            .Where(x => x != null)
                            .ToArray();

                        if (innerPlaylists.Length == 1)
                        {
                            return innerPlaylists.FirstOrDefault();
                        }

                        newPl.Children = innerPlaylists;

                        return newPl;
                    }
                }
            }
            return null;
        }

        private static bool IsAbsolute(Uri path)
        {
            try
            {
                return path.IsAbsoluteUri;
            }
            catch
            {
                return false;
            }
        }

        private static PathType GetPathType(Uri path)
        {
            var p = WithoutQuery(path).ToLowerInvariant();
            if (p.EndsWith(".m3u"))
            {
                return PathType.M3U;
            }

            if (p.EndsWith(".m3u8"))
            {
                return PathType.M3U8;
            }

            return PathType.Other;
        }

        private static Stream ToStream(Uri path)
        {
            try
            {
                if (IsLocal(path))
                {
                    return File.OpenRead(path.OriginalString);
                }
                else
                {
                    return WebClient.OpenRead(path);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("Cannot load stream from source [{0}]: {1}", path, e.Message));
                return null;
            }
        }

        private static string WithoutQuery(Uri path)
        {
            try
            {
                return path
                    .OriginalString
                    .Substring(0, path.OriginalString.Length - path.Query.Length)
                    .Trim()
                    .ToLowerInvariant();
            }
            catch
            {
                return path
                    .OriginalString
                    .Trim()
                    .ToLowerInvariant();
            }
        }

        private static Uri GetUri(string path)
        {
            if (Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out Uri res))
            {
                return res;
            }

            return null;
        }

        private static bool IsLocal(Uri p) => p.IsFile;

        private class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                var req = base.GetWebRequest(address);
                req.Timeout = 7000;
                return req;
            }
        }

        private enum PathType
        {
            M3U,
            M3U8,
            Other
        }
    }
}