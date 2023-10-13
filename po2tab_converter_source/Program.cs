using System;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace po2tab_converter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = "efmi.po";
            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
            {
                fileName = args[0];
            }

            string file = File.ReadAllText(fileName);
            var splitted = file.Split("msgctxt");

            var regexMarkup = new Regex("msgctxt \"([^\"]+)\"");
            var regexPl = new Regex("msgstr \"([^\"]+)\"");

            string result = "";
            string errors = "";

            foreach (var text in splitted)
            {
                if (string.IsNullOrEmpty(text)) continue;
                var textWithCuttedStart = "msgctxt" + text;

                try
                {
                    result += regexMarkup.Match(textWithCuttedStart).Groups[1].Value + "\t" + regexPl.Match(textWithCuttedStart).Groups[1].Value + "\r\n";
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
