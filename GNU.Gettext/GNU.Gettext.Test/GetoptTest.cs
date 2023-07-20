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
            Assert.AreEqual(Mode.SateliteAssembly, options.Mode);
        }

        [Test()]
        public void GetoptLongParamsTest()
        {
            string[] args = new string[]
            {
                "--locale=fr-FR",
                "-d./bin/Debug",
                "--resource=Examples.Hello.Messages",
                "--compiler-name=gmcs",
                "--lib-dir=./../../Bin",
                "--verbose",
                "--check-format",
                "./po/fr.po",
                "./po/ru.po"
            };
            Options options = new Options();
            Assert.IsTrue(Msgfmt.Program.GetOptions(args, Msgfmt.Program.SOpts, Msgfmt.Program.LOpts, options, out StringBuilder message), message.ToString());
            Assert.AreEqual(0, message.Length, message.ToString());
            CheckOptions(options);
            Assert.AreEqual("gmcs", options.CompilerName);
            Assert.AreEqual(Mode.SateliteAssembly, options.Mode);
        }

        private void CheckOptions(Options options)
        {
            Assert.AreEqual(2, options.InputFiles.Count, "input files");
            Assert.AreEqual("./po/fr.po", options.InputFiles[0]);
            Assert.AreEqual("fr-FR", options.LocaleStr);
            Assert.AreEqual("./bin/Debug", options.OutDir);
            Assert.AreEqual("Examples.Hello.Messages", options.BaseName);
            Assert.AreEqual("./../../Bin", options.LibDir);
            Assert.IsTrue(options.Verbose);
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
                "--compiler-name=gmcs",
                "-L", "./../../Bin",
                "./po/fr.po",
                "./po/ru.po"
            };
            Options options = new Options();
            Assert.IsTrue(Msgfmt.Program.GetOptions(args, Msgfmt.Program.SOpts, Msgfmt.Program.LOpts, options, out StringBuilder message), message.ToString());
            CheckOptions(options);
            Assert.AreEqual(Mode.Resources, options.Mode);
        }

    }
}

