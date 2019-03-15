namespace MediaHost.VlcWrapper.VlcPlaylist
{
    public class LowLevelItem
    {
        internal LowLevelItem(string extinf, string extgrp, string uri)
        {
            if (!string.IsNullOrEmpty(extgrp) && extgrp.StartsWith("#EXTGRP"))
            {
                Group = extgrp.Substring(9).Trim();
            }

            if (!string.IsNullOrEmpty(extinf) && extinf.StartsWith("#EXTINF"))
            {
                GroupTitle = GetParamValue("group-title", extinf)?.Trim();
                TvgId = GetParamValue("tvg-id", extinf)?.Trim();
                TvgName = GetParamValue("tvg-name", extinf)?.Trim();
                TvgLogoPath = GetParamValue("tvg-logo", extinf)?.Trim();

                var nameIndex = extinf.LastIndexOf(',');
                if (nameIndex != -1)
                    Name = extinf.Substring(nameIndex + 1).Trim();
            }

            Path = uri?.Trim();
        }

        private static string GetParamValue(string paramName, string source)
        {
            var index = -1;
            if ((index = source.IndexOf(paramName)) != -1)
            {
                return source.Substring(index + paramName.Length + 2, source.IndexOf('"', index + paramName.Length + 2) - (index + paramName.Length + 2));
            }

            return string.Empty;
        }

        public string Group { get; }

        public string GroupTitle { get; }

        public string TvgId { get; }

        public string TvgLogoPath { get; }

        public string TvgName { get; }

        public string Name { get; }

        public string Path { get; }
    }
}