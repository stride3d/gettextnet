using NUnit.Framework;

namespace GNU.Gettext.Test
{
    [TestFixture()]
    public class GettextResourceManagerTest
    {
        [Test()]
        public void NamesExtractionTest()
        {
            string n1 = "One.Two.Three";
            Assert.That("Three", Is.EqualTo(GettextResourceManager.ExtractClassName(n1)));
            Assert.That("One.Two", Is.EqualTo(GettextResourceManager.ExtractNamespace(n1)));

            Assert.That("Class", Is.EqualTo(GettextResourceManager.ExtractClassName("Class")));
            Assert.That(string.Empty, Is.EqualTo(GettextResourceManager.ExtractNamespace(".Test")));
        }

        [Test]
        public void Ex1Test()
        {
            Assert.Throws<Exception>(() => GettextResourceManager.ExtractClassName(null));
        }

        [Test]
        public void Ex2Test()
        {
            Assert.Throws<Exception>(() =>
            GettextResourceManager.ExtractClassName(string.Empty));
        }

        [Test]
        public void Ex3Test()
        {
            Assert.Throws<Exception>(() =>
            GettextResourceManager.ExtractClassName("Class."));
        }
    }
}

