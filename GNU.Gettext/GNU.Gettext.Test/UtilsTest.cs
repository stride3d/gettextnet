using System;
using System.Collections.Generic;
using System.IO;

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

            Assert.AreEqual("test/test.htm", FileUtils.GetRelativeUri("http://www.contoso.com/test/test.htm", "http://www.contoso.com/"));
            Assert.AreEqual("../", FileUtils.GetRelativeUri("http://www.contoso.com/", "http://www.contoso.com/test1/dummy"));

            Assert.AreEqual("../../Messages.pot", FileUtils.GetRelativeUri(@"C:\dir1\dir2\Messages.pot", @"C:\dir1\dir2\dir3\dir4\"));
            Assert.AreEqual("../../Messages.pot", FileUtils.GetRelativeUri(
                Path.Combine(Environment.CurrentDirectory, "Messages.pot"),
                Path.Combine(Environment.CurrentDirectory, string.Format("dir{0}subdir{0}", Path.DirectorySeparatorChar))));
        }

        [Test()]
        public void ReadStringsTest()
        {
            List<string> files = FileUtils.ReadStrings("./Data/UtilsTest.txt");
            Assert.AreEqual(3, files.Count, "Simple reading");

            FileUtils.ReadStrings("./Data/UtilsTest.txt", files);
            Assert.AreEqual(3, files.Count, "Merged with duplicates");

            files = new List<string>
            {
                "Test 1",
                "Test 2"
            };
            FileUtils.ReadStrings("./Data/UtilsTest.txt", files);
            Assert.AreEqual(5, files.Count, "Merge failed");
        }
    }
}

