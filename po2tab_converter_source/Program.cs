using System;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Linq;

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

            string fileEn = File.ReadAllText(fileNameEn);
            string filePl = File.ReadAllText(fileNamePl);

            string[] linesEn = fileEn.Split("\r\n");
            string[] linesPl = filePl.Split("\r\n");
            var listEn = linesEn.Select(x => new Line(x)).Where(x => x.Markup != null);
            LineMerge._dictPl = linesPl.Select(x => new Line(x))
                .Where(x => x.Markup != null)
                .GroupBy(x => x.Markup)
                .ToDictionary(x => x.Key, y => y.First().Contents, StringComparer.OrdinalIgnoreCase);

            var linesMerged = listEn.Select(x => new LineMerge(x)).Where(x => string.IsNullOrEmpty(x.ContentsPl)).ToList();

            //msgctxt "txtwed007"
            //msgid "enemy pirate"
            //msgstr "wrogi pirat"

            File.WriteAllLines("missing.po", linesMerged.Select(x =>
                 $"msgctxt \"{x.Markup}\"\r\n" +
                 $"msgid \"{x.ContentsEn}\"\r\n" +
                 $"msgstr \"{x.ContentsEn}\"\r\n"));

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
