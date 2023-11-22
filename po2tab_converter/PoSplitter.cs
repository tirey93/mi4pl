
using System;
using System.Linq;

namespace po2tab_converter
{
    internal class PoSplitter
    {
        public string FullString { get; protected set; }
        public bool IsValid { get; set; }

        private const string _plStatic = "msgstr";
        private const string _orgStatic = "msgid";
        private const string _markupStatic = "msgctxt";

        private readonly string _plLine;
        private readonly string _orgLine;
        private readonly string _markup;

        public string PlText => GetPlText();
        public string OrgText => GetOrgText();
        public string Markup => GetMarkup();

        protected string GetPlText()
        {
            if (_plLine.Contains(_plStatic))
            {
                var result = _plLine.Remove(_plLine.Length - 1, 1);
                return result.Remove(0, _plStatic.Length + 2);
            }
            return string.Empty;
        }
        protected string GetOrgText()
        {
            if (_orgLine.Contains(_orgStatic))
            {
                var result = _orgLine.Remove(_orgLine.Length - 1, 1);
                return result.Remove(0, _orgStatic.Length + 2);
            }
            return string.Empty;
        }
        protected string GetMarkup()
        {
            if (_markup.Contains(_markupStatic))
            {
                var result = _markup.Remove(_markup.Length - 1, 1);
                return result.Remove(0, _markupStatic.Length + 2);
            }
            return string.Empty;
        }

        public PoSplitter(string fullString)
        {
            FullString = fullString;
            FullString = FullString.Replace("\\", "");
            var splitted = FullString.Split('\n');
            IsValid = false;
            if (splitted.Any(x => x.Contains(_plStatic)))
            {
                _plLine = splitted.FirstOrDefault(x => x.Contains(_plStatic));
                IsValid = true;
            }
            if (splitted.Any(x => x.Contains(_orgStatic)))
            {
                _orgLine = splitted.FirstOrDefault(x => x.Contains(_orgStatic));
                IsValid = true;
            }
            if (splitted.Any(x => x.Contains(_markupStatic)))
            {
                _markup = splitted.FirstOrDefault(x => x.Contains(_markupStatic));
                IsValid = true;
            }

        }


    }
}
