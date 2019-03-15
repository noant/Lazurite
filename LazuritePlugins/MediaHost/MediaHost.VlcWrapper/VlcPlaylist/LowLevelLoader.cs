using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MediaHost.VlcWrapper.VlcPlaylist
{
    public static class LowLevelLoader
    {
        public static LowLevelItem[] FromStream(Stream stream)
        {
            var lines = InternalReadAllLines(stream);

            if (!lines.Any() || !lines[0].StartsWith("#EXT"))
            {
                return new LowLevelItem[0];
            }

            string itemExtinf = null;
            string itemExtgroup = null;

            var result = new List<LowLevelItem>();

            foreach (var line in lines)
            {
                if (line.StartsWith("#EXTINF"))
                {
                    itemExtinf = line;
                }
                else if (line.StartsWith("#EXTGRP"))
                {
                    itemExtgroup = line;
                }
                else if (line.StartsWith("#EXT")) // Ignore all other tags
                {
                    continue;
                }
                else if (!string.IsNullOrEmpty(itemExtinf)) // Then line is uri
                {
                    result.Add(new LowLevelItem(itemExtinf, itemExtgroup, line));
                    itemExtgroup = null;
                    itemExtinf = null;
                }
            }

            return result.ToArray();
        }

        private static string[] InternalReadAllLines(Stream stream)
        {
            byte[] data;

            using (stream)
            using (var ms = new MemoryStream())
            {
                var cnt = 0;
                do
                {
                    var buff = new byte[512];
                    cnt = stream.Read(buff, 0, 512);
                    ms.Write(buff, 0, cnt);
                } while (stream.CanRead && cnt > 0);

                data = ms.ToArray();
            }

            var sb = new StringBuilder(Encoding.UTF8.GetString(data));

            // Crutch for bad files
            sb.Replace("\r", string.Empty)
                .Replace("#EXT", "\n#EXT")
                .Replace("\n\n", "\n");

            var lines = sb.ToString().Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }
    }
}