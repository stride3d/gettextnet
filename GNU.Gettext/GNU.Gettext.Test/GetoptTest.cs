using System.Text;

using NUnit.Framework;
using GNU.Gettext.Msgfmt;

namespace GNU.Gettext.Test
{
    [TestFixture()]
    public class GetoptTest
    {
        [SetUp]
        public void TestSetup()
        { }

        [Test()]
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
            Assert.IsTrue(Msgfmt.Program.GetOptions(args, Msgfmt.Program.SOpts, Msgfmt.Program.LOpts, options, out StringBuilder message), message.ToString());
            CheckOptions(options);
            Assert.That(Mode.SateliteAssembly, Is.EqualTo(options.Mode));
        }

        [Test()]
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
            Assert.IsTrue(Msgfmt.Program.GetOptions(args, Msgfmt.Program.SOpts, Msgfmt.Program.LOpts, options, out StringBuilder message), message.ToString());
            Assert.That(0, Is.EqualTo(message.Length), message.ToString());
            CheckOptions(options);
            Assert.That(Mode.SateliteAssembly, Is.EqualTo(options.Mode));
        }

        private void CheckOptions(Options options)
        {
            Assert.That(2, Is.EqualTo(options.InputFiles.Count), "input files");
            Assert.That("./po/fr.po", Is.EqualTo(options.InputFiles[0]));
            Assert.That("fr-FR", Is.EqualTo(options.LocaleStr));
            Assert.That("./bin/Debug", Is.EqualTo(options.OutDir));
            Assert.That("Examples.Hello.Messages", Is.EqualTo(options.BaseName));
            Assert.That("./../../Bin", Is.EqualTo(options.LibDir));
        }

        [Test()]
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
            Assert.IsTrue(Msgfmt.Program.GetOptions(args, Msgfmt.Program.SOpts, Msgfmt.Program.LOpts, options, out StringBuilder message), message.ToString());
            CheckOptions(options);
            Assert.That(Mode.Resources, Is.EqualTo(options.Mode));
        }

    }
}

