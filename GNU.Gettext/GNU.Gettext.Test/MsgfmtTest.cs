using System.Globalization;

using GNU.Gettext.Msgfmt;

using Xunit;

namespace GNU.Gettext.Test;

public class MsgfmtTest
{
    [Fact]
    public void AssemblyGenerationTest()
    {
        Options options = new()
        {
            Mode = Mode.SateliteAssembly,
            InputFiles = { "../../../../Examples.Hello/po/fr.po" },
            BaseName = "Examples.Hello.Messages",
            OutDir = "../../../Examples.Hello/bin/Debug",
            LibDir = "./",
            Locale = new CultureInfo("fr-FR"),
            DebugMode = true
        };

        AssemblyGen gen = new(options);
        gen.Run();
    }

    [Fact]
    public void ResourcesGenerationTest()
    {
        Options options = new()
        {
            Mode = Mode.Resources
        };
        options.InputFiles.Add("./Data/Test01.po");
        options.OutFile = "./Messages.fr-FR.resources";

        ResourcesGen gen = new(options);
        gen.Run();
    }
}
