using System.Text;

using Xunit;
using GNU.Gettext.Msgfmt;

namespace GNU.Gettext.Test
{
    public class GetoptTest
    {
        [Fact]
        public void GetoptParamsTest()
        {
            string[] args = new string[]
            {
                "-l", "fr-FR",
                "-d", "./bin/Debug",
                "-r", "Examples.Hello.Messages",
                "-L", "./../../Bin",
                "./po/fr.po",
                "./po/ru.po"
            };
            Options options = new Options();
            Assert.True(Msgfmt.Program.GetOptions(args, Msgfmt.Program.SOpts, Msgfmt.Program.LOpts, options, out StringBuilder message));
            CheckOptions(options);
            Assert.Equal(Mode.SateliteAssembly, options.Mode);
        }

        [Fact]
        public void GetoptLongParamsTest()
        {
            string[] args = new string[]
            {
                "--locale=fr-FR",
                "-d./bin/Debug",
                "--resource=Examples.Hello.Messages",
                "-L ./../../Bin",
                "--check-format",
                "./po/fr.po",
                "./po/ru.po"
            };
            Options options = new Options();
            Assert.True(Msgfmt.Program.GetOptions(args, Msgfmt.Program.SOpts, Msgfmt.Program.LOpts, options, out StringBuilder message));
            Assert.Equal(0, message.Length);
            CheckOptions(options);
            Assert.Equal(Mode.SateliteAssembly, options.Mode);
        }

        private void CheckOptions(Options options)
        {
            Assert.Equal(2, options.InputFiles.Count);
            Assert.Equal("./po/fr.po", options.InputFiles[0]);
            Assert.Equal("fr-FR", options.LocaleStr);
            Assert.Equal("./bin/Debug", options.OutDir);
            Assert.Equal("Examples.Hello.Messages", options.BaseName);
            Assert.Equal("./../../Bin", options.LibDir);
        }

        [Fact]
        public void MsgfmtResourceModeParamsTest()
        {
            string[] args = new string[]
            {
                "--csharp-resources",
                "-l", "fr-FR",
                "-d", "./bin/Debug",
                "-r", "Examples.Hello.Messages",
                "-L", "./../../Bin",
                "./po/fr.po",
                "./po/ru.po"
            };
            Options options = new Options();
            Assert.True(Msgfmt.Program.GetOptions(args, Msgfmt.Program.SOpts, Msgfmt.Program.LOpts, options, out StringBuilder message));
            CheckOptions(options);
            Assert.Equal(Mode.Resources, options.Mode);
        }
    }
}
