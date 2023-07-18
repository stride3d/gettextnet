using NUnit.Framework;
using System;

namespace GNU.Gettext.Test
{
    [TestFixture()]
	public class GettextResourceManagerTest
	{
		[Test()]
		public void NamesExtractionTest()
		{
			string n1 = "One.Two.Three";
			Assert.AreEqual("Three", GettextResourceManager.ExtractClassName(n1));
			Assert.AreEqual("One.Two", GettextResourceManager.ExtractNamespace(n1));

			Assert.AreEqual("Class", GettextResourceManager.ExtractClassName("Class"));
			Assert.AreEqual(string.Empty, GettextResourceManager.ExtractNamespace(".Test"));
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

