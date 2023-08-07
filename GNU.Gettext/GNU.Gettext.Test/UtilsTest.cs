using NUnit.Framework;

using GNU.Gettext.Utils;

namespace GNU.Gettext.Test
{
    [TestFixture()]
    public class UtilsTest
    {
        [Test()]
        public void RelativePathTest()
        {

            Assert.That("test/test.htm", Is.EqualTo(FileUtils.GetRelativeUri("http://www.contoso.com/test/test.htm", "http://www.contoso.com/")));
            Assert.That("../", Is.EqualTo(FileUtils.GetRelativeUri("http://www.contoso.com/", "http://www.contoso.com/test1/dummy")));

            Assert.That("../../Messages.pot", Is.EqualTo(FileUtils.GetRelativeUri(@"C:\dir1\dir2\Messages.pot", @"C:\dir1\dir2\dir3\dir4\")));
            Assert.That("../../Messages.pot", Is.EqualTo(FileUtils.GetRelativeUri(
                Path.Combine(Environment.CurrentDirectory, "Messages.pot"),
                Path.Combine(Environment.CurrentDirectory, string.Format("dir{0}subdir{0}", Path.DirectorySeparatorChar)))));
        }

        [Test()]
        public void ReadStringsTest()
        {
            List<string> files = FileUtils.ReadStrings("./Data/UtilsTest.txt");
            Assert.That(3, Is.EqualTo(files.Count), "Simple reading");

            FileUtils.ReadStrings("./Data/UtilsTest.txt", files);
            Assert.That(3, Is.EqualTo(files.Count), "Merged with duplicates");

            files = new List<string>
            {
                "Test 1",
                "Test 2"
            };
            FileUtils.ReadStrings("./Data/UtilsTest.txt", files);
            Assert.That(5, Is.EqualTo(files.Count), "Merge failed");
        }
    }
}

