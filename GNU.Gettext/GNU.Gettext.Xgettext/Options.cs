using System.Text;
using System.Collections.Generic;

namespace GNU.Gettext.Xgettext
{
    public class Options
	{
		public Options()
		{
            InputFiles = new List<string>();
			InputDirs = new List<string>();
			InputEncoding = new UTF8Encoding();
			SearchPatterns = new List<string>();
			OutFile = "messages.pot";
			DetectEncoding = false;
		}

        public string OutFile { get; set; }
		public bool Overwrite { get; set; }
		public bool Recursive { get; set; }
		public bool Verbose { get; set; }
		public bool ShowUsage { get; set; }
		public Encoding InputEncoding { get; set; }
		public bool DetectEncoding { get; set; }
		public List<string> InputFiles { get; private set; }
		public List<string> InputDirs { get; private set; }
		public List<string> SearchPatterns { get; private set; }
		
		public void SetEncoding(string encodingName)
		{
			InputEncoding = Encoding.GetEncoding(encodingName);
		}
	}
}
