using System.Globalization;

using NUnit.Framework;

using GNU.Gettext.Msgfmt;


namespace GNU.Gettext.Test
{
    [TestFixture()]
    public class MsgfmtTest
    {
        [Test()]
        public void AssemblyGenerationTest()
        {
            Options options = new Options();
            options.Mode = Mode.SateliteAssembly;
            options.InputFiles.Add("../../../../Examples.Hello/po/fr.po");
            options.BaseName = "Examples.Hello.Messages";
            options.OutDir = "../../../Examples.Hello/bin/Debug";
            options.LibDir = "./";
            options.Locale = new CultureInfo("fr-FR");
            options.DebugMode = true;

            AssemblyGen gen = new AssemblyGen(options);
            try
            {
                gen.Run();
            }
            catch (Exception e)
            {
                Assert.Fail("Assebly generation failed. Exception message:\n{0}", e.Message);
            }
        }

        [Test()]
        public void ResourcesGenerationTest()
        {
            Options options = new Options();
            options.Mode = Mode.Resources;
            options.InputFiles.Add("./Data/Test01.po");
            options.OutFile = "./Messages.fr-FR.resources";

            ResourcesGen gen = new ResourcesGen(options);
            gen.Run();
        }
    }
}

