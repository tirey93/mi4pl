using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace po2tab_converter
{
    internal class Tools
    {
        public static void ImportLackingLines()
        {
            string fileNameEn = "script_en.tab";
            string fileNamePl = "script_pl.tab";



            System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ppp);


            string fileEn = File.ReadAllText(fileNameEn, Encoding.GetEncoding("windows-1250"));
            string filePl = File.ReadAllText(fileNamePl, Encoding.GetEncoding("windows-1250"));

            string[] linesEn = fileEn.Split("\r\n");
            string[] linesPl = filePl.Split("\r\n");
            var listEn = linesEn.Select(x => new Line(x)).Where(x => x.Markup != null);
            var dictPl = linesPl.Select(x => new Line(x))
                .Where(x => x.Markup != null)
                .GroupBy(x => x.Markup)
                .ToDictionary(x => x.Key, y => y.First().Contents, StringComparer.OrdinalIgnoreCase);

            var dictEn = listEn
                .Where(x => x.Markup != null)
                .GroupBy(x => x.Markup)
                .ToDictionary(x => x.Key, y => y.First().Contents, StringComparer.OrdinalIgnoreCase);

            //msgctxt "txtwed007"
            //msgid "enemy pirate"
            //msgstr "wrogi pirat"

            /*   File.WriteAllLines("missing.po", linesMerged.Select(x =>
                    $"msgctxt \"{x.Markup}\"\r\n" +
                    $"msgid \"{x.ContentsEn}\"\r\n" +
                    $"msgstr \"{x.ContentsEn}\"\r\n"));*/

            var enMarkup = listEn.Select(x => x.Markup);
            var enMarkupDict = listEn.Select(x => x.Markup)
                .Where(x => x != null)
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, y => y.Key);
            var results = new Dictionary<string, List<string>>();
            foreach (var markup in enMarkup)
            {
                var orginal = markup.Substring(0, markup.Length - 1);
                if (enMarkupDict.ContainsKey(orginal))
                {
                    if (results.TryGetValue(orginal, out var duplKeys))
                    {
                        duplKeys.Add(markup);
                    }
                    else
                    {
                        results.Add(orginal, new List<string>() { markup });
                    }
                }
            }


            //File.WriteAllLines("dupl.txt", results.Select(x => $"{x.Key} -> {String.Join(",", x.Value)}" ));

            var toFile = "";
            foreach (var result in results)
            {
                foreach (var subStr in result.Value)
                {
                    var msgid = dictEn[subStr];
                    var msgstr = dictPl[result.Key];

                    toFile +=
                     $"msgctxt \"{subStr}\"\r\n" +
                     $"msgid \"{msgid}\"\r\n" +
                     $"msgstr \"{msgstr}\"\r\n\r\n";
                }
            }

            File.WriteAllText("duplicates.po", toFile, Encoding.GetEncoding("windows-1250"));

        }

        public static void FindingDifferences()
        {
            string fileNameEn = "script_en.tab";
            string fileNamePl = "script_pl.tab";



            System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ppp);


            string fileEn = File.ReadAllText(fileNameEn, Encoding.GetEncoding("windows-1250"));
            string filePl = File.ReadAllText(fileNamePl, Encoding.GetEncoding("windows-1250"));

            string[] linesEn = fileEn.Split("\r\n");
            string[] linesPl = filePl.Split("\r\n");
            var dictEn = linesEn.Select(x => new Line(x))
                .Where(x => x.Markup != null)
                .GroupBy(x => x.Markup)
                .ToDictionary(x => x.Key, y => y.First().Contents, StringComparer.OrdinalIgnoreCase);

            var linesPlList = linesPl.Select(x => new Line(x)).ToList();

            var result = new List<string>();
            foreach (var line in linesPlList.Where(x => x.Markup != null))
            {
                if (!dictEn.TryGetValue(line.Markup, out var linePl))
                {
                    result.Add(line.Markup);
                }
            }

            File.WriteAllLines("inPlOnly.po", result, Encoding.GetEncoding("windows-1250"));

        }

        public static void AnyDuplicatesInPl()
        {
            string fileNameEn = "script_en.tab";
            string fileNamePl = "script_pl.tab";



            System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ppp);


            string fileEn = File.ReadAllText(fileNameEn, Encoding.GetEncoding("windows-1250"));
            string filePl = File.ReadAllText(fileNamePl, Encoding.GetEncoding("windows-1250"));

            string[] linesEn = fileEn.Split("\r\n");
            string[] linesPl = filePl.Split("\r\n");
            var dictPl = linesPl.Select(x => new Line(x))
                .Where(x => x.Markup != null)
                .GroupBy(x => x.Markup)
                .ToDictionary(x => x.Key, y => y.ToList(), StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Value.Count > 1);
            var listEn = linesEn.Select(x => new Line(x)).Where(x => x.Markup != null);
            var dictEn = listEn
                .Where(x => x.Markup != null)
                .GroupBy(x => x.Markup)
                .ToDictionary(x => x.Key, y => y.First().Contents, StringComparer.OrdinalIgnoreCase);
            int a = 5;
            var toFile = "";
            foreach (var result in dictPl)
            {
                foreach (var subLine in result.Value)
                {
                    toFile +=
                     $"msgctxt \"{subLine.Markup}\"\r\n" +
                     $"msgid \"{dictEn[subLine.Markup]}\"\r\n" +
                     $"msgstr \"{subLine.Contents}\"\r\n\r\n";
                }
            }

            File.WriteAllText("duplicates.po", toFile, Encoding.GetEncoding("windows-1250"));

            //File.WriteAllText("duplicates.po", toFile, Encoding.GetEncoding("windows-1250"));

        }
    }
}
