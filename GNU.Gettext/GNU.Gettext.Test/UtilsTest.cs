using Xunit;

using GNU.Gettext.Utils;

namespace GNU.Gettext.Test
{
    public class UtilsTest
    {
        [Fact]
        public void RelativePathTest()
        {
            Assert.Equal("test/test.htm", FileUtils.GetRelativeUri("http://www.contoso.com/test/test.htm", "http://www.contoso.com/"));
            Assert.Equal("../", FileUtils.GetRelativeUri("http://www.contoso.com/", "http://www.contoso.com/test1/dummy"));

            Assert.Equal("../../Messages.pot", FileUtils.GetRelativeUri(@"C:\dir1\dir2\Messages.pot", @"C:\dir1\dir2\dir3\dir4\"));
            Assert.Equal("../../Messages.pot", FileUtils.GetRelativeUri(
                Path.Combine(Environment.CurrentDirectory, "Messages.pot"),
                Path.Combine(Environment.CurrentDirectory, string.Format("dir{0}subdir{0}", Path.DirectorySeparatorChar))));
        }

        [Fact]
        public void ReadStringsTest()
        {
            List<string> files = FileUtils.ReadStrings("./Data/UtilsTest.txt");
            Assert.Equal(3, files.Count);

            FileUtils.ReadStrings("./Data/UtilsTest.txt", files);
            Assert.Equal(3, files.Count);

            files = new List<string>
            {
                "Test 1",
                "Test 2"
            };
            FileUtils.ReadStrings("./Data/UtilsTest.txt", files);
            Assert.Equal(5, files.Count);
        }
    }
}
