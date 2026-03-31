using System.Globalization;

using GNU.Gettext.Msgfmt;

using Xunit;

namespace GNU.Gettext.Test;

public class MsgfmtTest
{
    [Fact]
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
        gen.Run();
    }

    [Fact]
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
