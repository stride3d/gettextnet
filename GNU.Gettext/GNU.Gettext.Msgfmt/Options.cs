using System.Collections.Generic;
using System.Globalization;

namespace GNU.Gettext.Msgfmt
{
    public class Options
    {
        public Options()
        {
            InputFiles = new List<string>();
        }

        public string CompilerName { get; set; }
        public string OutFile { get; set; }
        public string OutDir { get; set; }
        public string LibDir { get; set; }
        public string LocaleStr { get; set; }
        public CultureInfo Locale { get; set; }
        public string BaseName { get; set; }
        public Mode Mode { get; set; }
        public bool CheckFormat { get; set; }
        public bool Verbose { get; set; }
        public bool ShowUsage { get; set; }
        public bool DebugMode { get; set; }
        public bool HasNamespace
        {
            get { return !string.IsNullOrEmpty(BaseName); }
        }
        public List<string> InputFiles { get; private set; }
    }
}
