using System;
using System.Collections.Generic;
using System.Text;

namespace po2tab_converter
{
    internal class Line
    {
        public string Markup { get; set; }
        public string Contents { get; set; }

        public Line(string line)
        {
            var splitted = line.Split('\t');
            if (splitted.Length != 2)
                return;
            Markup = splitted[0].ToLowerInvariant();
            Contents = splitted[1];
        }
    }
    
    internal class LineMerge
    {
        public static Dictionary<string, string> _dictPl;
        public string Markup { get; set; }
        public string ContentsEn { get; set; }
        public string ContentsPl { get; set; }

        public LineMerge(Line line)
        {
            Markup = line.Markup;
            ContentsEn = line.Contents;
            if (_dictPl.TryGetValue(line.Markup, out var contentPl))
                ContentsPl = contentPl;

        }
    }
}
