using Xunit;

namespace GNU.Gettext.Test;

public class GettextResourceManagerTest
{
    [Fact]
    public void NamesExtractionTest()
    {
        string n1 = "One.Two.Three";
        Assert.Equal("Three", GettextResourceManager.ExtractClassName(n1));
        Assert.Equal("One.Two", GettextResourceManager.ExtractNamespace(n1));

        Assert.Equal("Class", GettextResourceManager.ExtractClassName("Class"));
        Assert.Equal(string.Empty, GettextResourceManager.ExtractNamespace(".Test"));
    }

    [Fact]
    public void Ex1Test()
    {
        Assert.Throws<Exception>(() => GettextResourceManager.ExtractClassName(null));
    }

    [Fact]
    public void Ex2Test()
    {
        Assert.Throws<Exception>(() =>
            GettextResourceManager.ExtractClassName(string.Empty));
    }

    [Fact]
    public void Ex3Test()
    {
        Assert.Throws<Exception>(() =>
            GettextResourceManager.ExtractClassName("Class."));
    }
}
