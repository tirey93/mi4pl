using System;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace po2tab_converter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Convert(args);
            //ImportLackingLines();
        }

        private static void ImportLackingLines()
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
            LineMerge._dictPl = linesPl.Select(x => new Line(x))
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
                    if(results.TryGetValue(orginal, out var duplKeys))
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
                foreach(var subStr in result.Value)
                {
                    var msgid = dictEn[subStr];
                    var msgstr = LineMerge._dictPl[result.Key];

                    toFile += 
                     $"msgctxt \"{subStr}\"\r\n" +
                     $"msgid \"{msgid}\"\r\n" +
                     $"msgstr \"{msgstr}\"\r\n\r\n";
                }
            }

            File.WriteAllText("duplicates.po", toFile, Encoding.GetEncoding("windows-1250"));

        }
        private static void Convert(string[] args)
        {
            string fileName = "..\\..\\..\\omega_t\\team project\\target\\efmi.po";

            if (!File.Exists(fileName))
            {
                fileName = "efmi.po";
            }

            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
            {
                fileName = args[0];
            }

            string file = File.ReadAllText(fileName);
            var splitted = file.Split("msgctxt");

            string result = "";
            string errors = "";

            foreach (var text in splitted)
            {
                if (string.IsNullOrEmpty(text)) continue;
                var textWithCuttedStart = "msgctxt" + text;

                var splitter = new PoSplitter(textWithCuttedStart);
                if (!splitter.IsValid)
                    continue;

                try
                {
                    var markup = splitter.Markup;
                    string textTarget = markup;
                    var plText = splitter.PlText;
                    if (!string.IsNullOrEmpty(plText))
                    {
                        textTarget = plText;
                    }
                    else if (!string.IsNullOrEmpty(splitter.OrgText))
                    {
                        textTarget = splitter.OrgText;
                    }

                    result += markup + "\t" + textTarget + "\r\n";
                }
                catch (Exception ex)
                {
                    errors += textWithCuttedStart;
                    errors += ex.Message;
                    errors += "\n\n";
                }
            }

            System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ppp);
            if (!string.IsNullOrEmpty(errors))
            {
                File.WriteAllText("errors.txt", errors, Encoding.GetEncoding("windows-1250"));
            }
            File.WriteAllText("script.tab", result, Encoding.GetEncoding("windows-1250"));
        }
    }
}
