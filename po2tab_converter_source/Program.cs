using System;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace po2tab_converter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Convert(args);
            //Tools.FindingDifferences();
            //Tools.AnyDuplicatesInPl();
        }
        private static void Convert(string[] args)
        {
            string configFile = File.ReadAllText("config.json");
            var config = JsonSerializer.Deserialize<Config>(configFile);
            var fileName = config.FileName;

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
